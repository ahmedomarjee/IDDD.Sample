using System;
using IDDD.Common.Cqs.Command;

namespace IDDD.App.Cqs.Commands.RefreshTokens
{
    public class DeleteRefreshTokenByTokenCommand : ICommand
    {
        public string TicketId { get; }

        public DeleteRefreshTokenByTokenCommand(string ticketId)
        {
            TicketId = ticketId;
        }
    }
}
