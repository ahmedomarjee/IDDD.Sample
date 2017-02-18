using IDDD.Core.Cqs.Query;
using System.Threading.Tasks;
using IDDD.Domain.Membership.Tokens;
using IDDD.App.Cqs.Queries.RefreshTokens;
using System.Linq;
using IDDD.Core.Cqs;
using IDDD.Core;

namespace IDDD.App.Cqs.QueryHandlers.RefreshTokens
{
    public class ValidateRefreshTokenHandler : 
        IHandleQueryAsync<RefreshTokenValidatorQuery, Result>
    {
        private readonly IRefreshTokenFinder _finder;

        public ValidateRefreshTokenHandler(IRefreshTokenFinder finder)
        {
            _finder = finder;
        }

        public async Task<Result> ExecuteAsync(RefreshTokenValidatorQuery query)
        {
            var result = Validate(query);
            return await Task.Factory.StartNew(() =>
            {
                return result;
            });
        }

        private Result Validate(RefreshTokenValidatorQuery query)
        {
            RefreshToken token =
                _finder.All().Result
                .FirstOrDefault(criteria => criteria.TicketId == query.TicketId);
            if (token == null)
            {
                return Result.Fail("Unknown refresh token");
            }

            if (token.UserId.Id != query.UserId || token.ClientId.Id != query.ClientId)
            {
                return Result.Fail("Invalid refresh token");
            }

            return Result.Ok();
        }
    }
}