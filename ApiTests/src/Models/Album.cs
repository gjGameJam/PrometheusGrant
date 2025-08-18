namespace ApiTests.Models
{
    // Album model representing a photo album
    public class Album
    {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
    }
}
