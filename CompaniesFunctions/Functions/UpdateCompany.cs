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

// what is the difference between a namespace and an interface?
namespace Companies.Functions
{
    public class CompanyPatch
    {
        public string Name { get; set; }
        public string LegalEntity { get; set; }
        public int Employees { get; set; }
        public int Equity { get; set; }
    }
    public static class UpdateCompany
    {

        [FunctionName(nameof(UpdateCompany))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "companies/{id}")] HttpRequest req,
            // look into the partition key a bit more
            [CosmosDB(
                "CompaniesDB",
                "Companies",
                ConnectionStringSetting = "CosmosDbConnectionString", Id = "{id}",
                PartitionKey = "{id}")] Company companyToUpdate,
            [CosmosDB("CompaniesDB", "Companies", ConnectionStringSetting = "CosmosDbConnectionString")] IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            try
            {
                // http requests are sent as packages which are read as a stream by the computer, a low level library will need to interact with those packages (especially for larger files) and read them byte by byte. The packages are also received as packages which then need to be deserialized into a json object
                // example: using fetch rather than axios in js
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var companyRequest = JsonConvert.DeserializeObject<CompanyPatch>(requestBody);

                var updatedCompany = new Company
                {
                    Id = companyToUpdate.Id,
                    Name = (String.IsNullOrEmpty(companyRequest.Name) ? companyToUpdate.Name : companyRequest.Name),
                    LegalEntity = String.IsNullOrEmpty(companyRequest.LegalEntity) ? companyToUpdate.LegalEntity : companyRequest.LegalEntity,
                    Employees = (companyRequest.Employees == 0 ? companyToUpdate.Employees : companyRequest.Employees),
                    Equity = (companyRequest.Equity == 0 ? companyToUpdate.Equity : companyRequest.Equity)
                };

                await documentsOut.AddAsync(updatedCompany);
                return new OkObjectResult(updatedCompany);
            }

            catch (Exception ex)
            {
                log.LogError($"Internal Server Error. Exception: {ex.Message}");

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }
    }
}
