using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shard.API.Models;
using Shard.Shared.Core;
using System.Collections.Immutable;

namespace Shard.API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly List<User> _users;
        private readonly Sector _sector;
        private readonly IClock _systemClock;

        public UnitsController(List<User> list, Sector sector, IClock systemClock)
        {
            _users = list;
            _sector = sector;
            _systemClock = systemClock;
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
        public async Task<ActionResult<Unit>> GetUserUnit(string id, string unitId)
        {
            foreach (var user in _users)
            {
                if (user.Id == id)
                {
                    foreach (Unit unit in user.Units)
                    {
                        if (unit.Id == unitId)
                        {
                            if (unit.EstimatedTimeOfArrival == null || unit.EstimatedTimeOfArrival == "" )
                            {
                                return unit;
                            }
                            /*
                            else if (getSecondsBetweenTwoDateTime(DateTime.Parse(unit.EstimatedTimeOfArrival), _systemClock.Now.AddSeconds(2)) <= 0)
                            {
                                unit.EstimatedTimeOfArrival = "";
                                return unit;
                            }
                            */
                            else if (_systemClock.Now.AddSeconds(2) >= DateTime.Parse(unit.EstimatedTimeOfArrival)) 
                            {
                                // Wait for end of task
                                await unit._movingTask;
                                unit.EstimatedTimeOfArrival = "";
                                return unit;
                            }
                            else
                            {
                                return unit;
                            }
                        }
                    }
                }
            }
            return NotFound();

        }
        
        private Double getSecondsBetweenTwoDateTime(DateTime d1, DateTime d2)
        {
            return (d1 - d2).TotalSeconds;
        }

        [HttpPut("{id}/[controller]/{unitId}")]
        public ActionResult<Unit> moveUnits(string id, string unitId, Unit newUnit)
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

                            if (newUnit.DestinationPlanet != null)
                            {
                                unit.DestinationPlanet = newUnit.DestinationPlanet;
                            }
                            unit.DestinationSystem = newUnit.DestinationSystem;

                            if (newUnit.DestinationPlanet == null)
                            {
                                unit._movingTask = MoveUnitToNewSystem(unit, newUnit.DestinationSystem);
                                unit.EstimatedTimeOfArrival = _systemClock.Now.AddSeconds(60).ToString();
                            }
                            else
                            {
                                unit._movingTask = MoveUnitToNewPlanet(unit, newUnit.DestinationSystem, newUnit.DestinationPlanet);
                                unit.EstimatedTimeOfArrival = _systemClock.Now.AddSeconds(90).ToString();
                            }
                            
                            return unit;
                        }
                    }
                }
            }
            return NotFound();
        }

        /*
        private void MoveUnitToNewSystem(Unit unit, string destinationSystem)
        {
            var timer = _systemClock.CreateTimer(_ =>
                { 
                    unit.DestinationSystem = destinationSystem;
                    unit.DestinationPlanet = null;
                },
                null,
                dueTime:60000,
                period: 0
            );
        }

        private void MoveUnitToNewPlanet(Unit unit, string destinationSystem, string destinationPlanet)
        {
            _systemClock.Delay(15000);
            unit.Planet = null;
            MoveUnitToNewSystem(unit, destinationSystem);
            _systemClock.Delay(15000);
            unit.Planet = destinationPlanet;
            unit.DestinationPlanet = null;
        }
        */

        
        private async Task MoveUnitToNewSystem(Unit unit, string destinationSystem)
        {
            await _systemClock.Delay(60000);
            unit.System = destinationSystem;
            unit.DestinationSystem = "";
        }

        private async Task MoveUnitToNewPlanet(Unit unit, string destinationSystem, string destinationPlanet)
        {
            unit.Planet = null;
            await MoveUnitToNewSystem(unit, destinationSystem);
            await _systemClock.Delay(15000);
            unit.Planet = destinationPlanet;
            unit.DestinationPlanet = null;
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
                                         select s).First();

                            var planet = (from p in system.Planets
                                         where p.Name == unit.Planet
                                         select p).First();
                            return new UnitLocation(system.Name, planet.Name, planet.ResourceQuantity);
                        }
                    }
                }
            }
            return NotFound();
        }

    }
}
