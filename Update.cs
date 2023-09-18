using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;

namespace CRUD_CosmosDb
{
    public static class Update
    {
        [FunctionName("Update")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route =null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string cosmosEndPointUri = "Your Endpoint Uri";
            string cosmosDbKey = "Your CosmosDB Key";
            string databaseName = "Your Database Name"; //My database name was "Database1"
            string containerName = "Your Container Name";  //My container name was "Container1"

            CosmosClient cosmosClient;
            cosmosClient = new CosmosClient(cosmosEndPointUri, cosmosDbKey);

            Database database=cosmosClient.GetDatabase(databaseName);
            Container container = database.GetContainer(containerName);

            string orderID = req.Query["orderID"];
            string category = req.Query["category"];
            string quantity = req.Query["quantity"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            orderID = orderID ?? data?.orderID;
            category = category ?? data?.category;
            quantity = quantity ?? data?.quantity;

            
            string sqlQuery = $"SELECT c.id,c.category FROM Container1 c WHERE c.orderID='{orderID}'";

            string id = "";
           

            QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);

            FeedIterator<Order> feedIterator = container.GetItemQueryIterator<Order>(queryDefinition);

            while(feedIterator.HasMoreResults)
            {
                FeedResponse<Order> feedResponse=await feedIterator.ReadNextAsync();
                foreach(Order order in feedResponse)
                {
                    id=order.id; 
                    category=order.category;
                }
            }

            ItemResponse<Order> response=await container.ReadItemAsync<Order>(id,new PartitionKey(category));

            var item = response.Resource;
            item.quantity = quantity;

            await container.ReplaceItemAsync<Order>(item, id, new PartitionKey(category));

            return new OkObjectResult($"Item with id '{id}' updated successfully");
        }
    }
}
