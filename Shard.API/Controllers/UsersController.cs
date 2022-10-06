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
        private List<User> _users;

        public UsersController([FromServices] List<User> list)
        {
            _users = list;
        }

        [HttpPut("{id}")]
        public ActionResult<User> AddUser(string id, string pseudo)
        {

            //TODO : Return 404 if : 
            //If there is no body, or if the id in the body is different than the one in the url.
            User user = new User(id, pseudo);
            _users.Add(user);
            return Ok(user);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUser(string id)
        {
            foreach(var u in _users)
            {
                if(u.id == id)
                {
                    return Ok(u);
                }
            }
            return BadRequest();
        }


    }
}
