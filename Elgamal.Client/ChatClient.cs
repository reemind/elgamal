using Elgamal.Lib;
using Microsoft.AspNetCore.SignalR.Client;
using System.Numerics;
using System.Text;

namespace Elgamal.Client
{
    public class ChatClient : IDisposable
    {
        private readonly CancellationTokenSource source;
        private readonly ConsoleManager _consoleManager;
        private readonly HubConnection _connection;
        private string _userName;
        private Elgamal.Lib.IKey privateKey;
        private Elgamal.Lib.IKey publicKey;
        private Elgamal.Lib.IKey remotePublicKey;
        private byte[] key;
        private bool _mitm;
        private string _reciever;

        public ChatClient(string url)
        {
            source = new();
            _consoleManager = new();
            _connection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();
            _connection.Closed += Closed;
            _connection.On<string, byte[]>("Receive", OnMessageRecieved);
        }

        private void OnMessageRecieved(string who, byte[] msg)
        {
            if (who != _reciever && !_mitm)
                return;

            var message = Lib.Elgamal.Decrypt(msg, privateKey);
            _consoleManager.WriteLine(who + "# " + Encoding.UTF8.GetString(message));
        }

        private Task Closed(Exception? error)
        {
            if (error is not null)
            {
                _consoleManager.WriteLine(error.Message, ConsoleColor.Red);
            }

            return Task.CompletedTask;
        }

        public async Task Start(string username, bool mitm)
        {
            this._userName = username;
            _mitm = mitm;
            _consoleManager.Prefix = username;

            _consoleManager.WriteLeadLine();
            while (!source.Token.IsCancellationRequested)
            {
                while (_connection.State == HubConnectionState.Connected)
                {
                    string message = _consoleManager.ReadLine().Trim();
                    if (message == "/exit")
                    {
                        _consoleManager.WriteLine("Exiting...", ConsoleColor.Green);
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(message) && _connection.State == HubConnectionState.Connected)
                    {

                        var encMessage = Lib.Elgamal.Encrypt(Encoding.UTF8.GetBytes(message), remotePublicKey);

                        await _connection.SendAsync("SendAsync", username, encMessage, source.Token);
                        _consoleManager.WriteLine(username + "# " + message, ConsoleColor.Cyan);
                    }
                }
                _consoleManager.WriteLine("Connecting to server...", ConsoleColor.Green);

                while (_connection.State != HubConnectionState.Connected)
                {
                    try
                    {
                        _connection.StartAsync(source.Token).Wait(2000);

                        var parameters = (await _connection.InvokeAsync<ParamsDto>(
                            "GetParams",
                            cancellationToken: source.Token)).ToElgamalParameters();

                        var keyPair = ElgamalKey.GenerateKeyPair(parameters)!;
                        privateKey = keyPair.PrivateKey;
                        publicKey = keyPair.PublicKey;

                        await _connection.InvokeAsync<bool>(
                            "Init",
                            username,
                            (publicKey as ElgamalKey)?.Key.ToByteArray(),
                            mitm,
                            cancellationToken: source.Token);

                        Console.Write("Введите получателя: ");
                        _reciever = Console.ReadLine();

                        var remotePublicKeyValue = await _connection.InvokeAsync<byte[]?>(
                            "Connect",
                            _reciever,
                            cancellationToken: source.Token);

                        remotePublicKey = new ElgamalKey(false, new BigInteger(remotePublicKeyValue), parameters);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                _consoleManager.WriteLine("Connected!", ConsoleColor.Green);
            }
        }
        public async Task Stop()
        {
            await _connection.StopAsync(source.Token);
        }

        public void Dispose()
        {
            Task.WaitAll(Stop());
            source.Cancel();
            source.Dispose();
            _connection.DisposeAsync();
        }
    }
}