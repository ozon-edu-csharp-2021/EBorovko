using System;
using System.Threading;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Ozon.MerchandiseService.HttpModels;


namespace Ozon.MerchandiseService.HttpClient
{
    public class MerchHttpClient
    {
        private readonly System.Net.Http.HttpClient _httpClient;

        public MerchHttpClient(System.Net.Http.HttpClient httpClient, string baseUri)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseUri);
        }

        public async Task<ProvideResponse> ProvideAsync(ProvideRequest provideRequest, CancellationToken token)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/merches/provide", provideRequest, token);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Cannot provide merch");
            
            return await response.Content.ReadFromJsonAsync<ProvideResponse>(cancellationToken: token);
        }
        
        public async Task<CheckProvidingResponse> CheckProvidingAsync(CheckProvidingRequest checkProvidingRequest, CancellationToken token)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/merches/check-providing", checkProvidingRequest, token);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Cannot check providing merch");
            
            return await response.Content.ReadFromJsonAsync<CheckProvidingResponse>(cancellationToken: token);
        }
    }
}