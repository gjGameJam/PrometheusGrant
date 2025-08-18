namespace ApiTests.Models
{
    // Photo model representing a photo in an album
    public class Photo
    {
        public int albumId { get; set; }
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string url { get; set; } = string.Empty;
        public string thumbnailUrl { get; set; } = string.Empty;
    }
}
