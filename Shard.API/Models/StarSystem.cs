using Shard.Shared.Core;

namespace Shard.API.Models
{
    public class StarSystem
    {
        public string Name { get; set; }
        public IReadOnlyList<Planet> Planets { get; set; }

        public StarSystem(string parName) 
        {
            Name = parName;
        }
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
        public Planet GetOneRandomPlanet()
        {
            Random random = new Random();
            int randIndex = random.Next(Planets.Count);
            return Planets[randIndex];
        }

        public Planet? GetPlanetByName(string planetName)
        {
            Planet? planet = Planets.FirstOrDefault(p => p.Name == planetName);
            return planet;
        }
    }
}
