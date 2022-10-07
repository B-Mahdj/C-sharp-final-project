using Shard.Shared.Core;

namespace Shard.API.Models
{
    public class UnitLocation
    {
        public string System { get; set; }
        public string Planet { get; set; }
        public IReadOnlyDictionary<ResourceKind, int> ResourcesQuantity { get; set; }

        public UnitLocation(string system, string planet, IReadOnlyDictionary<ResourceKind, int> resourceQuantity)
        {
            this.System = system;
            this.Planet = planet;
            ResourcesQuantity = resourceQuantity;
        }
    }
}
