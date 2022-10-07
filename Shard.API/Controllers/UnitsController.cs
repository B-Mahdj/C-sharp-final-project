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
        private Dictionary<User, List<Unit>> _units;

        public UnitsController([FromServices]Dictionary<User,List<Unit>> list)
        {
            _units = list;
        }

        [HttpGet("{id}/[controller]")]
        public ActionResult<List<Unit>> GetUserUnits(string id)
        {
            foreach (var unit in _units)
            {
                if (unit.Key.id == id) 
                    return Ok(unit.Value);
            }
            return NotFound();
        }

        [HttpGet("{id}/[controller]/{unitId}")]
        public ActionResult<List<Unit>> GetUserUnit(string id, string unitId)
        {
            foreach (var unit in _units)
            {
                if (unit.Key.id == id)
                {
                    foreach (var unit2 in unit.Value)
                    {
                        if (unit2.Id == unitId) 
                            return Ok(unit2);
                    }
                }
            }
            return NotFound();

        }

        [HttpPut("{id}/[controller]/{unitId}")]
        public ActionResult<Unit> moveUnits(string id, string unitId, [FromBody]Unit newUnit)
        {
            foreach (var unit in _units)
            {
                if (unit.Key.id == id)
                {
                    foreach (var unit2 in unit.Value)
                    {
                        if (unit2.Id == unitId)
                        {
                            if (newUnit == null || newUnit.Id != unitId)
                                return BadRequest();
                            unit2.system = newUnit.system;
                            unit2.planet = newUnit.planet;
                            return Ok(unit2);
                        }

                    }
                }
            }
            return NotFound();
        }
    }
}
