using Newtonsoft.Json;

namespace GraphQL.Client.Extensions.UnitTests.Models
{
    public class Truck
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("weelsNumber")]
        public int WeelsNumber { get; set; }

        [JsonProperty("load")]
        public Load Load { get; set; }
    }
}
