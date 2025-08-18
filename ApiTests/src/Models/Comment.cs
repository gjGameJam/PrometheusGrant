namespace ApiTests.Models
{
    // Comment model representing a comment on a post
    public class Comment
    {
        public int postId { get; set; }
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string body { get; set; } = string.Empty;
    }
}
