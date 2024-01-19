using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace SayList.Utilities;

public class HttpRequestUtility
{
    private readonly HttpClient _httpClient;

    public HttpRequestUtility(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<T> PostFormUrlEncodedAsync<T>(string url, Dictionary<string, string> formData)
    {
        using var requestContent = new FormUrlEncodedContent(formData);
        var response = await _httpClient.PostAsync(url, requestContent);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent) ?? throw new JsonException("Failed to deserialize response");
    }

    public async Task<T> GetAsync<T>(string url, Dictionary<string, string> headers, Dictionary<string, string> queryParams = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Add headers to the request
        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        // Add query parameters to the request URL
        if (queryParams != null)
        {
            request.RequestUri = new Uri(QueryHelpers.AddQueryString(url, queryParams));
        }

        var response = await _httpClient.SendAsync(request);


        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent) ?? throw new JsonException("Failed to deserialize response");
    }
}