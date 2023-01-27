using Newtonsoft.Json;
using Shard.Shared.Core;

namespace Shard.API.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Pseudo { get; set; }
        public DateTime DateOfCreation { get; } = DateTime.Now;
        [JsonIgnore]
        public List<Unit> Units { get; } = new List<Unit>();
        [JsonIgnore]
        public List<Building> Buildings { get; } = new List<Building>();
        public Dictionary<ResourceKind, int> ResourcesQuantity { get; set; }
        
        public User(string id, string pseudo, DateTime dateOfCreation)
        {
            this.Id = id;
            this.Pseudo = pseudo;
            this.DateOfCreation = dateOfCreation;
            this.ResourcesQuantity = new Dictionary<ResourceKind, int>()
                {
                    {ResourceKind.Aluminium,0},
                    {ResourceKind.Carbon,20 },
                    {ResourceKind.Gold,0 },
                    {ResourceKind.Iron,10 },
                    {ResourceKind.Oxygen,50 },
                    {ResourceKind.Titanium,0 },
                    {ResourceKind.Water,50 },
                };
        }
    }
}
