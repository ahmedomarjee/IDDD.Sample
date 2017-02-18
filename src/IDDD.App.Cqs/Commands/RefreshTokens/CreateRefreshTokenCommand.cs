using System;
using IDDD.Core.Cqs.Command;
using System.ComponentModel.DataAnnotations;

namespace IDDD.App.Cqs.Commands.RefreshTokens
{
    public class CreateRefreshTokenCommand : ICommand
    {
        public string TicketId { get; }

        public string ClientId { get; }
        public string UserId { get; }
        public string UserName { get; set; }

        public string IpAddress { get; }

        public DateTime ExpiresUtc { get; }
        public DateTime IssuedUtc { get; set; }

        public CreateRefreshTokenCommand(string ticketId, string clientId, string userId, string userName, string ipAddress, DateTime issuedUtc, DateTime expiresUtc)
        {
            TicketId = ticketId;
            ClientId = clientId;
            UserId = userId;
            UserName = userName;
            IpAddress = ipAddress;
            IssuedUtc = issuedUtc;
            ExpiresUtc = expiresUtc;
        }
    }
}
