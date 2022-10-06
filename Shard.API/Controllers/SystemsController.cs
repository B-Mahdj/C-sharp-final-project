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
        [HttpGet]
        public ActionResult<List<StarSystem>> Get([FromServices] Sector sector)
        {

            return Ok(sector.Systems);
        }

        [HttpGet("{systemName}")]
        public ActionResult<StarSystem> GetSystem(string systemName, [FromServices] Sector sector)
        {
            StarSystem? starsystem;
            foreach (var system in sector.Systems)
            {
                if (system.Name == systemName) { starsystem = system; return Ok(starsystem); }

            }
            return BadRequest();
        }

        [HttpGet("{systemName}/planets")]
        public ActionResult<Planet> GetPlanets(string systemName, [FromServices] Sector sector)
        {
            List<Planet>? planetList;
            foreach (var system in sector.Systems)
            {
                if (system.Name == systemName)
                {
                    StarSystem starsystem = system;
                    planetList = starsystem.Planets.ToList();
                    return Ok(planetList);
                }

            }
            return BadRequest();
        }

        [HttpGet("{systemName}/planets/{planetName}")]
        public ActionResult<Planet> GetPlanet(string systemName, string planetName, [FromServices] Sector sector)
        {
            Planet? planet;
            foreach (var system in sector.Systems)
            {
                if (system.Name == systemName)
                {
                    StarSystem starsystem = system;
                    foreach (var p in starsystem.Planets)
                    {
                        if (p.Name == planetName)
                        {
                            return Ok(planet = p);
                        }
                    }
                }
            }
            return BadRequest();
        }
    }
}
