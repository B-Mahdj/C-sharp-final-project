using Shard.Shared.Core;

namespace Shard.API.Controllers
{
    internal class RessourceComp : IComparer<ResourceKind>
    {
        // Note : We could optimzie this code or let it this way to easily change the way we compare resources
        public int Compare(ResourceKind x, ResourceKind y)
        {
            // Compare in that order : Titanium, Gold, Aluminium, Iron, Carbon
            // Biggest value : Titanium, lowest value : Carbon
            if (x == ResourceKind.Titanium && y == ResourceKind.Titanium) return 0;
            if (x == ResourceKind.Titanium && y == ResourceKind.Gold) return 1;
            if (x == ResourceKind.Titanium && y == ResourceKind.Aluminium) return 1;
            if (x == ResourceKind.Titanium && y == ResourceKind.Iron) return 1;
            if (x == ResourceKind.Titanium && y == ResourceKind.Carbon) return 1;
            if (x == ResourceKind.Gold && y == ResourceKind.Titanium) return -1;
            if (x == ResourceKind.Gold && y == ResourceKind.Gold) return 0;
            if (x == ResourceKind.Gold && y == ResourceKind.Aluminium) return 1;
            if (x == ResourceKind.Gold && y == ResourceKind.Iron) return 1;
            if (x == ResourceKind.Gold && y == ResourceKind.Carbon) return 1;
            if (x == ResourceKind.Aluminium && y == ResourceKind.Titanium) return -1;
            if (x == ResourceKind.Aluminium && y == ResourceKind.Gold) return -1;
            if (x == ResourceKind.Aluminium && y == ResourceKind.Aluminium) return 0;
            if (x == ResourceKind.Aluminium && y == ResourceKind.Iron) return 1;
            if (x == ResourceKind.Aluminium && y == ResourceKind.Carbon) return 1;
            if (x == ResourceKind.Iron && y == ResourceKind.Titanium) return -1;
            if (x == ResourceKind.Iron && y == ResourceKind.Gold) return -1;
            if (x == ResourceKind.Iron && y == ResourceKind.Aluminium) return -1;
            if (x == ResourceKind.Iron && y == ResourceKind.Iron) return 0;
            if (x == ResourceKind.Iron && y == ResourceKind.Carbon) return 1;
            if (x == ResourceKind.Carbon && y == ResourceKind.Titanium) return -1;
            if (x == ResourceKind.Carbon && y == ResourceKind.Gold) return -1;
            if (x == ResourceKind.Carbon && y == ResourceKind.Aluminium) return -1;
            if (x == ResourceKind.Carbon && y == ResourceKind.Iron) return -1;
            if (x == ResourceKind.Carbon && y == ResourceKind.Carbon) return 0;
            return 0;
        }
    }
}