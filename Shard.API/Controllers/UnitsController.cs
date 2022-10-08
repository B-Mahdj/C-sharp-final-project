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
        private readonly Sector _sector;

        public UnitsController([FromServices] List<User> list, [FromServices] Sector sector)
        {
            _users = list;
            _sector = sector;
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
                            unit.System = newUnit.System;
                            unit.Planet = newUnit.Planet;
                            return unit;
                        }
                    }
                }
            }
            return NotFound();
        }

        [HttpGet("{id}/[controller]/{unitId}/location")]
        public ActionResult<UnitLocation> GetUnitLocation(string id, string unitId)
        {
            foreach (var user in _users)
            {
                if (user.Id == id)
                {
                    foreach (var unit in user.Units)
                    {
                        if (unit.Id == unitId)
                        {
                            var system = (from s in _sector.Systems
                                         where s.Name == unit.System
                                         select s).ToList();

                            var planet = (from p in system[0].Planets
                                         where p.Name == unit.Planet
                                         select p).ToList();
                            return new UnitLocation(system[0].Name, planet[0].Name, planet[0].ResourceQuantity);
                        }
                    }
                }
            }
            return NotFound();
        }

    }
}
