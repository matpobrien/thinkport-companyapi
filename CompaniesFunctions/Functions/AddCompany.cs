using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Companies.Models;

namespace Companies.Functions
{
    public static class AddCompany
    {
        [FunctionName(nameof(AddCompany))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies")] HttpRequest req,
            [CosmosDB(
                "CompaniesDB",
                "Companies",
                ConnectionStringSetting = "CosmosDbConnectionString",
                CreateIfNotExists = true,
                PartitionKey = "/id",
                PreferredLocations = "West Europe")]IAsyncCollector<dynamic> documentsOut,
                ILogger log)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var companyRequest = JsonConvert.DeserializeObject<Company>(requestBody);

                var company = new Company
                {
                    Id = Guid.NewGuid().ToString().Substring(0, 5),
                    Name = companyRequest.Name,
                    LegalEntity = companyRequest.LegalEntity,
                    Employees = companyRequest.Employees,
                    Equity = companyRequest.Equity
                };

                await documentsOut.AddAsync(company);

                return new OkObjectResult(company);
            }

            catch (Exception ex)
            {
                log.LogError($"Internal Server Error. Exception: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
