using IDDD.App.Cqs.Commands.RefreshTokens;
using IDDD.Core.Cqs;
using IDDD.Core.Cqs.Command;
using IDDD.Domain.Membership.Tokens;
using IDDD.Core;
using System.Linq;
using System.Threading.Tasks;

namespace IDDD.App.Cqs.CommandHandlers.RefreshTokens
{
    public class DeleteRefreshTokenHandler :
                IAsyncCommandHandler<DeleteRefreshTokenByTokenCommand, Result>
    {
        private readonly IRefreshTokenRepository _repo;
        private readonly IRefreshTokenFinder _finder;
        public DeleteRefreshTokenHandler(IRefreshTokenRepository repo,
            IRefreshTokenFinder finder)
        {
            _repo = repo;
            _finder = finder;
        }

        public async Task<Result> HandleAsync(DeleteRefreshTokenByTokenCommand command)
        {
            return await GetResult(command);
        }

        private async Task<Result> GetResult(DeleteRefreshTokenByTokenCommand command)
        {
            var token =
                 _finder.All().Result.FirstOrDefault(t => t.TicketId == command.TicketId);
            if (token == null)
            {
                return Result.Fail("Could not find token");
            }
            await _repo.Delete(token);
            return Result.Ok();
        }
    }
}