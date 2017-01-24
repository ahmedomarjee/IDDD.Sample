using IDDD.Domain.Membership.Clients;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Driver;
using IDDD.Infrastructure.Mongo;
using System.Collections.Generic;

namespace IDDD.Infrastructure.Mongo.Repositories.Clients
{
    public class ClientFinder : MongoDbBase, IClientFinder
    {
        public ClientFinder(IMongoContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Client>> All(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Task.Factory.StartNew(() =>
            {
                return _dbContext.Clients.AsQueryable().AsEnumerable();
            }, cancellationToken);
        }

        public async Task<Maybe<Client>> GetById(ClientId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<Client>.Filter.Eq(s => s.Id, id);
            var result = _dbContext.Clients.Find(filter).ToListAsync();
            return await Task.Factory.StartNew(() =>
            {
                var first = result.Result.FirstOrDefault();
                return first;
            }, cancellationToken);
        }
    }
}
