using Shard.Shared.Core;
using System.Text.Json.Serialization;

namespace Shard.API.Models
{
    public class PlanetJson
    {
        public string Name { get; set; }
        public int Size { get; set; }
        [JsonIgnore]
        public IReadOnlyDictionary<ResourceKind, int> ResourceQuantity { get; set; }

        [JsonIgnore]
        public List<BuildingJson>? Buildings { get; set; } 

        internal PlanetJson(Random random)
        {
            Name = random.NextGuid().ToString();

            Size = 1 + random.Next(999);
            ResourceQuantity = new RandomShareComputer(random).GenerateResources(Size);
        }
        public PlanetJson(Planet planet)
        {
            Name = planet.Name;
            Size = planet.Size;
            ResourceQuantity = planet.ResourceQuantity;
            if(planet.Buildings != null)
            {
            this.Buildings = planet.Buildings.Select(x => new BuildingJson(x)).ToList();
            }
        }
    }
}
