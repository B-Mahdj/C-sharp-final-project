using Shard.Shared.Core;

namespace Shard.API.Models
{
    public class StarSystemJson
    {
        public string Name { get; set; }
        public IReadOnlyList<PlanetJson>? Planets { get; set; }

        public StarSystemJson(StarSystem starSystem)
        {
           this.Name = starSystem.Name;
           this.Planets = starSystem.Planets.Select(p => new PlanetJson(p)).ToList();

        }
       
    }
}
