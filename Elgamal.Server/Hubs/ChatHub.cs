using Elgamal.Lib;
using Microsoft.AspNetCore.SignalR;
using System.Numerics;

namespace Elgamal.Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserContext _userContext;

        public ChatHub(UserContext userContext)
        {
            _userContext = userContext;
        }

        public ParamsDto GetParams()
            => ParamsDto.FromElgamalParameters(PrimeGetter.Parameters);

        public bool Init(string fromUser, byte[] publicKey, bool isMITM)
        {
            if (_userContext.Users.Any(x => x.Name == fromUser))
                return false;

            _userContext.Add(new()
            {
                Name = fromUser,
                Key = new BigInteger(publicKey),
                IsMITM = isMITM,
            });

            return true;
        }

        public byte[]? Connect(string toUser)
        {
            return _userContext.Users.FirstOrDefault(x => x.Name == toUser)?.Key.ToByteArray();
        }

        public async Task SendAsync(string who, byte[] message)
        {
            await Clients.All.SendAsync("Receive", who, message);
        }
    }
}
