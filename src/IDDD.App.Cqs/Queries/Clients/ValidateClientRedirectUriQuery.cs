using IDDD.Common.Cqs;
using IDDD.Common.Cqs.Query;
using IDDD.Common;

namespace IDDD.App.Cqs.Queries.Clients
{
    public class ValidateClientRedirectUriQuery : IQuery<Result>
    {
        public string ClientId { get; }
        public string RedirectUri { get; }

        public ValidateClientRedirectUriQuery(string clientId, string redirectUri)
        {
            ClientId = clientId;
            RedirectUri = redirectUri;
        }
    }
}