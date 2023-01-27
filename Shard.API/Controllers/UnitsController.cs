using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Shard.API.Models;
using Shard.Shared.Core;
using System.Collections.Immutable;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Shard.API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly List<User> _users;
        private readonly Sector _sector;
        private readonly IClock _systemClock;
        private readonly IHttpClientFactory _clientFactory;

        public UnitsController(List<User> list, Sector sector, IClock systemClock, IHttpClientFactory clientFactory)
        {
            _users = list;
            _sector = sector;
            _systemClock = systemClock;
            _clientFactory = clientFactory;
        }

        [HttpGet("{id}/[controller]")]
        public ActionResult<List<UnitJson>> GetUserUnits(string id)
        {
            User? user = _users.FirstOrDefault(user => user.Id == id);
            if (user != null)
            {
                return user.Units.Select(unit => new UnitJson(unit)).ToList();
            }
            return NotFound();
        }

        [HttpGet("{id}/[controller]/{unitId}")]
        public async Task<ActionResult<UnitJson>> GetUserUnit(string id, string unitId)
        {
            User? user = _users.FirstOrDefault(user => user.Id == id);
            Unit? unit = user.Units.FirstOrDefault(unit => unit.Id == unitId);
            if (unit == null)
            {
                return NotFound();
            }
            if (unit.Health <= 0 && unit.Damage > 0)
            {
                user.Units.Remove(unit);
                return NotFound();
            }
            if (unit.EstimatedTimeOfArrival != null && unit.EstimatedTimeOfArrival != ""
                && _systemClock.Now.AddSeconds(2) >= DateTime.Parse(unit.EstimatedTimeOfArrival))
            {
                // Wait for end of task
                await unit.MovingTask;
                unit.EstimatedTimeOfArrival = "";
            }
            return new UnitJson(unit);
        }
        
        [HttpPut("{id}/[controller]/{unitId}")]
        public async Task<ActionResult<UnitJson>> MoveUnits(string id, string unitId, Unit newUnit)
        {
            User? user = _users.FirstOrDefault(x => x.Id == id);
            if (user == null) { return NotFound(); }
            Unit unit = user.Units.FirstOrDefault(x => x.Id == unitId);
            
            if (newUnit.DestinationShard != null)
            {
                var serializeOptions = new JsonSerializerOptions {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
                };
                
                var userToSend = new UserJson(user);
                var baseUri = GetServerBaseUri();
                var putUserUri = baseUri + "users/" + user.Id; 
                var putUnitUri = baseUri + "users/" + user.Id + "/units/" + unitId;
                
                HttpClient client = _clientFactory.CreateClient();
                await client.PutAsJsonAsync(putUserUri, userToSend);
                HttpResponseMessage response = await client.PutAsJsonAsync(putUnitUri, newUnit, serializeOptions);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectPermanentPreserveMethod(putUnitUri);
                }
                else
                {
                    return StatusCode(502);
                }
            }
            
            if (unit == null)
            {
                bool IsAuthorized = Request.Headers.TryGetValue("Authorization", out StringValues headerValues);
                if (IsAuthorized || newUnit.Type == "cargo")
                {
                    Unit createdUnit = new(unitId, newUnit.Type, newUnit.System, newUnit.Planet, newUnit.Health, newUnit.ResourcesQuantity, _systemClock, _users) ;
                    if(newUnit.Type == "cargo")
                    {
                        createdUnit.System = GetServerSystem();
                    }
                    else
                    {
                    createdUnit.DestinationSystem = newUnit.System;
                    createdUnit.DestinationPlanet = newUnit.Planet;
                    }
                    user.Units.Add(createdUnit);
                    return new UnitJson(createdUnit);
                }
                return Unauthorized();
            }

            if (newUnit.Id != unitId)
                return BadRequest();
            unit.DestinationPlanet = newUnit.DestinationPlanet;
            unit.DestinationSystem = newUnit.DestinationSystem;
            
            if (newUnit.DestinationPlanet != unit.Planet || newUnit.DestinationSystem != unit.System)
            {
                CancelBuild(user, unit.Id);
            }
            if (newUnit.DestinationPlanet == null && newUnit.DestinationSystem != null && unit.System != newUnit.DestinationSystem)
            {
                CancelBuild(user, unit.Id);
                unit.MovingTask = MoveUnitToNewSystem(unit, newUnit.DestinationSystem);
                unit.EstimatedTimeOfArrival = _systemClock.Now.AddSeconds(60).ToString();
            }
            else if (newUnit.DestinationPlanet != null && unit.Planet != newUnit.DestinationPlanet && (newUnit.DestinationSystem == null || unit.System == newUnit.DestinationSystem))
            {
                CancelBuild(user, unit.Id);
                unit.MovingTask = MoveUnitToNewPlanet(unit, newUnit.DestinationPlanet);
                unit.EstimatedTimeOfArrival = _systemClock.Now.AddSeconds(15).ToString();
            }
            else if (newUnit.DestinationPlanet != null && newUnit.DestinationSystem != null
                && unit.System != newUnit.DestinationSystem && unit.Planet != newUnit.DestinationPlanet)
            {
                CancelBuild(user, unit.Id);
                unit.MovingTask = MoveUnitToNewSystemAndPlanet(unit, newUnit.DestinationSystem, newUnit.DestinationPlanet);
                unit.EstimatedTimeOfArrival = _systemClock.Now.AddSeconds(75).ToString();
            }

            InitResources(newUnit);

            if (ResourcesAreDifferent(unit, newUnit))
            {
                if (unit.Type == "builder")
                {
                    return BadRequest("Cannot load resources into a builder");
                }

                // Put ressources quantity in cargo 
                if (unit.Type == "cargo")
                {
                    if (NoStarport(newUnit.DestinationPlanet))
                    {
                        return BadRequest("Cannot load resources without a starport");
                    }

                    foreach (ResourceKind resource in Enum.GetValues(typeof(ResourceKind)))
                    {
                        int quantityToMove = unit.ResourcesQuantity[resource] - newUnit.ResourcesQuantity[resource];
                        if (user.ResourcesQuantity[resource] + quantityToMove < 0)
                        {
                            return BadRequest("Cannot load more resources than you have");
                        }
                        user.ResourcesQuantity[resource] += quantityToMove;
                        unit.ResourcesQuantity[resource] = newUnit.ResourcesQuantity[resource];
                    }
                }
            }
            return new UnitJson(unit);
        }

        private async Task MoveUnitToNewPlanet(Unit unit, string destinationPlanet)
        {
            unit.Planet = null;
            await _systemClock.Delay(15000);
            unit.Planet = destinationPlanet;
            unit.DestinationPlanet = null;
        }

        private async Task MoveUnitToNewSystem(Unit unit, string destinationSystem)
        {
            await _systemClock.Delay(60000);
            unit.System = destinationSystem;
            unit.DestinationSystem = null;
        }

        private async Task MoveUnitToNewSystemAndPlanet(Unit unit, string destinationSystem, string destinationPlanet)
        {
            unit.Planet = null;
            await MoveUnitToNewSystem(unit, destinationSystem);
            await MoveUnitToNewPlanet(unit, destinationPlanet);
        }

        private void CancelBuild(User user, String Id)
        {
            user.Buildings.FindAll(building => (bool)!building.IsBuilt && building.BuilderId == Id).ForEach(building =>
            {
                try
                {
                    building.TokenSource.Cancel();
                }
                catch (AggregateException ae)
                {
                    Console.WriteLine(ae);
                }
                finally
                {
                    building.TokenSource.Dispose();
                    user.Buildings.Remove(building);
                }
            });

        }
        private void InitResources(Unit unit)
        {
            foreach (ResourceKind resource in Enum.GetValues(typeof(ResourceKind)))
            {
                if (!unit.ResourcesQuantity.ContainsKey(resource))
                {
                    unit.ResourcesQuantity.Add(resource, 0);
                } 
            }
        }

        private bool ResourcesAreDifferent(Unit unit, Unit newUnit)
        {
            foreach (ResourceKind resource in Enum.GetValues(typeof(ResourceKind)))
            {
                if (unit.ResourcesQuantity[resource] != newUnit.ResourcesQuantity[resource])
                    return true;
            }
            return false;
        }

        private bool NoStarport(string destinationPlanet)
        {
            foreach (var user in _users)
            {
                foreach (var building in user.Buildings)
                {
                    if (building.Planet == destinationPlanet && building.Type == "starport")
                        return false;
                }
            }
            return true;
        }

        private static string GetServerBaseUri()
        {
            var settingsJson = new StreamReader("appsettings.json").ReadToEnd();
            var settings = (JObject)JsonConvert.DeserializeObject(settingsJson);
            return settings["Wormholes"]["server2"]["baseUri"].Value<string>();
        }
        private static string GetServerSystem()
        {
            var settingsJson = new StreamReader("appsettings.json").ReadToEnd();
            var settings = (JObject)JsonConvert.DeserializeObject(settingsJson);
            return settings["Wormholes"]["server2"]["system"].Value<string>();
        }

        [HttpGet("{id}/[controller]/{unitId}/location")]
        public ActionResult<UnitLocation> GetUnitLocation(string id, string unitId)
        {
            User? user = _users.FirstOrDefault(user => user.Id == id);
            Unit? unit = user.Units.FirstOrDefault(unit => unit.Id == unitId);
            if (unit == null)
            {
                return NotFound();
            }
            var system = _sector.Systems.First(system => system.Name == unit.System);
            var planet = system.Planets.First(planet => planet.Name == unit.Planet);
            var resourceQuantity = unit.Type.Equals("scout") ? planet.ResourceQuantity : null ;
                            
            return new UnitLocation(system.Name, planet.Name, resourceQuantity);
        }
    }
}
