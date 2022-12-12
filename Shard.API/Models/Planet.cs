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
            Dictionary<ResourceKind, int> solidRessources = this.GetListOfSolidRessources();
            foreach (var resource in solidRessources)
            {
                numberOfSolidRessourcesLeft += resource.Value;
            }
            return numberOfSolidRessourcesLeft;
        }

        public int GetNumberOfLiquidRessourcesLeft()
        {
            int numberOfLiquidRessourcesLeft = 0;
            // Iterate through this.ResourceQuantity if key equals "water" add value to numberOfLiquidRessourcesLeft
            foreach (var resource in this.ResourceQuantity)
            {
                if (resource.Key.Equals(ResourceKind.Water))
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
                if (resource.Key.Equals(ResourceKind.Oxygen))
                {
                    numberOfGasRessourcesLeft += resource.Value;
                }
            }
            return numberOfGasRessourcesLeft;

        }

        public Dictionary<ResourceKind, int> GetListOfSolidRessources()
        {
            // Return the dictionnary containing only the solid ressources
            Dictionary<ResourceKind, int> solidRessources = new Dictionary<ResourceKind, int>();
            // Iterate through this.ResourceQuantity if key equals "aluminium", "carbon", "gold", "iron", "titanium" add value and key to dictionary
            foreach (var resource in this.ResourceQuantity)
            {
                if (resource.Key.Equals(ResourceKind.Carbon) || resource.Key.Equals(ResourceKind.Aluminium) || resource.Key.Equals(ResourceKind.Gold) || resource.Key.Equals(ResourceKind.Iron) || resource.Key.Equals(ResourceKind.Titanium))
                {
                    solidRessources.Add(resource.Key, resource.Value);
                }
            }
            return solidRessources;
        }








    }
}