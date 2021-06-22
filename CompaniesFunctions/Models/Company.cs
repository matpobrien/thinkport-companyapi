using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Companies.Models
{
    public class Company
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string LegalEntity { get; set; }
        public int Employees { get; set; }
        public int Equity { get; set; }
    }
}
