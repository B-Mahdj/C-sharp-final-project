using Shard.Shared.Core;

namespace Shard.API.Models
{
    public class StarSystem
    {
        public string Name { get; }
        public IReadOnlyList<Planet> Planets { get; }

        internal StarSystem(Random random)
        {
            Name = random.NextGuid().ToString();

            var planetCount = 1 + random.Next(9);
            Planets = Generate(planetCount, random);
        }

        private static List<Planet> Generate(int count, Random random)
        {
            return Enumerable.Range(1, count)
                .Select(_ => new Planet(random))
                .ToList();
        }
    }
}
