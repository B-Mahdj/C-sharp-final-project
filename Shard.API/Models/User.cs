using Shard.Shared.Core;

namespace Shard.API.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Pseudo { get; set; }
        public string DateOfCreation { get; } = DateTime.Now.ToString();

        public List<Unit> Units { get; } = new List<Unit>();
        public List<Building> Buildings { get; } = new List<Building>();
        public IReadOnlyDictionary<string, int> ResourcesQuantity { get; }

        public User(string id, string pseudo)
        {
            this.Id = id;
            this.Pseudo = pseudo;
            this.ResourcesQuantity = new Dictionary<string, int>()
            {
                {"aluminium",0 },
                {"carbon",20 },
                {"gold",0 },
                {"iron",10 },
                {"oxygen",50 },
                {"titanium",0 },
                {"water",50 },
            };
        }
    }
}
