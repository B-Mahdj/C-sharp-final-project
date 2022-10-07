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
        private Dictionary<User, List<Unit>> _units;
        private Sector _sector;

        public UsersController([FromServices] List<User> list, [FromServices]Dictionary<User, List<Unit>> units, [FromServices]Sector sector)
        {
            _users = list;
            _units = units;
            _sector = sector;
        }

        [HttpPut("{id}")]
        public ActionResult<User> AddUser(User user, string id)
        {
            StarSystem system = _sector.GetOneRandomStarSystem();
            //TODO : Return 404 if : 
            //If there is no body, or if the id in the body is different than the one in the url.
            if (user == null || user.id != id || !(Regex.IsMatch(user.id, "^[a-zA-Z0-9_-]+$")))
            {
                return BadRequest();
            }
            _users.Add(user);
            Unit firstUnit = new(Guid.NewGuid().ToString(),"scout", system.Name, system.GetOneRandomPlanet().Name);
            _units.Add(user, new List<Unit>() {firstUnit});
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
