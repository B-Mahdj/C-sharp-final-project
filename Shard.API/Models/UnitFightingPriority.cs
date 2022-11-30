namespace Shard.API.Models
{
    public class UnitFightingPriority
    {
        public enum FighterPriority
        {
            Bomber = 0,
            Fighter = 1,
            Cruiser = 2,
        }
        public enum BomberPriority
        {
            Cruiser = 0,
            Bomber = 1,
            Fighter = 2, 
        }
        public enum CruiserPriority
        {
            Fighter = 0,
            Cruiser = 1,
            Bomber = 2,
        }
    }
}
