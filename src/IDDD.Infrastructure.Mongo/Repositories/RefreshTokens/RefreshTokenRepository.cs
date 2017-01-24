using IDDD.Domain.Membership.Tokens;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Driver;
using IDDD.Infrastructure.Mongo;

namespace IDDD.Infrastructure.Mongo.Repositories.RefreshTokens
{
    public class RefreshTokenRepository : MongoDbBase, IRefreshTokenRepository
    {
        public RefreshTokenRepository(IMongoContext dbContext) : base(dbContext)
        {
        }

        public async Task Create(RefreshToken entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _dbContext.RefreshTokens.InsertOneAsync(entity);
        }

        public async Task Delete(RefreshToken entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<RefreshToken>.Filter.Eq(s => s.Id, entity.Id);
            await _dbContext.RefreshTokens.DeleteOneAsync(filter);
        }

        public async Task<RefreshToken> GetById(RefreshTokenId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<RefreshToken>.Filter.Eq(s => s.Id, id);
            var result = _dbContext.RefreshTokens.Find(filter).FirstOrDefaultAsync();
            return await Task.Factory.StartNew(() =>
            {
                return result.Result;
            }, cancellationToken);
        }

        public async Task Update(RefreshToken entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<RefreshToken>.Filter.Eq(s => s.Id, entity.Id);
            await _dbContext.RefreshTokens.ReplaceOneAsync(filter, entity);
        }
    }
}
