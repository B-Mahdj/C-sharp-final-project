using Shard.Shared.Core;

namespace Shard.API.Models
{
    public class Planet
    {
        public string Name { get; set; }
        public int Size { get; set; }

        internal Planet(Random random)
        {
            Name = random.NextGuid().ToString();

            Size = 1 + random.Next(999);
        }
        public Planet(string parName, int parSize)
        {
            Name = parName;
            Size = parSize;
        }
    }
}