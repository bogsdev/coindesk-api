using Newtonsoft.Json;

namespace coinbaseapi.Models
{

    public class CurrentPriceResponse
    {
        public CurrentPrice Ethereum { get; set; }
        public CurrentPrice Bitcoin { get; set; }
    }


    public class CurrentPrice
    {
        //rate in NZD
        [JsonProperty("nzd")]
        public float RateFloat { get; set; }
    }

}