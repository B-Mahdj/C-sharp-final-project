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
            StarSystem? starSystem = _sector.Systems.FirstOrDefault(system => system.Name == systemName);
            if (starSystem != null) {
                return new StarSystemJson(starSystem); 
            }
            return BadRequest();
        }

        [HttpGet("{systemName}/planets")]
        public ActionResult<List<PlanetJson>> GetPlanets(string systemName)
        {
            StarSystem? starSystem = _sector.Systems.FirstOrDefault(system => system.Name == systemName);
            if (starSystem != null) { 
                return starSystem.Planets.Select(p => new PlanetJson(p)).ToList(); 
            }
            return BadRequest();
        }

        [HttpGet("{systemName}/planets/{planetName}")]
        public ActionResult<PlanetJson> GetPlanet(string systemName, string planetName)
        {
            Planet? planet = _sector.Systems.FirstOrDefault(system => system.Name == systemName).
                             Planets.FirstOrDefault(planet => planet.Name == planetName);
            if (planet != null)
            {
                return new PlanetJson(planet);
            }
            return BadRequest();
        }
    }
}
