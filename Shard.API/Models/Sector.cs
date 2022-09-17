namespace Shard.API.Models
{
    public class Sector
    {
        public IReadOnlyList<StarSystem> Systems { get; }

        internal Sector(Random random)
        {
            Systems = Generate(10, random);
        }

        private static List<StarSystem> Generate(int count, Random random)
        {
            return Enumerable.Range(1, count)
                .Select(_ => new StarSystem(random))
                .ToList();
        }
    }
}
