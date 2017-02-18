using System;
using System.Threading.Tasks;
using IDDD.Core.Cqs.Command;
using IDDD.Domain.Membership.Tokens;
using IDDD.App.Cqs.Commands.RefreshTokens;
using System.Linq;
using IDDD.Core.Cqs;
using IDDD.Core.Domain;
using IDDD.Core;

namespace IDDD.App.Cqs.CommandHandlers.RefreshTokens
{
    public class CreateRefreshTokenHandler : 
        IAsyncCommandHandler<CreateRefreshTokenCommand, Result>
    {
        private readonly IRefreshTokenRepository _repo;
        private readonly IRefreshTokenFinder _finder;
        public CreateRefreshTokenHandler(IRefreshTokenRepository repo,
            IRefreshTokenFinder finder)
        {
            _repo = repo;
            _finder = finder;
        }

        public async Task<Result> HandleAsync(CreateRefreshTokenCommand command)
        {
            if(!(await VerifyNotExisting(command.TicketId)).Succeeded)
            {
                return await Task.FromResult(Result.Fail($"RefreshToken {command.TicketId} is already register in the system"));

            }

            var token = new RefreshToken
            {
                Id = new RefreshTokenId(Guid.NewGuid().ToString()),
                TicketId = command.TicketId,
                ClientId = new Domain.Membership.Clients.ClientId(command.ClientId),
                UserId = new Domain.Membership.IdentityUserId(command.UserId),
                IpAddress = command.IpAddress,
                ExpiresUtc = command.ExpiresUtc,
                IssuedUtc = command.IssuedUtc
            };

            await _repo.Create(token);

            return Result.Ok();
        }

        private async Task<Result> VerifyNotExisting(string ticketId)
        {
            var existings = await _finder.All();
            return await Task.Factory.StartNew(() =>
            {
                if (existings.FirstOrDefault(criteria => criteria.TicketId == ticketId) != null)
                {
                    return Result.Fail("Refresh token exists with");
                }
                return Result.Ok();
            });
        }
    }
}