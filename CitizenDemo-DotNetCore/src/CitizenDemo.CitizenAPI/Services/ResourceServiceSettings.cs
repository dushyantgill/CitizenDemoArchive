namespace CitizenDemo.CitizenAPI.Services
{
    public class ResourceServiceSettings : IResourceServiceSettings
    {
        public string? Url { get; set; }
    }

    public interface IResourceServiceSettings
    {
        string? Url { get; set; }
    }
}
