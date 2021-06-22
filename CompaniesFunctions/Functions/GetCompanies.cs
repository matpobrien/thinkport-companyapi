using System.Threading.Tasks;
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
        [FunctionName(nameof(GetCompanies))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Companies")] HttpRequest req,
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
