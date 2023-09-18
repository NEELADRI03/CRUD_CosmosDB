using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.Azure.Cosmos;

namespace CRUD_CosmosDb
{
    public static class GetOneItem
    {
        [FunctionName("GetOneItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route ="Get/{id}")] HttpRequest req, string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string cosmosEndPointUri = "Your Endpoint Uri";
            string cosmosDbKey = "Your CosmosDB Key";
            string databaseName = "Your Database Name"; //My database name was "Database1"
            string containerName = "Your Container Name";  //My container name was "Container1"

            CosmosClient cosmosClient;
            cosmosClient = new CosmosClient(cosmosEndPointUri, cosmosDbKey);

            Database database = cosmosClient.GetDatabase(databaseName);
            Container container = database.GetContainer(containerName);

            string sqlQuery = $"SELECT c.id,c.orderID,c.category,c.quantity FROM Container1 c WHERE c.orderID='{id}'";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);

            FeedIterator<Order> feedIterator = container.GetItemQueryIterator<Order>(queryDefinition);

            while (feedIterator.HasMoreResults)
            {
                FeedResponse<Order> feedResponse = await feedIterator.ReadNextAsync();
                foreach (Order order in feedResponse)
                {
                    return new OkObjectResult(order);
                }
            }
            return new OkObjectResult(null);
        }
    }
}
