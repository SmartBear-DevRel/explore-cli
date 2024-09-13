using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Explore.Cli.Models.Explore;
using System.Threading.Tasks;
using System.CommandLine;
using System.Net;
using System.Net.Http.Json;
using Spectre.Console;
namespace Explore.Cli.ExploreHttpClient;
public class ExploreHttpClient
{
    private readonly HttpClient _httpClient;

    public ExploreHttpClient(string baseAddress = "https://api.explore.swaggerhub.com/spaces-api/v1")
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
    }

    public async Task<bool> CheckSpaceExists(string exploreCookie, string? id, bool? verboseOutput)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        _httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        _httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");

        var spacesResponse = await _httpClient.GetAsync($"/spaces/{id}");

        if (spacesResponse.StatusCode == HttpStatusCode.OK)
        {
            if (!UtilityHelper.IsContentTypeExpected(spacesResponse.Content.Headers, "application/hal+json"))
            {
                Console.WriteLine(spacesResponse);
                AnsiConsole.MarkupLine($"[red]Please review your credentials, Unexpected response GET spaces endpoint[/]");
                throw new HttpRequestException("Please review your credentials, Unexpected response GET spaces endpoint");
            }

            return true;
        }

        if (verboseOutput != null && verboseOutput == true)
        {
            AnsiConsole.MarkupLine($"[orange3]StatusCode {spacesResponse.StatusCode} returned from the GetSpaceById API. New space will be created[/]");
        }

        return false;
    }

    public async Task<bool> CheckApiExists(string exploreCookie, string spaceId, string? id, bool? verboseOutput)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        _httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        _httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");

        var spacesResponse = await _httpClient.GetAsync($"/spaces/{spaceId}/apis/{id}");

        if (spacesResponse.StatusCode == HttpStatusCode.OK)
        {
            return true;
        }

        if (verboseOutput != null && verboseOutput == true)
        {
            AnsiConsole.MarkupLine($"[orange3]StatusCode {spacesResponse.StatusCode} returned from the GetApiById API. New API will be created in the space.[/]");
        }

        return false;
    }

    public async Task<bool> CheckConnectionExists(string exploreCookie, string spaceId, string apiId, string? id, bool? verboseOutput)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }



        _httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        _httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");

        var response = await _httpClient.GetAsync($"/spaces/{spaceId}/apis/{apiId}/connections/{id}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return true;
        }

        if (verboseOutput != null && verboseOutput == true)
        {
            AnsiConsole.MarkupLine($"[orange3]StatusCode {response.StatusCode} returned from the GetConnectionById API. New connection within API will be created.[/]");
        }

        return false;
    }

    public async Task<SpaceResponse> UpsertSpace(string exploreCookie, bool spaceExists, string? name, string? id)
    {


        var spaceContent = new StringContent(JsonSerializer.Serialize(
            new SpaceRequest()
            {
                Name = name
            }
        ), Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        _httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");

        HttpResponseMessage? spacesResponse;

        if (string.IsNullOrEmpty(id) || !spaceExists)
        {
            spacesResponse = await _httpClient.PostAsync("/spaces", spaceContent);
        }
        else
        {
            spacesResponse = await _httpClient.PutAsync($"/spaces/{id}", spaceContent);

            if (spacesResponse.StatusCode == HttpStatusCode.Conflict)
            {
                // swallow 409 as server is being overly strict
                return new SpaceResponse() { Id = Guid.Parse(id), Name = name };
            }
        }

        if (spacesResponse.IsSuccessStatusCode)
        {
            return await spacesResponse.Content.ReadFromJsonAsync<SpaceResponse>() ?? new SpaceResponse();
        }

        if (!UtilityHelper.IsContentTypeExpected(spacesResponse.Content.Headers, "application/hal+json") && !UtilityHelper.IsContentTypeExpected(spacesResponse.Content.Headers, "application/json"))
        {   
            Console.WriteLine(spacesResponse);
            AnsiConsole.MarkupLine($"[red]Please review your credentials, Unexpected response from POST/PUT spaces API for name: {name}, id:{id}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]StatusCode {spacesResponse.StatusCode} returned from the POST/PUT spaces API for name: {name}, id:{id}[/]");
        }

        return new SpaceResponse();
    }

    public async Task<ApiResponse> UpsertApi(string exploreCookie, bool spaceExists, string spaceId, string? id, string? name, string? type, bool? verboseOutput)
    {


        var apiContent = new StringContent(JsonSerializer.Serialize(
            new ApiRequest()
            {
                Name = name,
                Type = type
            }
        ), Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        _httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");

        HttpResponseMessage? apiResponse;

        if (spaceExists && await CheckApiExists(exploreCookie, spaceId, id, verboseOutput))
        {
            // update the api
            apiResponse = await _httpClient.PutAsync($"/spaces/{spaceId}/apis/{id}", apiContent);

            if (apiResponse.StatusCode == HttpStatusCode.Conflict)
            {
                // swallow 409 as server is being overly strict
                return new ApiResponse() { Id = Guid.Parse(id ?? string.Empty), Name = name, Type = type };
            }
        }
        else
        {
            //create the api
            apiResponse = await _httpClient.PostAsync($"/spaces/{spaceId}/apis", apiContent);
        }

        if (apiResponse.IsSuccessStatusCode)
        {
            return await apiResponse.Content.ReadFromJsonAsync<ApiResponse>() ?? new ApiResponse();
        }

        if (!UtilityHelper.IsContentTypeExpected(apiResponse.Content.Headers, "application/hal+json") && !UtilityHelper.IsContentTypeExpected(apiResponse.Content.Headers, "application/json"))
        {
            AnsiConsole.MarkupLine($"[red]Please review your credentials, Unexpected response from POST/PUT spaces API for name: {name}, id:{id}[/]");
        }
        else
        {
            AnsiConsole.WriteLine($"[red]StatusCode {apiResponse.StatusCode} returned from the POST spaces/{{id}}/apis for {name}[/]");
        }

        return new ApiResponse();
    }

    public async Task<bool> UpsertConnection(string exploreCookie, bool spaceExists, string spaceId, string apiId, string? connectionId, Connection? connection, bool? verboseOutput)
    {


        _httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        _httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");

        var connectionContent = new StringContent(JsonSerializer.Serialize(MappingHelper.MassageConnectionExportForImport(connection)), Encoding.UTF8, "application/json");

        HttpResponseMessage? connectionResponse;

        if (spaceExists && await CheckConnectionExists(exploreCookie, spaceId, apiId, connectionId, verboseOutput))
        {
            connectionResponse = await _httpClient.PutAsync($"/spaces/{spaceId}/apis/{apiId}/connections/{connectionId}", connectionContent);
        }
        else
        {
            connectionResponse = await _httpClient.PostAsync($"/spaces/{spaceId}/apis/{apiId}/connections", connectionContent);
        }

        if (connectionResponse.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            if (!UtilityHelper.IsContentTypeExpected(connectionResponse.Content.Headers, "application/hal+json") && !UtilityHelper.IsContentTypeExpected(connectionResponse.Content.Headers, "application/json"))
            {
                AnsiConsole.MarkupLine($"[red]Please review your credentials, Unexpected response from the connections API for api: {apiId} and {connection?.Name}[/]");
            }
            else
            {
                AnsiConsole.WriteLine($"[red]StatusCode {connectionResponse.StatusCode} returned from the connections API for api: {apiId} and {connection?.Name}[/]");

                var message = await connectionResponse.Content.ReadAsStringAsync();
                AnsiConsole.WriteLine($"error: {message}");
            }
        }
        return false;
    }

    public class CreateSpaceResult
    {
        public bool Result { get; set; }
        public Guid? Id { get; set; }
        public string? Reason { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }
    }

    public async Task<CreateSpaceResult> CreateSpace(string exploreCookie, string spaceName)
    {
        var cleanedCollectionName = UtilityHelper.CleanString(spaceName);
        var spaceContent = new StringContent(JsonSerializer.Serialize(new SpaceRequest() { Name = cleanedCollectionName }), Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        _httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");
        var spacesResponse = await _httpClient.PostAsync("/spaces", spaceContent);
        var spaceResponse = spacesResponse.Content.ReadFromJsonAsync<SpaceResponse>();
        switch (spacesResponse.StatusCode)
        {
            case HttpStatusCode.Created:
                var spaceId = spaceResponse.Result?.Id;
                return new CreateSpaceResult
                {
                    Id = new Guid(),
                    Result = true,
                    StatusCode = spacesResponse.StatusCode
                };
            case HttpStatusCode.OK:
                return new CreateSpaceResult
                {
                    Reason = "AUTH_REQUIRED",
                    Result = false,
                    StatusCode = spacesResponse.StatusCode
                };
            case HttpStatusCode.Conflict:
                return new CreateSpaceResult
                {
                    Reason = "SPACE_CONFLICT",
                    Result = false,
                    StatusCode = spacesResponse.StatusCode
                };

            default:
                return new CreateSpaceResult
                {
                    Reason = spacesResponse.ReasonPhrase,
                    Result = false,
                    StatusCode = spacesResponse.StatusCode
                };
        }
    }

    public class CreateApiEntryResult
    {
        public bool Result { get; set; }
        public Guid? Id { get; set; }
        public string? Reason { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }
    }

    public async Task<CreateApiEntryResult> CreateApiEntry(string exploreCookie, Guid? spaceId, string apiName, string importer, string? description)
    {

        var cleanedAPIName = UtilityHelper.CleanString(apiName);
        var apiContent = new StringContent(JsonSerializer.Serialize(new ApiRequest()
        {
            Name = cleanedAPIName,
            Type = "REST",
            Description = $"imported from {importer} on {DateTime.UtcNow.ToShortDateString()}\n{description}"
        }), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        _httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");
        var apiResponse = await _httpClient.PostAsync($"/spaces/{spaceId}/apis", apiContent);
        switch (apiResponse.StatusCode)
        {
            case HttpStatusCode.Created:
                var createdApiResponse = apiResponse.Content.ReadFromJsonAsync<ApiResponse>();
                var createdApiResponseId = createdApiResponse.Result?.Id;
                return new CreateApiEntryResult
                {
                    Result = true,
                    Id = createdApiResponseId,
                    StatusCode = apiResponse.StatusCode
                };

            default:
                return new CreateApiEntryResult
                {
                    Reason = apiResponse.StatusCode.ToString(),
                    Result = false,
                    StatusCode = apiResponse.StatusCode
                };
        }
    }
    public class CreateApiConnectionResult
    {
        public bool Result { get; set; }
        public Guid? Id { get; set; }
        public string? Reason { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }
    }

    public async Task<CreateApiConnectionResult> CreateApiConnection(string exploreCookie, Guid? spaceId, Guid? apiId, string connectionRequestBody)
    {
        var connectionContent = new StringContent(connectionRequestBody, Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        _httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");
        var connectionResponse = await _httpClient.PostAsync($"/spaces/{spaceId}/apis/{apiId}/connections", connectionContent);

        switch (connectionResponse.StatusCode)
        {
            case HttpStatusCode.Created:
                return new CreateApiConnectionResult
                {
                    Result = true,
                    StatusCode = connectionResponse.StatusCode
                };

            default:
                return new CreateApiConnectionResult
                {
                    Reason = "Connection NOT created",
                    Result = false,
                    StatusCode = connectionResponse.StatusCode
                };
        }
    }

    public class GetSpacesResult
    {
        public bool Result { get; set; }
        public PagedSpaces? Spaces { get; set; }
        public string? Reason { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }

    }

    public async Task<GetSpacesResult> GetSpaces(string exploreCookie)
    {

        var spacesResponse = await _httpClient.GetAsync("/spaces?page=0&size=2000");
        if (spacesResponse.StatusCode == HttpStatusCode.OK)
        {
            if (!UtilityHelper.IsContentTypeExpected(spacesResponse.Content.Headers, "application/hal+json"))
            {
                Console.WriteLine(spacesResponse);
                AnsiConsole.MarkupLine($"[red]Please review your credentials, Unexpected response GET spaces endpoint[/]");
                return new GetSpacesResult
                {
                    Result = false,
                    Reason = "AUTH_REQUIRED",
                    StatusCode = spacesResponse.StatusCode
                };
            }
            var spaces = await spacesResponse.Content.ReadFromJsonAsync<PagedSpaces>();
            return new GetSpacesResult
            {
                Result = true,
                Spaces = spaces,
                StatusCode = spacesResponse.StatusCode
            };
        }
        else
        {
            return new GetSpacesResult
            {
                Result = false,
                Reason = spacesResponse.ReasonPhrase,
                StatusCode = spacesResponse.StatusCode
            };
        }
    }
    public class GetSpaceApisResult
    {
        public bool Result { get; set; }
        public PagedApis? Apis { get; set; }
        public string? Reason { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }

    }

    public async Task<GetSpaceApisResult> GetSpaceApis(string exploreCookie, Guid? spaceId)
    {

        var apisResponse = await _httpClient.GetAsync($"/spaces/{spaceId}/apis?page=0&size=2000");
        if (apisResponse.StatusCode == HttpStatusCode.OK)
        {
            var apis = await apisResponse.Content.ReadFromJsonAsync<PagedApis>();
            return new GetSpaceApisResult
            {
                Result = true,
                Apis = apis,
                StatusCode = apisResponse.StatusCode
            };
        }
        else
        {
            return new GetSpaceApisResult
            {
                Result = false,
                Reason = apisResponse.ReasonPhrase,
                StatusCode = apisResponse.StatusCode
            };
        }
    }
    public class GetApiConnectionsForSpaceResult
    {
        public bool Result { get; set; }
        public PagedConnections? Connections { get; set; }
        public string? Reason { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }

    }

    public async Task<GetApiConnectionsForSpaceResult> GetApiConnectionsForSpace(string exploreCookie, Guid? spaceId, Guid? apiId)
    {

        var connectionsResponse = await _httpClient.GetAsync($"/spaces/{spaceId}/apis/{apiId}/connections?page=0&size=2000");
        if (connectionsResponse.StatusCode == HttpStatusCode.OK)
        {
            var connections = await connectionsResponse.Content.ReadFromJsonAsync<PagedConnections>();
            return new GetApiConnectionsForSpaceResult
            {
                Result = true,
                Connections = connections,
                StatusCode = connectionsResponse.StatusCode
            };
        }
        else
        {
            return new GetApiConnectionsForSpaceResult
            {
                Result = false,
                Reason = connectionsResponse.ReasonPhrase,
                StatusCode = connectionsResponse.StatusCode
            };
        }
    }

}
