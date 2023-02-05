namespace CitizenDemo.LoadGenerator.Models
{
    public class SampleData
    {
        public List<string>? GivenNames { get; set; }
        public List<string>? Surnames { get; set; }
        public List<string>? StreetNames { get; set; }
        public List<CityData>? Cities { get; set; }
    }
    public class CityData
    {
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? AreaCode { get; set; }
    }
}