using Newtonsoft.Json;

namespace CitizenDemo.LoadGenerator.Models
{
    public class Citizen
    {
        [JsonProperty(PropertyName = "citizenId")]
        public string? CitizenId { get; set; }

        [JsonProperty(PropertyName = "tenantId")]
        public string? TenantId { get; set; }

        [JsonProperty(PropertyName = "givenName")]
        public string? GivenName { get; set; }

        [JsonProperty(PropertyName = "surname")]
        public string? Surname { get; set; }

        [JsonProperty(PropertyName = "phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "streetAddress")]
        public string? StreetAddress { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string? City { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string? State { get; set; }

        [JsonProperty(PropertyName = "postalCode")]
        public string? PostalCode { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string? Country { get; set; }

        public override string ToString()
        {
            return string.Format(
                "Citizen: CitizenId: {0}, GivenName: {1}, Surname: {2}, PhoneNumber: {3}, " +
                "StreetAddress: {4}, City: {5}, State: {6}, PostalCode: {7}, Country: {8}", 
                CitizenId, GivenName, Surname, PhoneNumber, StreetAddress, City, State, PostalCode, Country
                );
        }
    }
}