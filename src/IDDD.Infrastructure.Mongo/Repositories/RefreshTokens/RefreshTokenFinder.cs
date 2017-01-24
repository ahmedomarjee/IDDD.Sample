using IDDD.Domain.Membership.Tokens;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Driver;
using System.Collections.Generic;

namespace IDDD.Infrastructure.Mongo.Repositories.RefreshTokens
{
    public class RefreshTokenFinder : MongoDbBase, IRefreshTokenFinder
    {
        public RefreshTokenFinder(IMongoContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<RefreshToken>> All(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Task.Factory.StartNew(() =>
            {
                return _dbContext.RefreshTokens.AsQueryable().AsEnumerable();
            }, cancellationToken);
        }

        public async Task<Maybe<RefreshToken>> GetById(RefreshTokenId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<RefreshToken>.Filter.Eq(s => s.Id, id);
            var result = _dbContext.RefreshTokens.Find(filter).ToListAsync();
            return await Task.Factory.StartNew(() =>
            {
                var first = result.Result.FirstOrDefault();
                return first;
            }, cancellationToken);
        }
    }
}
