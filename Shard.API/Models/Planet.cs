using Shard.Shared.Core;
using System;

namespace Shard.API.Models
{
    public class Planet
    {
        public string Name { get; set; }
        public int Size { get; set; }

        public Dictionary<ResourceKind, int> ResourceQuantity { get; set; }

        public List<Building> Buildings { get; set; }

        internal Planet(Random random)
        {
            Name = random.NextGuid().ToString();

            Size = 1 + random.Next(999);
            ResourceQuantity = (Dictionary<ResourceKind, int>?)new RandomShareComputer(random).GenerateResources(Size);
            Buildings = new List<Building>();
        }
        public Planet(string parName, int parSize, Dictionary<ResourceKind, int> parRessources, List<Building> parBuildings)
        {
            Name = parName;
            Size = parSize;
            ResourceQuantity = parRessources;
            Buildings = parBuildings;
        }

        public int GetNumberOfSolidRessourcesLeft()
        {
            int numberOfSolidRessourcesLeft = 0;
            // Iterate through this.ResourceQuantity if key equals "aluminium", "carbon", "gold", "iron", "titanium" add value to numberOfSolidRessourcesLeft
            foreach (var resource in this.ResourceQuantity)
            {
                if (resource.Key.Equals("aluminium") || resource.Key.Equals("carbon") || resource.Key.Equals("gold") || resource.Key.Equals("iron") || resource.Key.Equals("titanium"))
                {
                    numberOfSolidRessourcesLeft += resource.Value;
                }
            }
            return numberOfSolidRessourcesLeft;
        }

        public int GetNumberOfLiquidRessourcesLeft()
        {
            int numberOfLiquidRessourcesLeft = 0;
            // Iterate through this.ResourceQuantity if key equals "water" add value to numberOfLiquidRessourcesLeft
            foreach (var resource in this.ResourceQuantity)
            {
                if (resource.Key.Equals("Water"))
                {
                    numberOfLiquidRessourcesLeft += resource.Value;
                }

            }
            return numberOfLiquidRessourcesLeft;

        }

        public int GetNumberOfGasRessourcesLeft()
        {

            int numberOfGasRessourcesLeft = 0;
            // Iterate through this.ResourceQuantity if key equals "oxygen" add value to numberOfGasRessourcesLeft
            foreach (var resource in this.ResourceQuantity)
            {
                if (resource.Key.Equals("oxygen"))
                {
                    numberOfGasRessourcesLeft += resource.Value;
                }
            }
            return numberOfGasRessourcesLeft;

        }








    }
}