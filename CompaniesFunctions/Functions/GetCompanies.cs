using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Companies.Models;

namespace Companies.Functions
{
    public class GetCompanies
    {
        // making sure the function is serialized with a certain name
        // attribute decorating the run function as the function that should be called by the azure function
        [FunctionName(nameof(GetCompanies))]
        public static IActionResult Run(
            // The square brackets in front of the arguments and in front of the function are attributes -> look up C# attributes, basically alow you to attach extra information to declarations
            // I should probably look more into these triggers
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies")] HttpRequest req,
            // Is this an output or input trigger?
            [CosmosDB("CompaniesDB", "Companies", ConnectionStringSetting = "CosmosDbConnectionString",
            SqlQuery = "SELECT * FROM c")] IEnumerable<Company> companies, ILogger log)
        {
            if (companies == null)
            {
                log.LogWarning("No Companies found!");
                return new NotFoundResult();
            }
            return new OkObjectResult(companies);
        }
    }
}
