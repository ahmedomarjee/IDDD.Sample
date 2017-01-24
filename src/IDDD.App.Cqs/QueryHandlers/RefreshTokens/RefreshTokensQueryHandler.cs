using IDDD.App.Cqs.Queries.RefreshTokens;
using IDDD.App.Cqs.QueryResult.RefreshTokens;
using IDDD.Common.Cqs.Query;
using IDDD.Domain.Membership.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDDD.App.Cqs.QueryHandlers.RefreshTokens
{
    public class RefreshTokensQueryHandler : 
        IHandleQueryAsync<RefreshTokensQuery, IEnumerable<RefreshTokenResult>>
    {
        private readonly IRefreshTokenFinder _finder;
        public RefreshTokensQueryHandler(IRefreshTokenFinder finder)
              : base()
        {
            _finder = finder;
        }

        public async Task<IEnumerable<RefreshTokenResult>> ExecuteAsync(RefreshTokensQuery query)
        {
            var result = await _finder.All();
            return await Task.Factory.StartNew(() =>
            {
                return result.Select(x => GetResult(x).Result).AsEnumerable();
            });
        }

        private async Task<RefreshTokenResult> GetResult(Maybe<RefreshToken> refreshToken)
        {

            return await Task.Factory.StartNew(() =>
            {
                if (refreshToken.HasNoValue) return default(RefreshTokenResult);
                var token = refreshToken.Value;
                return new RefreshTokenResult
                {
                    Id = Guid.Parse(token.Id.Id),
                    IpAddress = token.IpAddress,
                    ClientId = token.ClientId.Id,
                    ExpiresUtc = token.ExpiresUtc,
                    IssuedUtc = token.IssuedUtc,
                    UserId = token.UserId.Id
                };
            });
        }
    }
}