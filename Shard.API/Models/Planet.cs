﻿using Shard.Shared.Core;
using System;

namespace Shard.API.Models
{
    public class Planet
    {
        public string Name { get; set; }
        public int Size { get; set; }

        public IReadOnlyDictionary<ResourceKind, int> ResourceQuantity { get; set; }

        internal Planet(Random random)
        {
            Name = random.NextGuid().ToString();

            Size = 1 + random.Next(999);
            ResourceQuantity = new RandomShareComputer(random).GenerateResources(Size);
        }
        public Planet(string parName, int parSize, IReadOnlyDictionary<ResourceKind, int> parRessources )
        {
            Name = parName;
            Size = parSize;
            ResourceQuantity = parRessources;
            
        }
    }
}