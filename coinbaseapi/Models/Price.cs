using System;

namespace coinbaseapi.Models
{

    public class Price
    {
        public float BitcoinValue { get; set; }
        public float EthereumValue { get; set; }
        public DateTime Date { get; set; }
    }
}
