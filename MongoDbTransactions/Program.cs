using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbTransactions.Entities;
using System;
using System.Collections.Generic;

namespace MongoDbTransactions
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("TestTransaction");
            var productCollection = database.GetCollection<Product>("Product");
            var orderCollection = database.GetCollection<Order>("Order");

            var product = productCollection.Find(Builders<Product>.Filter.Eq("Name", "Camisa")).FirstOrDefault();

            var orderItem = new List<OrderItem>();
            orderItem.Add(new OrderItem { ProductId = product.Id, Quantity = 1 });
            var order = new Order()
            {
                Items = orderItem
            };

            using (var session = client.StartSession())
            {
                try
                {
                    session.StartTransaction();                    

                    var filterProduct = Builders<Product>.Filter.Eq("_id", ObjectId.Parse(product.Id));
                    var updateProduct = Builders<Product>.Update.Inc("Quantity", order.TotalQuantity() * -1);

                    orderCollection.InsertOne(session, order);
                    var result = productCollection.UpdateOne(session, filterProduct, updateProduct);

                    if (result.ModifiedCount != 1)
                        throw new Exception("Occurred an error in updating the product");

                    session.CommitTransaction();
                }
                catch (Exception ex)
                {
                    session.AbortTransaction();
                }
            }
        }
    }
}
