namespace ApiTests.Models
{
    // User model representing a user with nested address and company details
    public class User
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public Address address { get; set; } = new Address();
        public string phone { get; set; } = string.Empty;
        public string website { get; set; } = string.Empty;
        public Company company { get; set; } = new Company();
    }
    
    // Nested Address model within User
    public class Address
    {
        public string street { get; set; } = string.Empty;
        public string suite { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string zipcode { get; set; } = string.Empty;
        public Geo geo { get; set; } = new Geo();
    }

    // Nested Geo model within Address
    public class Geo
    {
        public string lat { get; set; } = string.Empty;
        public string lng { get; set; } = string.Empty;
    }

    // Nested Company model within User
    public class Company
    {
        public string name { get; set; } = string.Empty;
        public string catchPhrase { get; set; } = string.Empty;
        public string bs { get; set; } = string.Empty;
    }
}
