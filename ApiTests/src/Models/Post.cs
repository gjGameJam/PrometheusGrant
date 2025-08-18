namespace ApiTests.Models
{
    // Post model representing a blog post
    public class Post
    {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string body { get; set; } = string.Empty;
    }
}
