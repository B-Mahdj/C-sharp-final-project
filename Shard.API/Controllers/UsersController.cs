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
        private readonly List<User> _users;
        private readonly Sector _sector;

        public UsersController(List<User> list, Sector sector)
        {
            _users = list;
            _sector = sector;
        }

        [HttpPut("{id}")]
        public ActionResult<UserJson> AddUser(User user, string id)
        {
            StarSystem system = _sector.GetOneRandomStarSystem();
            //TODO : Return 404 if : 
            //If there is no body, or if the id in the body is different than the one in the url.
            if (user == null || user.Id != id || !(Regex.IsMatch(user.Id, "^[a-zA-Z0-9_-]+$")))
            {
                return BadRequest();
            }
            string[] type = { "scout", "builder" };
            string systemName = system.GetOneRandomPlanet().Name;
            Unit firstUnit = new(Guid.NewGuid().ToString(), "scout", system.Name, systemName );
            Unit secondUnit = new(Guid.NewGuid().ToString(), "builder", system.Name, null);
            user.Units.Add(firstUnit);
            user.Units.Add(secondUnit);
            _users.Add(user);
            return new UserJson(user);
        }

        [HttpGet("{id}")]
        public ActionResult<UserJson> GetUser(string id)
        {
            foreach(var u in _users)
            {
                if(u.Id == id)
                {
                    return new UserJson(u);
                }
            }
            return NotFound();
        }


    }
}
