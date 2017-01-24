using IDDD.Domain.Membership.Clients;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Driver;

namespace IDDD.Infrastructure.Mongo.Repositories.Clients
{
    public class ClientRepository : MongoDbBase, IClientRepository
    {
        public ClientRepository(IMongoContext dbContext) : base(dbContext)
        {
        }

        public async Task Create(Client entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _dbContext.Clients.InsertOneAsync(entity);
        }

        public async Task Delete(Client entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<Client>.Filter.Eq(s => s.Id, entity.Id);
            await _dbContext.Clients.DeleteOneAsync(filter);
        }

        public async Task Update(Client entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<Client>.Filter.Eq(s => s.Id, entity.Id);
            await _dbContext.Clients.ReplaceOneAsync(filter, entity);
        }
    }
}
