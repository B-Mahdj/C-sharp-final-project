using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
        public ActionResult<BuildingJson> AddBuilding(string userId, Building building)
        {
            User? user = _users.FirstOrDefault(x => x.Id == userId);
            if (user == null) return NotFound("User not found");
            Unit? unit = user.Units.FirstOrDefault(x => x.Id.Equals(building.BuilderId)) ;
            
            if (unit == null || building == null || building.Type != "mine" && building.Type!="starport" || building.BuilderId != unit.Id || unit.Planet==null)
            {
                return BadRequest();
            }
            if (building.ResourceCategory != "solid" && building.ResourceCategory != "liquid" && building.ResourceCategory != "gaseous" && building.Type=="mine")
            {
                return BadRequest();
            }
            building.Id = Guid.NewGuid().ToString();
            building.System = unit.System;
            building.Planet = unit.Planet;
            building.IsBuilt = false;
            if (building.Type == "starport")
            {
                building.ResourceCategory = null;
            }
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
            if (building.Type == "mine")
                MineRessources(building, user);
        }

        private async Task MineRessources(Building building, User user)
        {
            switch (building.ResourceCategory)
            {
                case "solid":
                    MineSolid(building, user);
                    break;
                case "liquid":
                    MineLiquid(building, user);
                    break;
                case "gaseous":
                    MineGas(building, user);
                    break;
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
        public Boolean CheckResourcesForQueue(User user, ResourceKind ressourceKind, int value)
        {
            if (user.ResourcesQuantity[ressourceKind] < value)
            {
                return false;
            }
            return true;
        }

        [HttpPost("/users/{userId}/[controller]/{starportId}/queue")]
        public ActionResult<UnitJson> CreateUnit(string userId, string starportId, Unit unit) {
            var user = _users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                return NotFound("user not found");
            }
            var starport = user.Buildings.FirstOrDefault(x => (x.Id == starportId));
            if(starport == null)
            {
                return NotFound("Starport Not found");
            }
            else if (starport.Type != "starport" || starport.IsBuilt==false)
            {
                return BadRequest();
            }
            Unit newUnit = new(Guid.NewGuid().ToString(), unit.Type, starport.System,starport.Planet, _systemClock, _users, null);
            switch (newUnit.Type)
            {
                case "scout":
                    if(this.CheckResourcesForQueue(user,ResourceKind.Carbon,5)&& this.CheckResourcesForQueue(user, ResourceKind.Iron, 5))
                    {
                        user.ResourcesQuantity[ResourceKind.Iron]-=5;
                        user.ResourcesQuantity[ResourceKind.Carbon] -= 5;
                        break;
                    }
                    else
                    {
                        return BadRequest("Not enough resources");
                    }
                case "builder":
                    if (this.CheckResourcesForQueue(user, ResourceKind.Carbon, 5) && this.CheckResourcesForQueue(user, ResourceKind.Iron, 10))
                    {
                        user.ResourcesQuantity[ResourceKind.Iron] -= 10;
                        user.ResourcesQuantity[ResourceKind.Carbon] -= 5;
                        break;
                    }
                    else
                    {
                        return BadRequest("Not enough resources");
                    }
                case "fighter":
                    if (this.CheckResourcesForQueue(user, ResourceKind.Aluminium, 10) && this.CheckResourcesForQueue(user, ResourceKind.Iron, 20))
                    {
                        user.ResourcesQuantity[ResourceKind.Iron] -= 20;
                        user.ResourcesQuantity[ResourceKind.Aluminium] -= 10;
                        break;
                    }
                    else
                    {
                        return BadRequest("Not enough resources");
                    }
                case "bomber":
                    if (this.CheckResourcesForQueue(user, ResourceKind.Titanium, 10) && this.CheckResourcesForQueue(user, ResourceKind.Iron, 30))
                    {
                        user.ResourcesQuantity[ResourceKind.Iron] -= 30;
                        user.ResourcesQuantity[ResourceKind.Titanium] -= 10;
                        break;
                    }
                    else
                    {
                        return BadRequest("Not enough resources");
                    }
                case "cruiser":
                    if (this.CheckResourcesForQueue(user, ResourceKind.Gold, 20) && this.CheckResourcesForQueue(user, ResourceKind.Iron, 60))
                    {
                        user.ResourcesQuantity[ResourceKind.Iron] -= 60;
                        user.ResourcesQuantity[ResourceKind.Gold] -= 20;
                        break;
                    }
                    else
                    {
                        return BadRequest("Not enough resources");
                    }
                case "cargo":
                    if (this.CheckResourcesForQueue(user, ResourceKind.Carbon, 10) && this.CheckResourcesForQueue(user, ResourceKind.Iron, 10)
                        && this.CheckResourcesForQueue(user, ResourceKind.Gold, 5))
                    {
                        user.ResourcesQuantity[ResourceKind.Carbon] -= 10;
                        user.ResourcesQuantity[ResourceKind.Iron] -= 10;
                        user.ResourcesQuantity[ResourceKind.Gold] -= 5;
                        break;
                    }
                    else
                    {
                        return BadRequest("Not enough resources");
                    }
            }
            user.Units.Add(newUnit);
            return new UnitJson(newUnit);
        
        }

        [HttpGet("{userId}/[controller]")]
        public ActionResult<List<BuildingJson>> GetBuildings(string userId)
        {
            User? user = _users.FirstOrDefault(x => x.Id == userId);
            if (user == null) return NotFound("User not Found");

            return user.Buildings.Select(b=>new BuildingJson(b)).ToList();
        }

        [HttpGet("{userId}/[controller]/{buildingId}")]
        public async Task<ActionResult<BuildingJson>> GetBuilding(string userId, string buildingId)
        {
            User? user = _users.FirstOrDefault(x => x.Id == userId);
            Unit? unit = user?.Units.FirstOrDefault(x => x.Type == "builder");
            if (user == null) return NotFound("User not Found");
            Building? building = user.Buildings.FirstOrDefault(x => x.Id == buildingId && (x.Type == "mine"||x.Type=="starport"));
            if (building == null) 
                return NotFound("bulding not found hihi");
            if (building.EstimatedBuildTime != null && _systemClock.Now.AddSeconds(2) >= building.EstimatedBuildTime.Value)
            {
                while ((bool)!building.IsBuilt)
                {
                    if (building.TokenSource.IsCancellationRequested)
                    {
                        return NotFound("building not found");
                    }
                }
                await building.BuildingTask;
            }
           
            return new BuildingJson(building);
        }

       
    }
}
