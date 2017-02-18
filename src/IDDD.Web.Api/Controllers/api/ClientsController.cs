using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IDDD.App.Cqs.Commands.Client;
using IDDD.Core.Cqs.Command;
using Microsoft.AspNetCore.Authorization;
using IDDD.Core;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace IDDD.Web.Api.Controllers.api
{
    [Route("api/[controller]")]
    public class ClientsController : BaseController
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public ClientsController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        [AllowAnonymous]
        [HttpPost]       
        public async Task<IActionResult> Post([FromBody]RegisterClientCommand command)
        {
            if (!ModelState.IsValid)
            {
                return ErrorModelResult();
            }
            try
            {
                var result = await _commandDispatcher.DispatchAsync<RegisterClientCommand, Result>(command);

                return ToResult(result);
            }
            catch(FailureResult reqEx)
            {
                return ErrorResult(reqEx);
            }
        }

    }
}
