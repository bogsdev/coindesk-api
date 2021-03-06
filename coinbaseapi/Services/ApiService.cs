using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using coinbaseapi.Hubs;
using coinbaseapi.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace coinbaseapi.Services
{
    public class ApiService : IApiService
    {
        private HttpClient _httpClient;
        private int _pollingInterval;
        private IMemoryCache _cache;
        private IHubContext<BTCHub> _hubContext;

        public ApiService(
          HttpClient httpClient,
          IMemoryCache cache,
          IHubContext<BTCHub> hubContext)
        {
            _httpClient = httpClient;
            _pollingInterval = 10000;
            _cache = cache;
            _hubContext = hubContext;
        }

        public async Task<Price> GetCurrentBtcAndEthereumPrices()
        {
            var response = await _httpClient.GetStringAsync("https://api.coingecko.com/api/v3/simple/price?ids=bitcoin%2Cethereum&vs_currencies=nzd");
            CurrentPriceResponse currentPrice = JsonConvert.DeserializeObject<CurrentPriceResponse>(response);
            return new Price() { BitcoinValue = currentPrice.Bitcoin.RateFloat, EthereumValue = currentPrice.Ethereum.RateFloat, Date = DateTime.UtcNow };
        }

        public async Task StartPollingCoindesk()
        {
            while (true)
            {
                Price currentPrice = await GetCurrentBtcAndEthereumPrices();
                AddPriceToListInMemory(currentPrice);
                SendCurrentPriceToHub(currentPrice);
                Thread.Sleep(_pollingInterval);
            }
        }

        private void SendCurrentPriceToHub(Price price)
        {
            _hubContext.Clients.All.SendAsync("ReceivePrice", price);
        }

        private void AddPriceToListInMemory(Price price)
        {
            IList<Price> priceList = _cache.Get("PriceList") as List<Price>;
            if (priceList == null)
            {
                _cache.CreateEntry("PriceList");
                _cache.Set("PriceList", new List<Price>() { price });
                priceList = _cache.Get("PriceList") as List<Price>;
            }

            if (priceList.Count == 5)
            {
                priceList.RemoveAt(0);
            }
            priceList.Add(price);
            _cache.Set("PriceList", priceList);
        }
    }
}