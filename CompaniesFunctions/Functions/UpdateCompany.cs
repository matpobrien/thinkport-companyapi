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
using System.Net.Http;

namespace Companies.Functions
{
    public static class UpdateCompany
    {

        [FunctionName("UpdateCompany")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "Company/{partitionKey}/{id}")] HttpRequest req,
            [CosmosDB(
                "CompaniesDB",
                "Companies",
                ConnectionStringSetting = "CosmosDbConnectionString", Id = "{id}",
                PartitionKey = "{partitionKey}")] Company companyToUpdate,
            [CosmosDB("CompaniesDB", "Companies", ConnectionStringSetting = "CosmosDbConnectionString")] IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var companyRequest = JsonConvert.DeserializeObject<Company>(requestBody);

                // companyToUpdate.Name = companyRequest.Name;
                // companyToUpdate.LegalEntity = companyRequest.LegalEntity;
                // companyToUpdate.Employees = companyRequest.Employees;
                // companyToUpdate.Equity = companyRequest.Equity;

                companyToUpdate = new Company
                {
                    Id = companyRequest.Id,
                    Name = companyRequest.Name,
                    LegalEntity = companyRequest.LegalEntity,
                    Employees = companyRequest.Employees,
                    Equity = companyRequest.Equity
                };

                await documentsOut.AddAsync(companyToUpdate);
                return new OkObjectResult(companyToUpdate);
            }

            catch (Exception ex)
            {
                log.LogError($"Internal Server Error. Exception: {ex.Message}");

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }
    }
}
