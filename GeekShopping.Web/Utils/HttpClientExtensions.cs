using System.Net.Http.Headers;
using System.Text.Json;

namespace GeekShopping.Web.Utils;

public static class HttpClientExtensions
{
    private static readonly MediaTypeHeaderValue contentType = new MediaTypeHeaderValue("application/json");
    public static async Task<T?> ReadContentAs<T>(this HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            throw new ApplicationException($"Request failed with status code {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(content))
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException ex)
        {
            throw new ApplicationException($"Error deserializing response content: {ex.Message}", ex);
        }
    }
    // Method to create a StringContent from a value of type T
    private static StringContent GetJsonContent<T>(T value)
    {
        return new StringContent(JsonSerializer.Serialize(value), System.Text.Encoding.UTF8, contentType);
    }

    public static Task<HttpResponseMessage> PostAsJson<T>(this HttpClient client, string requestUri, T value)
        => client.PostAsync(requestUri, GetJsonContent(value));

    public static Task<HttpResponseMessage> PutAsJson<T>(this HttpClient client, string requestUri, T value)
        => client.PutAsync(requestUri, GetJsonContent(value));
   
}
