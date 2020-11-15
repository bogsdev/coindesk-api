using System.Threading.Tasks;
using coinbaseapi.Models;

public interface IApiService
{
    public Task<Price> GetCurrentBtcAndEthereumPrices();
    public Task StartPollingCoindesk();
}