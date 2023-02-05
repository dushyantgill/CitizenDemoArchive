using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CitizenDemo.ResourceAPI.Models
{
    public class Resource
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement ("_id")]
        public string? InternalId { get; set; }

        [BsonElement("resourceId")]
        public string? ResourceId { get; set; }

        [BsonElement("citizenId")]
        public string? CitizenId { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("status")]
        public string? Status { get; set; }
        public override string ToString()
        {
            return string.Format(
                "Resource: ResourceId: {0}, CitizenId: {1}, Name: {2}, Status: {3}",
                ResourceId, CitizenId, Name, Status
                );
        }
    }
}