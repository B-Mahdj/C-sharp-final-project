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
            
            if (unit == null || building == null || building.Type != "mine" || building.BuilderId != unit.Id || unit.Planet==null)
            {
                return BadRequest();
            }
            if (building.ResourceCategory != "solid" && building.ResourceCategory != "liquid" && building.ResourceCategory != "gaseous")
            {
                return BadRequest();
            }
            building.Id = Guid.NewGuid().ToString();
            building.System = unit.System;
            building.Planet = unit.Planet;
            user.Buildings.Add(building);
            return new BuildingJson(building);
        }

        [HttpGet("{userId}/[controller]")]
        public ActionResult<List<Building>> GetBuildings(string userId)
        {
            User? user = _users.FirstOrDefault(x => x.Id == userId);
            if (user == null) return NotFound();
            return user.Buildings;
        }

        [HttpGet("{userId}/[controller]/{buildingId}")]
        public ActionResult<Building> GetBuilding(string userId, string buildingId)
        {
            User? user = _users.FirstOrDefault(x => x.Id == userId);
            if (user == null) return NotFound();
            Building? building = user.Buildings.FirstOrDefault(x => x.Id == buildingId && x.Type == "mine");
            if (building == null) return NotFound();
            return building;
        }
    }
}
