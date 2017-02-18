using IDDD.Core.Cqs;
using IDDD.Core.Cqs.Query;
using IDDD.Core;

namespace IDDD.App.Cqs.Queries.RefreshTokens
{
    public class RefreshTokenValidatorQuery : IQuery<Result>
    {
        public string TicketId { get; set; }
        public string ClientId { get; set; }
        public string UserId { get; set; }

        public RefreshTokenValidatorQuery(string ticketId, string clientId, string userId)
        {
            TicketId = ticketId;
            ClientId = clientId;
            UserId = userId;
        }
    }
}