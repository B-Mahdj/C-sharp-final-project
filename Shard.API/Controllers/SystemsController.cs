using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shard.API.Models;
using System.Linq;

namespace Shard.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SystemsController : ControllerBase
    {
        private static List<StarSystem> array = null;
        [HttpGet]
        public IEnumerable<StarSystem> Get()
        {
            array = Enumerable.Range(1, 5).Select(index => new StarSystem(new Random())).ToList();
            return (IEnumerable<StarSystem>)array;
        }

        [HttpGet("{systemName}")]
        public StarSystem GetSystem(string systemName)
        {
            StarSystem? starsystem = null;
            foreach (var system in Get().Where(system => system.Name == systemName))
            {
                Console.WriteLine(system.Name);
                starsystem = system;
            }
            return starsystem;


        }
        [HttpGet("{systemName}/planets")]
        public List<Planet> GetPlanets(string systemName)
        {
            List<Planet> planetList = new List<Planet>();
            foreach (var system in Get().Where(system => system.Name == systemName))
            {
                StarSystem starsystem = system;
                planetList = starsystem.Planets.ToList();
               
            }
           return planetList;
        }

        [HttpGet("{systemName}/planets/{planetName}")]
        public Planet GetPlanet(string systemName, string planetName)
        {
            Planet? planet = null;
            StarSystem starsystem;
            foreach (var system in Get().Where(system => system.Name == systemName))
            {
                starsystem = system;
                foreach (var p in starsystem.Planets.Where(p => p.Name == planetName))
                {
                    planet = p;
                }
            }
            return planet;
        }

    }
}
