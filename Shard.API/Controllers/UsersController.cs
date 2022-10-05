using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Shard.API.Models;

namespace Shard.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpPut("{id}")]
        public ActionResult<User> AddUser(string id, string pseudo)
        {

            //TODO : Return 404 if : 
            //If there is no body, or if the id in the body is different than the one in the url.

            return new User(id, pseudo);
        }

        //[HttpGet("{id}")]
        //public ActionResult<User> GetUser(string id)
        //{

        //}


    }
}
