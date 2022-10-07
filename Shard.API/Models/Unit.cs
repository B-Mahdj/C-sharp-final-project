using System.Text.Json.Serialization;

namespace Shard.API.Models
{
    public class Unit
    {
        public string Id { get; set; }
        public string type { get; set; }
        public string system { get; set; }
        public string planet { get; set; }

        [JsonConstructor]
        public Unit(string id, string type, string system, string? planet)
        {
            this.Id = id;
            this.type = type;
            this.system = system;
            if(planet != null)
            {
                this.planet = planet;
            }
            else { this.planet= "";}
            
        }
    }
}
