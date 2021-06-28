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
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{id}")] HttpRequest req,
            // could I have used a CosmosDBTrigger here with IReadOnlyList<Document> companyToFind as the return type?
            [CosmosDB(
                databaseName: "CompaniesDB",
                collectionName: "Companies",
                ConnectionStringSetting = "CosmosDbConnectionString",
                Id = "{id}",
                PartitionKey = "{id}")] Company companyToFind, ILogger log)

        /*
        Query alternative:
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        [CosmosDB(
        databaseName: "CompaniesDB",
        collectionName: "Companies",
        ConnectionStringSetting = "CosmosDbConnectionString",
        Id = "{Query.id}",
        PartitionKey = "{Query.partitionKey}")] Company companyToFind,
        */
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
