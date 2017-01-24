using IDDD.App.Cqs.Commands.Client;
using IDDD.Common.Cqs;
using IDDD.Common.Cqs.Command;
using IDDD.Common.Domain;
using IDDD.Domain.Membership.Clients;
using IDDD.Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IDDD.App.Cqs.CommandHandlers.Clients
{
    public class RegisterClientCommandHandler :
                IAsyncCommandHandler<RegisterClientCommand, Result>
    {
        private readonly IClientFinder _finder;
        private readonly IClientRepository _repo;
        public RegisterClientCommandHandler(IClientRepository repo,
            IClientFinder finder)
        {
            _repo = repo;
            _finder = finder;
        }

        public async Task<Result> HandleAsync(RegisterClientCommand command)
        {
            if(!(await VerifyNotExisting(command.Key)).Succeeded)
            {
                return await Task.FromResult(Result.Fail($"Client {command.Key} already exists in the system."));
            }

            var client = new Client
            {
                Id = new ClientId(Guid.NewGuid().ToString()),
                Key = command.Key,
                Name = command.Name,
                ApplicationType = command.ApplicationType,
                Active = true,
                AllowedOrigin = command.Origin,
                RedirectUri = command.RedirectUri.TrimEnd('/'),
                LogoutRedirectUri = command.LogoutRedirectUri.TrimEnd('/'),
                ConfirmationUri = command.ConfirmationUri.TrimEnd('/')
            };

            await _repo.Create(client);
            return Result.Ok();
        }

        private async Task<Result> VerifyNotExisting(string key)
        {
            var existings = await _finder.All();
            return await Task.Factory.StartNew(() =>
            {
                if (existings.FirstOrDefault(criteria => criteria.Key == key) != null)
                {
                    return Result.Fail("Client already exists");
                }
                return Result.Ok();
            });

        }
    }
}
