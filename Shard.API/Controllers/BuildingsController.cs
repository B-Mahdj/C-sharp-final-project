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
        private readonly IClock _systemClock;

        public BuildingsController(List<User> list, Sector sector, IClock systemClock)
        {
            _users = list;
            _sector = sector;
            _systemClock = systemClock;
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
            building.IsBuilt = false;
            building.EstimatedBuildTime = _systemClock.Now.AddMinutes(5);
            building.BuildingTask = BuildMine(building, user);
            user.Buildings.Add(building);
            return new BuildingJson(building);
        }

        private async Task BuildMine(Building building, User user)
        {
            await _systemClock.Delay(300000);
            building.EstimatedBuildTime = null;
            building.IsBuilt = true;
            MineRessources(building, user);
        }

        private async Task MineRessources(Building building, User user)
        {
            if (building.ResourceCategory == "solid")
            {
                MineSolid(building, user);
            }
            else if (building.ResourceCategory == "liquid")
            {
                MineLiquid(building, user);
            }
            else if (building.ResourceCategory == "gaseous")
            {
                MineGas(building, user);
            }
            return;
        }

        private async Task MineSolid(Building mine, User user)
        {
            StarSystem? system = _sector.GetStarSystemByName(mine.System);
            if (system == null) return;
            Planet? planet = system.GetPlanetByName(mine.Planet);
            if (planet == null) return;
            while (true)
            {
                await _systemClock.Delay(60000);
                if (planet.GetNumberOfSolidRessourcesLeft() > 0)
                {
                    // Create a dictionnary that will only contains the solid ressources of the planet
                    Dictionary<ResourceKind, int> solidRessources = planet.GetListOfSolidRessources();

                    // Sort the dictionnary by value in descending order if two values are equal, sort by key in this order : Titanium, Gold, Aluminium, Iron, Carbon
                    ResourceKind mostAbundantRessource = solidRessources.OrderByDescending(x => x.Value).ThenByDescending(x => x.Key, new RessourceComp()).First().Key;
                    planet.ResourceQuantity[mostAbundantRessource]--;
                    if (user.ResourcesQuantity.ContainsKey(mostAbundantRessource))
                    {
                        user.ResourcesQuantity[mostAbundantRessource] += 1;
                    }
                    else
                    {
                        user.ResourcesQuantity.Add(mostAbundantRessource, 1);
                    }
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

                await _systemClock.Delay(60000);
                if (planet.GetNumberOfLiquidRessourcesLeft() > 0)
                {
                    // Add one ressource "water" to user and reduce this ressource from planet
                    planet.ResourceQuantity[ResourceKind.Water] -= 1;
                    if (user.ResourcesQuantity.ContainsKey(ResourceKind.Water))
                    {
                        user.ResourcesQuantity[ResourceKind.Water] += 1;
                    }
                    else
                    {
                        user.ResourcesQuantity.Add(ResourceKind.Water, 1);
                    }
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
                await _systemClock.Delay(60000);
                if (planet.GetNumberOfGasRessourcesLeft() > 0)
                {
                    // Add one ressource "oxygen" to user from planet
                    planet.ResourceQuantity[ResourceKind.Oxygen] -= 1;
                    if (user.ResourcesQuantity.ContainsKey(ResourceKind.Oxygen))
                    {
                        user.ResourcesQuantity[ResourceKind.Oxygen] += 1;
                    }
                    else
                    {
                        user.ResourcesQuantity.Add(ResourceKind.Oxygen, 1);
                    }
                }

            }
        }

        [HttpGet("{userId}/[controller]")]
        public ActionResult<List<BuildingJson>> GetBuildings(string userId)
        {
            User? user = _users.FirstOrDefault(x => x.Id == userId);
            if (user == null) return NotFound();

            return user.Buildings.Select(b=>new BuildingJson(b)).ToList();
        }

        [HttpGet("{userId}/[controller]/{buildingId}")]
        public async Task<ActionResult<BuildingJson>> GetBuilding(string userId, string buildingId)
        {
            User? user = _users.FirstOrDefault(x => x.Id == userId);
            if (user == null) return NotFound();
            Building? building = user.Buildings.FirstOrDefault(x => x.Id == buildingId && x.Type == "mine");
            if (building == null) return NotFound();
            if (building.EstimatedBuildTime == null)
            {
                return new BuildingJson(building);
            }
            else if (_systemClock.Now.AddSeconds(2) >= building.EstimatedBuildTime.Value)
            {
                try
                {
                    await building.BuildingTask;
                }
                catch (OperationCanceledException)
                {
                    return NotFound("Building creation was cancelled");
                }
            }
            return new BuildingJson(building);
        }
    }
}
