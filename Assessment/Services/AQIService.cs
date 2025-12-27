
namespace Assessment.Services
{
    public class AQIService
    {
        private readonly HttpClient _httpClient;
        private readonly string apiKey = "e5de8da15db8c29fbad455b569dde202";
        public AQIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAqiAsync(double lat, double lon)
        {
            var url = $"https://api.openweathermap.org/data/2.5/air_pollution?lat={lat}&lon={lon}&appid={apiKey}";
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
