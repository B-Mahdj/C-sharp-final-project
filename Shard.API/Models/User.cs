namespace Shard.API.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Pseudo { get; set; }
        public string DateOfCreation { get; } = DateTime.Now.ToString();

        public List<Unit> Units { get; } = new List<Unit>();

        public User(string id, string pseudo)
        {
            this.Id = id;
            this.Pseudo = pseudo;
        }
    }
}
