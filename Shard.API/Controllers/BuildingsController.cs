using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shard.API.Models;
using Shard.Shared.Core;

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
            MineRessources(building, user);
            return new BuildingJson(building);
        }

        private void MineRessources(Building mine, User user)
        {
            if (mine.ResourceCategory == "solid")
            {
                MineSolid(mine, user);
            }
            else if (mine.ResourceCategory == "liquid")
            {
                MineLiquid(mine, user);
            }
            else if (mine.ResourceCategory == "gaseous")
            {
                MineGas(mine, user);
            }
        }

        private async Task MineSolid(Building mine, User user)
        {
            StarSystem? system = _sector.GetStarSystemByName(mine.System);
            if (system == null) return;
            Planet? planet = system.GetPlanetByName(mine.Planet);
            if (planet == null) return;
            while (true)
            {
                await Task.Delay(60000);
                if (planet.GetNumberOfSolidRessourcesLeft() > 0)
                {

                }
                else
                {
                    // For now nothing, because we may have organic ressources in V5 but we should return the task here 
                    // to avoid having ressources being used for nothing
                }

            }
        }

        private async Task MineLiquid(Building mine, User user)
        {
            StarSystem? system = _sector.GetStarSystemByName(mine.System);
            if (system == null) return;
            Planet? planet = system.GetPlanetByName(mine.Planet);
            if (planet == null) return;
            while (true)
            {
                if (planet.GetNumberOfLiquidRessourcesLeft() > 0)
                {
                    // Add one ressource "water" to user and reduce this ressource from planet
                    user.ResourcesQuantity.
                }
            }
        }

        private async Task MineGas(Building mine, User user)
        {
            StarSystem? system = _sector.GetStarSystemByName(mine.System);
            if (system == null) return;
            Planet? planet = system.GetPlanetByName(mine.Planet);
            if (planet == null) return;
            while (true)
            {
                if (planet.GetNumberOfGasRessourcesLeft() > 0)
                {
                    // Add one ressource "oxygen" to user from planet
                    
                }
            }
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
