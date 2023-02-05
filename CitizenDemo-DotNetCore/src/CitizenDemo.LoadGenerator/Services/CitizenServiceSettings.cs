namespace CitizenDemo.LoadGenerator.Services
{
    public class CitizenServiceSettings : ICitizenServiceSettings
    {
        public string? Url { get; set; }
    }

    public interface ICitizenServiceSettings
    {
        string? Url { get; set; }
    }
}
