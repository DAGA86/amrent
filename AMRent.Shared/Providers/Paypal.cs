using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace AMRent.Shared.Providers
{
    public class PayPal
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _secret;
        private readonly string _paypalBaseUrl;
        private readonly string _appBaseUrl;

        public PayPal(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _clientId = configuration["PayPal:ClientId"];
            _secret = configuration["PayPal:Secret"];
            _paypalBaseUrl = configuration["PayPal:BaseUrl"]; // Ex: https://api.sandbox.paypal.com
            _appBaseUrl = configuration["AppBaseUrl"];
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var authToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_clientId}:{_secret}"));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var body = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

            var response = await _httpClient.PostAsync($"{_paypalBaseUrl}/v1/oauth2/token", body);
            response.EnsureSuccessStatusCode();

            var result = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return result.RootElement.GetProperty("access_token").GetString();
        }

        public async Task<JsonDocument> CreateOrderAsync(string accessToken, decimal amount, string reservationNumber)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var payload = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        reference_id = reservationNumber,
                        amount = new
                        {
                            currency_code = "EUR",
                            value = amount.ToString("0.00", CultureInfo.InvariantCulture)
                        }
                    }
                },
                application_context = new
                {
                    return_url = $"{_appBaseUrl}Booking/PaypalCapture",  // configura no appsettings
                    cancel_url = $"{_appBaseUrl}Booking/PaypalCapture"
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PostAsync($"{_paypalBaseUrl}/v2/checkout/orders", content);
            response.EnsureSuccessStatusCode();

            var result = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<JsonDocument> CaptureOrderAsync(string orderId, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_paypalBaseUrl}/v2/checkout/orders/{orderId}/capture");
            request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var result = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return result;
        }
    }
}