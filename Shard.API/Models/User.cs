namespace Shard.API.Models
{
    public class User
    {
        public string id { get; set; }
        public string pseudo { get; set; }
        public string DateOfCreation { get; } = DateTime.Now.ToString();

        public User(string id, string pseudo)
        {
            this.id = id;
            this.pseudo = pseudo;
        }
    }
}
