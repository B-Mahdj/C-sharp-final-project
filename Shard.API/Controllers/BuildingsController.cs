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
                await Task.Delay(60000);
                if (planet.GetNumberOfSolidRessourcesLeft() > 0)
                {
                    // Pick first the most abundant resource on the planet

                    //Default values
                    ResourceKind mostAbundantRessources = ResourceKind.Carbon;
                    int mostAbundantRessourcesQuantity = 0;
                    foreach (var resource in planet.ResourceQuantity)
                    {
                        if (resource.Key.Equals(ResourceKind.Carbon) || resource.Key.Equals(ResourceKind.Aluminium) || resource.Key.Equals(ResourceKind.Gold) || resource.Key.Equals(ResourceKind.Iron) || resource.Key.Equals(ResourceKind.Titanium))
                        {
                            if (resource.Value > mostAbundantRessourcesQuantity)
                            {
                                mostAbundantRessources = resource.Key;
                                mostAbundantRessourcesQuantity = resource.Value;
                            }
                            else if (resource.Value == mostAbundantRessourcesQuantity)
                            {
                                // If 2 resources are equally abundant, pick the one in that order : Titanium, Gold, Aluminium, Iron, Carbon
                                
                                
                            }
                        }
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
                await Task.Delay(60000);
                if (planet.GetNumberOfLiquidRessourcesLeft() > 0)
                {
                    // Add one ressource "water" to user and reduce this ressource from planet
                    planet.ResourceQuantity[ResourceKind.Water] -= 1;
                    if (user.ResourcesQuantity.ContainsKey("Water"))
                    {
                        user.ResourcesQuantity["Water"] += 1;
                    }
                    else
                    {
                        user.ResourcesQuantity.Add("Water", 1);
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
                if (planet.GetNumberOfGasRessourcesLeft() > 0)
                {
                    // Add one ressource "oxygen" to user from planet
                    planet.ResourceQuantity[ResourceKind.Oxygen] -= 1;
                    if (user.ResourcesQuantity.ContainsKey("Oxygen"))
                    {
                        user.ResourcesQuantity["Oxygen"] += 1;
                    }
                    else
                    {
                        user.ResourcesQuantity.Add("Oxygen", 1);
                    }
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
