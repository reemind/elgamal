using Microsoft.Extensions.CommandLineUtils;

namespace Elgamal.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication()
            {
                Name = "SignalRClient",
                FullName = "SignalR console chat",
                Description = "Simple SignalR console chat impl"
            };

            app.HelpOption("-h|--help");
            app.VersionOption("--version", "1.0.0");
            var usernameOption = app.Option("-u|--username", "Username", CommandOptionType.SingleValue);
            var serverOption = app.Option("-s|--server", "Server", CommandOptionType.SingleValue);
            var mitmOption = app.Option("--mitm", "Enable MITM", CommandOptionType.NoValue);

            app.OnExecute(async () =>
            {
                if (usernameOption.HasValue() && serverOption.HasValue())
                {
                    using var chatClient = new ChatClient($"http://{serverOption.Value()}/hub");
                    await chatClient.Start(usernameOption.Value(), mitmOption.HasValue());
                }

                return 0;
            });
            try
            {
                app.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
