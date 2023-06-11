using System.Text.Json;
using Demo_API.Models;

namespace Demo_API.Services
{
    public class TargetAssetService : ITargetAssetService
    {
        private readonly HttpClient _client;

        public TargetAssetService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<TargetAsset>> GetTargetAssets()
        {
            List<TargetAsset> retVal = null;
            var response = await _client.GetAsync("https://06ba2c18-ac5b-4e14-988c-94f400643ebf.mock.pstmn.io/targetAsset");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching target assets: {response.StatusCode}");
            }

            using (var reponseStream = await response.Content.ReadAsStreamAsync())
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                retVal = await JsonSerializer.DeserializeAsync<List<TargetAsset>>(reponseStream, options);
            }
            return retVal;

        }
    }
}