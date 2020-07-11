using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDbTransactions.Entities
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        public List<OrderItem> Items { get; set; }

        public int TotalQuantity()
        {
            return Items.Sum(x => x.Quantity);
        }
    }

    public class OrderItem
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string ProductId { get; set; }
        public int Quantity { get; set; }

    }
}
