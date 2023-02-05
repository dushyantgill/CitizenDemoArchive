using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CitizenDemo.CitizenAPI.Models
{
    public class Citizen
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string? InternalId { get; set; }

        [BsonElement("citizenId")]
        public string? CitizenId { get; set; }

        [BsonElement("givenName")]
        public string? GivenName { get; set; }

        [BsonElement("surname")]
        public string? Surname { get; set; }

        [BsonElement("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [BsonElement("streetAddress")]
        public string? StreetAddress { get; set; }

        [BsonElement("city")]
        public string? City { get; set; }

        [BsonElement("state")]
        public string? State { get; set; }

        [BsonElement("postalCode")]
        public string? PostalCode { get; set; }

        [BsonElement("country")]
        public string? Country { get; set; }

        public override string ToString()
        {
            return string.Format(
                "Citizen: CitizenId: {0}, GivenName: {1}, Surname: {2}, PhoneNumber: {3}, " +
                "StreetAddress: {4}, City: {5}, State: {6}, PostalCode: {7}, Country: {8}", CitizenId,
                GivenName, Surname, PhoneNumber, StreetAddress, City, State, PostalCode, Country
                );
        }
    }
}