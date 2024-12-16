using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderAPI.Models
{
    [Serializable, BsonIgnoreExtraElements]
    public class Order
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("user_id"), BsonRepresentation(BsonType.String)]
        public string UserId { get; set; }

        [BsonElement("total_price"), BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalPrice { get; set; }

        [BsonElement("total_items"), BsonRepresentation(BsonType.Int32)]
        public decimal TotalItems { get; set; }

        public DateTime OrderedOn { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }
}
