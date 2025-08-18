namespace ApiTests.Models
{
    // Todo model representing a task item
    public class Todo
    {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public bool completed { get; set; }
    }
}
