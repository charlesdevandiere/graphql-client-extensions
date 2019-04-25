using Newtonsoft.Json;

namespace GraphQL.Client.Extensions.UnitTests.Models
{
    public class Load
    {
        [JsonProperty("weight")]
        public int Weight { get; set; }
    }
}
