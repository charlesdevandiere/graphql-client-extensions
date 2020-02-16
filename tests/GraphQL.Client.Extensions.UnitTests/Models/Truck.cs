using Newtonsoft.Json;

namespace GraphQL.Client.Extensions.UnitTests.Models
{
    public class Truck
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("wheelsNumber")]
        public int WheelsNumber { get; set; }

        [JsonProperty("load")]
        public Load Load { get; set; }

        [JsonProperty("speedLimits")]
        public int[] SpeedLimits { get; set; }
    }
}
