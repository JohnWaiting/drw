using System.Linq;
using drw.server.Model;
using Microsoft.AspNetCore.Mvc;

namespace drw.server.Controllers
{
    public class LifeController : ControllerBase
    {
        private readonly UserManagerService _userManagerService;

        public LifeController(UserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }

        [HttpGet]
        [Route("version")]
        public JsonResult GetVersion()
        {
            return new JsonResult(new
            {
                Version = "1.0.0.6"
            });
        }

        [HttpGet]
        [Route("games")]
        public JsonResult GetActiveGames()
        {
            return new JsonResult(
                _userManagerService.GetActiveGames().Select(game => 
                    new
                    {
                        Game = game,
                        Players = _userManagerService.GetActivePlayers(game)
                    }
                ));
        }
    }
}
