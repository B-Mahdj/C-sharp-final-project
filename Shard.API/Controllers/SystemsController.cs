using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shard.API.Models;
using Shard.Shared.Core;
using System.Linq;

namespace Shard.API.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class SystemsController : ControllerBase
    {
        private readonly Sector _sector;
        public SystemsController(Sector sector)
        {
            _sector = sector;
        }

        [HttpGet]
        public ActionResult<List<StarSystemJson>> Get()
        {
            return _sector.Systems.Select(s => new StarSystemJson(s)).ToList();
        }

        [HttpGet("{systemName}")]
        public ActionResult<StarSystemJson> GetSystem(string systemName)
        {
            StarSystem? starsystem;
            foreach (var system in _sector.Systems)
            {
                if (system.Name == systemName) { starsystem = system; return new StarSystemJson(starsystem); }

            }
            return BadRequest();
        }

        [HttpGet("{systemName}/planets")]
        public ActionResult<List<PlanetJson>> GetPlanets(string systemName)
        {
           
            foreach (var system in _sector.Systems)
            {
                if (system.Name == systemName)
                {
                    StarSystem starSystem = system;
                    //planetList = starsystem.Planets.ToList();
                    //return planetList;
                    return starSystem.Planets.Select(p => new PlanetJson(p)).ToList(); 
                }

            }
            return BadRequest();
        }

        [HttpGet("{systemName}/planets/{planetName}")]
        public ActionResult<PlanetJson> GetPlanet(string systemName, string planetName)
        {
            Planet? planet;
            foreach (var system in _sector.Systems)
            {
                if (system.Name == systemName)
                {
                    StarSystem starsystem = system;
                    foreach (var p in starsystem.Planets)
                    {
                        if (p.Name == planetName)
                        {
                            return new PlanetJson(planet = p);
                        }
                    }
                }
            }
            return BadRequest();
        }
    }
}
