using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIApplication.Controllers
{
    public class ValuesController : Controller
    {
        [HttpGet]
        [Route("ping")]
        public string Ping()
        {
            return "All good. You don't need to be authenticated to call this.";
        }

        [Authorize]
        [HttpGet]
        [Route("secured/ping")]
        public string PingSecured()
        {
            return "All good. You only get this message if you are authenticated.";
        }
    }
}
