using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Companies.Models;

namespace Companies.Functions
{
    public class GetCompanyById
    {
        [FunctionName(nameof(GetCompanyById))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Company/{partitionKey}/{id}")] HttpRequestMessage req,
            [CosmosDB(
                databaseName: "CompaniesDB",
                collectionName: "Companies",
                ConnectionStringSetting = "CosmosDbConnectionString",
                Id = "{id}",
                PartitionKey = "{id}")] Company companyToFind, ILogger log)
        {
            if (companyToFind == null)
            {
                log.LogWarning($"No Company with given id found!");
                return new NotFoundResult();
            }
            return new OkObjectResult(companyToFind);
        }
    }
}
