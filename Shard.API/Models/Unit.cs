using Shard.Shared.Core;
using System.Linq;
using System.Text.Json.Serialization;

namespace Shard.API.Models
{
    public class Unit
    {
        public string? Id { get; set; }
        public string Type { get; set; }
        public string? System { get; set; }
        public string? Planet { get; set; }
        public string? DestinationSystem { get; set; }
        public string? DestinationPlanet { get; set; }
        public string? DestinationShard { get; set; }
        public string? EstimatedTimeOfArrival { get; set; }
        [JsonIgnore]
        public Task? MovingTask { get; set; }
        public IClock? SystemClock { get; set; }
        public int Damage { get; set; }
        public int ReloadTime { get; set; }
        public int Health { get; set; }
        public Dictionary<ResourceKind, int>? ResourcesQuantity { get; set; }



        [JsonConstructor]
        public Unit(string id, string type, string system, string planet)
        {
            this.Id = id;
            this.Type = type;
            this.System = system;
            this.Planet = planet;
        }

        public Unit(string id, string type, string system, string planet, int health,
                    Dictionary<ResourceKind, int> resourcesQuantity, IClock systemClock, List<User> users)
        {
            this.Id = id;
            this.Type = type;
            this.System = system;
            this.Planet = planet;
            this.DestinationPlanet = planet;
            this.DestinationSystem = system;
            this.Health = health;
            this.ResourcesQuantity = resourcesQuantity;
   
            switch (Type)
            {
                case ("fighter"):
                    Damage = 10;
                    ReloadTime = 6;
                    Health = 80;
                    Fighting(this, users, systemClock);
                    break;
                case ("bomber"):
                    Damage = 400;
                    ReloadTime = 60;
                    Health = 50;
                    Fighting(this, users, systemClock);
                    break;
                case ("cruiser"):
                    Damage = 40;
                    ReloadTime = 6;
                    Health = 400;
                    Fighting(this, users, systemClock);
                    break;
            }
        }
        public Unit(string id, string type, string system, string planet,
                    IClock systemClock, List<User> users, Dictionary<ResourceKind, int> resourcesQuantity)
        {
            this.Id = id;
            this.Type = type;
            this.System = system;
            this.DestinationSystem = system;
            this.DestinationPlanet = planet;
            this.SystemClock = systemClock;
            this.ResourcesQuantity = new Dictionary<ResourceKind, int>();
            foreach (ResourceKind resource in Enum.GetValues(typeof(ResourceKind)))
            {
               this.ResourcesQuantity.Add(resource, 0);
            }
            
            if(resourcesQuantity != null)
                this.ResourcesQuantity = resourcesQuantity;
            switch (Type)
            {
                case ("fighter"):
                    Damage = 10;
                    ReloadTime = 6;
                    Health = 80;
                    Fighting(this, users, systemClock);
                    break;
                case ("bomber"):
                    Damage = 400;
                    ReloadTime = 60;
                    Health = 50;
                    Fighting(this, users, systemClock);
                    break;
                case ("cruiser"):
                    Damage = 40;
                    ReloadTime = 6;
                    Health = 400;
                    Fighting(this, users, systemClock);
                    break;
                case ("cargo"):
                    if (Health == 0)
                        Health = 100;
                    break;
            }
        }
        
        private void Fighting(Unit unit, List<User> users, IClock clock)
        {
            clock.CreateTimer(
                _ =>
                {
                        List<Unit> unitsInSamePlace = GetAllUnitsInSamePlace(unit, users);
                        if (unitsInSamePlace.Count > 0 && clock.Now.Second % unit.ReloadTime == 0)
                        {
                            String[] priorities = GetPriorities(unit);
                            for (int i = 0; i < priorities.Length; ++i)
                            {
                                Unit unitToHit = unitsInSamePlace.FirstOrDefault(unit => unit.Type == priorities[i]);
                                if (unitToHit != null)
                                {
                                    if (unit.Type == "cruiser" && unitToHit.Type == "bomber")
                                    {
                                        unitToHit.Health -= Damage / 10;
                                        break;
                                    }
                                    unitToHit.Health -= Damage;
                                    break;
                                }
                            }
                        }
                },
                null,
                dueTime: 1000,
                period : 1000);
            
        }

        private List<Unit> GetAllUnitsInSamePlace(Unit unit, List<User> users)
        {
            {
                List<Unit> unitsInSamePlace = new List<Unit>();
                foreach (User user in users)
                {
                    foreach (Unit otherUnit in user.Units)
                    {
                        // If unit and other unit have both system taht are equals and planet is null 
                        // or if unit and other unit have both system and planet that are equals
                        if ((unit.System == otherUnit.System && unit.Planet == null && otherUnit.Planet == null) ||
                            (unit.System == otherUnit.System && unit.Planet == otherUnit.Planet))
                        {
                            // Check that otherUnit is not owned by the user that own unit 
                            if (unit.Id != otherUnit.Id)
                            {
                                unitsInSamePlace.Add(otherUnit);
                            }
                        }
                    }
                }
                return unitsInSamePlace;
            }
        }

        private String[] GetPriorities(Unit unit)
        {
            switch (unit.Type)
            {
                case ("fighter"):
                    return Enum.GetNames(typeof(UnitFightingPriority.FighterPriority))
                        .Select(x => x.ToLower())
                        .ToArray();
                case ("bomber"):
                    return Enum.GetNames(typeof(UnitFightingPriority.BomberPriority))
                        .Select(x => x.ToLower())
                        .ToArray();
                case ("cruiser"):
                    return Enum.GetNames(typeof(UnitFightingPriority.CruiserPriority))
                        .Select(x => x.ToLower())
                        .ToArray();
                default:
                    return Array.Empty<String>();
            }
        }
    }
}
