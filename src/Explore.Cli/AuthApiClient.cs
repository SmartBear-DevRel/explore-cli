using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

public class AuthUApiClient
{
    private readonly HttpClient _httpClient;

    public AuthUApiClient()
    {
        _httpClient = new HttpClient();
    }

    public async Task<JsonDocument> GetDeviceCodeAsync(string clientId, string audience, string scope)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("audience", audience),
            new KeyValuePair<string, string>("scope", scope)
        });

        var response = await _httpClient.PostAsync("https://acme-demo.auth0.com/oauth/device/code", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonDocument.Parse(responseContent);
    }

    public async Task<JsonDocument> ExchangeDeviceCodeForTokenAsync(string clientId, string deviceCode)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("device_code", deviceCode),
            new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:device_code")
        });

        var response = await _httpClient.PostAsync("https://acme-demo.auth0.com/oauth/token", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonDocument.Parse(responseContent);
    }

    public async Task<JsonDocument> GetUserInfoAsync(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.GetAsync("https://acme-demo.auth0.com/userinfo");
        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonDocument.Parse(responseContent);
    }
}

// public static async Task Main()
// {
    // var clientId = "nZ8JDrV8Hklf3JumewRl2ke3ovPZn5Ho";
    // var audience = "urn:my-videos";
    // var scope = "offline_access openid profile";

    // var authUApiClient = new AuthUApiClient();

    // // Step 1: Get Device Code
    // var deviceCodeResponse = await authUApiClient.GetDeviceCodeAsync(clientId, audience, scope);
    // var deviceCode = deviceCodeResponse.RootElement.GetProperty("device_code").GetString();
    // var userCode = deviceCodeResponse.RootElement.GetProperty("user_code").GetString();
    // var verificationUri = deviceCodeResponse.RootElement.GetProperty("verification_uri").GetString();

    // Console.WriteLine($"Authorization Request: {verificationUri}");
    // Console.WriteLine($"User Code: {userCode}");

    // // Step 2: Exchange Device Code for Token
    // Console.WriteLine("Waiting for user authorization...");
    // while (true)
    // {
    //     var exchangeResponse = await authUApiClient.ExchangeDeviceCodeForTokenAsync(clientId, deviceCode);
    //     if (exchangeResponse.RootElement.TryGetProperty("error", out var error))
    //     {
    //         if (error.GetString() == "authorization_pending")
    //         {
    //             await Task.Delay(TimeSpan.FromSeconds(5));
    //             continue;
    //         }
    //         else
    //         {
    //             Console.WriteLine($"Error: {error.GetString()}");
    //             break;
    //         }
    //     }

    //     var accessToken = exchangeResponse.RootElement.GetProperty("access_token").GetString();

    //     // Step 3: Get User Info
    //     var userInfoResponse = await authUApiClient.GetUserInfoAsync(accessToken);
    //     var nickname = userInfoResponse.RootElement.GetProperty("nickname").GetString();

    //     Console.WriteLine($"Welcome, {nickname}!");
    //     Console.WriteLine("Start streaming your favourite shows from AuthU TV");

    //     break;
    // }
// }