using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shard.API.Models;
using System.Collections.Immutable;

namespace Shard.API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly List<User> _users;

        public UnitsController([FromServices] List<User> list)
        {
            _users = list;
        }

        [HttpGet("{id}/[controller]")]
        public ActionResult<List<Unit>> GetUserUnits(string id)
        {
            foreach (var user in _users)
            {
                if (user.Id == id)
                    return user.Units;
            }
            return NotFound();
        }

        [HttpGet("{id}/[controller]/{unitId}")]
        public ActionResult<Unit> GetUserUnit(string id, string unitId)
        {
            foreach (var user in _users)
            {
                if (user.Id == id)
                {
                    foreach (var unit in user.Units)
                    {
                        if (unit.Id == unitId)
                            return unit;
                    }
                }
            }
            return NotFound();

        }

        [HttpPut("{id}/[controller]/{unitId}")]
        public ActionResult<Unit> moveUnits(string id, string unitId, [FromBody]Unit newUnit)
        {
            foreach (var user in _users)
            {
                if (user.Id == id)
                {
                    foreach (var unit in user.Units)
                    {
                        if (unit.Id == unitId)
                        {
                            if (newUnit == null || newUnit.Id != unitId)
                                return BadRequest();
                            unit.system = newUnit.system;
                            unit.planet = newUnit.planet;
                            return unit;
                        }
                    }
                }
            }
            return NotFound();
        }
    }
}
