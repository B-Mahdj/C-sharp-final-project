namespace Shard.API.Models
{
    public class Unit
    {
        public string Id = Guid.NewGuid().ToString();
        public string type { get; set; }
        public string system { get; set; }
        public string planet { get; set; }

        public Unit(string type, string system, string planet)
        {
            this.type = type;
            this.system = system;
            this.planet = planet;
        }
    }
}
