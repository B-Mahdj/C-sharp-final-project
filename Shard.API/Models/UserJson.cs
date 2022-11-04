﻿using System.Text.Json.Serialization;

namespace Shard.API.Models
{
    public class UserJson
    {
        public string Id { get; set; }
        public string Pseudo { get; set; }
        public string DateOfCreation { get; } = DateTime.Now.ToString();
        
        [JsonIgnore]
        public List<Unit> Units { get; }
        [JsonIgnore]
        public List<Building> Buildings { get; }

        public IReadOnlyDictionary<String, int> ResourcesQuantity { get; }

        public UserJson(User user)
        {
            this.Id = user.Id;
            this.Pseudo = user.Pseudo;
            this.DateOfCreation = user.DateOfCreation;
            this.Units = user.Units;
            this.Buildings = user.Buildings;
            this.ResourcesQuantity = user.ResourcesQuantity;
        }
    }
}
