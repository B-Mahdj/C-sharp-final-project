using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Shard.API.Models;
using System.Text.RegularExpressions;

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
        public ActionResult<User> AddUser(User user, string id)
        {

            //TODO : Return 404 if : 
            //If there is no body, or if the id in the body is different than the one in the url.
            if (user == null || user.id != id || !(Regex.IsMatch(user.id, "^[a-zA-Z0-9]*$")))
            {
                return BadRequest();
            }

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
            return NotFound();
        }


    }
}
