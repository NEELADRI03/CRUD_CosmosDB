using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;

namespace CRUD_CosmosDb
{
    public static class GetAllItems
    {
        [FunctionName("GetAllItems")]
        public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [CosmosDB(databaseName: "Your Database Name", //My database name was "Database1"
            containerName: "Your Container Name", //My container name was "Container1"
            SqlQuery = "SELECT * FROM Container1",
            Connection = "CosmosDbConnectionString")] IEnumerable<Order> orders,
    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (orders is null)
            {
                return new NotFoundResult();
            }

            foreach (var ord in orders)
            {
                log.LogInformation(ord.orderID);
            }

            return new OkObjectResult(orders);
        }
    }
    }

