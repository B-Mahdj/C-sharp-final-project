using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shard.API.Models;

namespace Shard.API.Controllers
{
    [Route("users")]
    [ApiController]
    public class BuildingsController : ControllerBase
    {

        private readonly List<User> _users;
        private readonly Sector _sector;

        public BuildingsController(List<User> list, Sector sector)
        {
            _users = list;
            _sector = sector;
        }

        [HttpPost("{userId}/[controller]")]
        public ActionResult<BuildingJson> AddMine(string userId, Building building)
        {
            User? user = _users.FirstOrDefault(x => x.Id == userId);
            if (user == null) return NotFound();
            Unit? unit = user.Units.FirstOrDefault(x => x.Type.Equals("builder"));
            building.Id = Guid.NewGuid().ToString();
            /*building.System = unit.System;
            building.Planet = unit.Planet;*/
            if (unit == null || building == null || building.Type != "mine" || building.BuilderId != unit.Id || unit.Planet == null)
            {
                return BadRequest();
            }
            
           /* foreach (var s in _sector.Systems)
            {
                if (s.Name == unit.System)
                {
                    foreach (var p in s.Planets)
                    {
                        if (p.Name == unit.Planet)
                        {
                            p.Buildings.Add(building);
                        }
                    }
                }
            }*/
            return new BuildingJson(building);
        }
    }
}
