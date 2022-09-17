using Shard.Shared.Core;

namespace Shard.API.Models
{
    public class Planet
    {
        public string Name { get; }
        public int Size { get; }

        internal Planet(Random random)
        {
            Name = random.NextGuid().ToString();

            Size = 1 + random.Next(999);
        }
    }
}