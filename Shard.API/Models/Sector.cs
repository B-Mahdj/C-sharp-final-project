using Newtonsoft.Json;
using Shard.Shared.Core;
using System.Text.Json.Serialization;

namespace Shard.API.Models
{
    public class Sector
    {
        public List<StarSystem> Systems { get; set; }

        public List<StarSystem> Generate(MapGenerator map)
        {
            SectorSpecification previousSector = map.Generate();
            //return previousSector;
            Sector finalSector = (Sector)previousSector;
            Systems = finalSector.Systems.ToList();
            return Systems;
        }

        public static explicit operator Sector(SectorSpecification sector)
      {
        return JsonConvert.DeserializeObject<Sector>(JsonConvert.SerializeObject(sector))!;
      }
    }
}
