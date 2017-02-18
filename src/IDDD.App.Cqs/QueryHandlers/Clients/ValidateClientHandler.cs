using System.Threading.Tasks;
using IDDD.Core.Cqs.Query;
using IDDD.Domain.Membership.Clients;
using IDDD.App.Cqs.QueryResult.Clients;
using IDDD.App.Cqs.Queries.Clients;
using System.Threading;
using System.Linq;

namespace IDDD.App.Cqs.QueryHandlers.Clients
{
    public class ValidateClientHandler :
        IHandleQueryAsync<ValidateClientQuery, ValidateClientResult>
    {
        private readonly IClientFinder _clientFinder;

        public ValidateClientHandler(IClientFinder clientFinder)
        {
            _clientFinder = clientFinder;
        }

        public async Task<ValidateClientResult> ExecuteAsync(ValidateClientQuery query)
        {
            return await Task.Factory.StartNew(() =>
            {
                return GetResult(query);
            });
        }

        private ValidateClientResult GetResult(ValidateClientQuery query)
        {
            var result = _clientFinder.All().Result;
            var client = result.FirstOrDefault(criteria => criteria.Key.Equals(query.ClientId));
            if (client == null)
            {
                return ValidateClientResult.Failed($"Client '{query.ClientId}' is not registered in the system.");
            }

            if (!client.Active)
            {
                return ValidateClientResult.Failed("Client is inactive.");
            }

            return new ValidateClientResult(true, client.Id.Id, client.Name, client.AllowedOrigin, client.RedirectUri);
        }
    }
}