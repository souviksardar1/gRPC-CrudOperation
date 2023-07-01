using Grpc.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkorderPackage;
using static WorkorderPackage.WorkorderService;

namespace app.server.Impl
{
    public class WorkorderServiceImpl : WorkorderServiceBase
    {
        public static MongoClient mongoClient = new MongoClient("mongodb://localhost:27017");
        public static IMongoDatabase db = mongoClient.GetDatabase("WorkorderDb");
        public static IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>("Workorders");
        public override Task<CreateWorkorderOutput> CreateWo(CreateWorkorderInput request, ServerCallContext context)
        {
            var wo = request.Input;
            BsonDocument doc = new BsonDocument("location", wo.Location).Add("systemId", wo.SystemId);
            collection.InsertOne(doc);

            string id = doc.GetValue("_id").ToString();
            wo.Id = id;

            return Task.FromResult(new CreateWorkorderOutput { Output = wo });
        }

        public override Task<ReadWorkorderOutput> ReadWo(ReadWorkorderInput request, ServerCallContext context)
        {
            var id = request.Id;
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var res = collection.Find(filter).FirstOrDefault();
            if (res != null)
            {

                var response = new ReadWorkorderOutput
                {
                    Output =
                            new WorkorderData
                            {
                                Id = Convert.ToString(res.GetValue("_id")),
                                SystemId = res.GetValue("systemId").AsInt32,
                                Location = res.GetValue("location").AsString,
                            }
                };
                return Task.FromResult(response);
            }
            else
            {
                return Task.FromResult(new ReadWorkorderOutput { Output = new WorkorderData { } });
            }
        }
        public override Task<UpdateWorkorderOutput> UpdateWo(UpdateWorkorderInput request, ServerCallContext context)
        {
            var id = request.Id;

            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));

            var update = Builders<BsonDocument>.Update.Set("location", "India");

            // Update the document
            var updateResult = collection.UpdateOne(filter, update);

            if (updateResult.ModifiedCount > 0)
            {
                var response = new UpdateWorkorderOutput { ModifiedCount = (int)updateResult.ModifiedCount };
                return Task.FromResult(response);
            }
            else
            {
                return Task.FromResult(new UpdateWorkorderOutput { ModifiedCount = 0});
            }
        }

        public override Task<DeleteWorkorderOutput> DeleteWo(DeleteWorkorderInput request, ServerCallContext context)
        {
            var id = request.Id;

            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));

            var deleteRes = collection.DeleteOne(filter);

            if (deleteRes.DeletedCount > 0)
            {
                return Task.FromResult(new DeleteWorkorderOutput { DeleteCount = (int)deleteRes.DeletedCount });
            }
            else
            {
                return Task.FromResult(new DeleteWorkorderOutput { DeleteCount = 0 });
            }
        }
    }
}
