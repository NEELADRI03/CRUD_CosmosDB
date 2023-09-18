using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.Azure.Cosmos;

namespace CRUD_CosmosDb
{
    public static class AddItems
    {
        [FunctionName("AddItems")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                    databaseName: "Database1",
                    containerName: "Container1",
                    Connection = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string orderID = req.Query["orderID"];
            string category = req.Query["category"];
            string quantity = req.Query["quantity"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            orderID = orderID ?? data?.orderID;
            category = category ?? data?.category;
            quantity = quantity ?? data?.quantity;

            await documentsOut.AddAsync(new
            {
                // create a random ID
                id = System.Guid.NewGuid().ToString(),
                orderID = orderID,
                category = category,
                quantity = quantity
            });
            

            string responseMessage = string.IsNullOrEmpty(category)
                ? "Pass parameters"
                : $"Document Added Successfully";

            return new OkObjectResult(responseMessage);
        }

        
        }
    }

