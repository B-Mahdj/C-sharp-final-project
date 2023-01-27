using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Shard.API.Models;
using Shard.Shared.Core;
using System.Text.RegularExpressions;

namespace Shard.API.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly List<User> _users;
        private readonly Sector _sector;
        private readonly IClock _systemClock;

        public UsersController(List<User> list, Sector sector, IClock systemClock)
        {
            _users = list;
            _sector = sector;
            _systemClock = systemClock;
        }

        [HttpPut("{id}")]
        public ActionResult<UserJson> AddUser(User user, string id)
        {

            StarSystem system = _sector.GetOneRandomStarSystem();
            
            if (user == null || user.Id != id || !(Regex.IsMatch(user.Id, "^[a-zA-Z0-9_-]+$")))
            {
                return BadRequest();
            }
            bool isAuthorized = Request.Headers.TryGetValue("Authorization", out StringValues headerValues);
            User? existingUser = _users.FirstOrDefault(x => x.Id == user.Id);
            if (existingUser != null)
            {
                if (isAuthorized)
                {    
                    existingUser.ResourcesQuantity = user.ResourcesQuantity;
                }
                return new UserJson(existingUser);
            }
            string systemName = system.GetOneRandomPlanet().Name;
            if (!isAuthorized)
            {
                Unit firstUnit = new(Guid.NewGuid().ToString(), "scout", system.Name, null, _systemClock, _users, null);
                Unit secondUnit = new(Guid.NewGuid().ToString(), "builder", system.Name,null, _systemClock, _users, null);
                user.Units.Add(firstUnit);
                user.Units.Add(secondUnit);
            }
            else
            {
                foreach (ResourceKind resource in Enum.GetValues(typeof(ResourceKind)))
                    user.ResourcesQuantity[resource] = 0;
            }
            
            _users.Add(user);
            return new UserJson(user);
        }

        [HttpGet("{id}")]
        public ActionResult<UserJson> GetUser(string id)
        {
            User? user = _users.FirstOrDefault(x => x.Id == id);
            
            if(user != null)
            {
                return new UserJson(user);
            }
            return NotFound();
        }

    }
}
