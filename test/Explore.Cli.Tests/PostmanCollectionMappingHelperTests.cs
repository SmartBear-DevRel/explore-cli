using Explore.Cli.Models.Explore;
using Explore.Cli.Models.Postman;
using System.Text.Json;

public class PostmanCollectionMappingHelperTests
{
    [Fact]
    public void MapEntryBodyToContentExamples_ShouldReturnExamples()
    {
        // Arrange
        var rawBody = "raw body";

        // Act
        var result = PostmanCollectionMappingHelper.MapEntryBodyToContentExamples(rawBody);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Examples>(result);
    }

    [Fact]
    public void CreatePathsDictionary_ShouldReturnDictionary()
    {
        // Arrange
        var mockRequestAsJson = "{\r\n\t\"method\": \"GET\",\r\n\t\"header\": [],\r\n\t\"body\": {\r\n\t\t\"mode\": \"formdata\",\r\n\t\t\"formdata\": []\r\n\t},\r\n\t\"url\": {\r\n\t\t\"raw\": \"http://localhost:17456/api/apilogs?start_date_time=2017-09-27%2010%3A20%3A00&end_date_time=2017-09-30%2010%3A20%3A00\",\r\n\t\t\"protocol\": \"http\",\r\n\t\t\"host\": [\r\n\t\t\t\"localhost\"\r\n\t\t],\r\n\t\t\"port\": \"17456\",\r\n\t\t\"path\": [\r\n\t\t\t\"api\",\r\n\t\t\t\"apilogs\"\r\n\t\t],\r\n\t\t\"query\": [\r\n\t\t\t{\r\n\t\t\t\t\"key\": \"start_date_time\",\r\n\t\t\t\t\"value\": \"2017-09-27%2010%3A20%3A00\"\r\n\t\t\t},\r\n\t\t\t{\r\n\t\t\t\t\"key\": \"end_date_time\",\r\n\t\t\t\t\"value\": \"2017-09-30%2010%3A20%3A00\"\r\n\t\t\t}\r\n\t\t]\r\n\t}\r\n}";
        Request? request = JsonSerializer.Deserialize<Request>(mockRequestAsJson);

        // Act
        var result = PostmanCollectionMappingHelper.CreatePathsDictionary(request);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Dictionary<string, object>>(result);
    }    

    [Fact]
    public void MapHeaderAndQueryParams_ShouldReturnListOfParameters()
    {
        // Arrange
        var mockRequestAsJson = "{\r\n\t\"method\": \"GET\",\r\n\t\"header\": [],\r\n\t\"body\": {\r\n\t\t\"mode\": \"formdata\",\r\n\t\t\"formdata\": []\r\n\t},\r\n\t\"url\": {\r\n\t\t\"raw\": \"http://localhost:17456/api/apilogs?start_date_time=2017-09-27%2010%3A20%3A00&end_date_time=2017-09-30%2010%3A20%3A00\",\r\n\t\t\"protocol\": \"http\",\r\n\t\t\"host\": [\r\n\t\t\t\"localhost\"\r\n\t\t],\r\n\t\t\"port\": \"17456\",\r\n\t\t\"path\": [\r\n\t\t\t\"api\",\r\n\t\t\t\"apilogs\"\r\n\t\t],\r\n\t\t\"query\": [\r\n\t\t\t{\r\n\t\t\t\t\"key\": \"start_date_time\",\r\n\t\t\t\t\"value\": \"2017-09-27%2010%3A20%3A00\"\r\n\t\t\t},\r\n\t\t\t{\r\n\t\t\t\t\"key\": \"end_date_time\",\r\n\t\t\t\t\"value\": \"2017-09-30%2010%3A20%3A00\"\r\n\t\t\t}\r\n\t\t]\r\n\t}\r\n}";
        Request? request = JsonSerializer.Deserialize<Request>(mockRequestAsJson);

        // Act
        var result = PostmanCollectionMappingHelper.MapHeaderAndQueryParams(request);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<Parameter>>(result);
    }    

    [Fact]
    public void GetServerUrlFromItemRequest_ShouldReturnUrl()
    {
        // Arrange
        var mockRequestAsJson = "{\r\n\t\"method\": \"GET\",\r\n\t\"header\": [],\r\n\t\"body\": {\r\n\t\t\"mode\": \"formdata\",\r\n\t\t\"formdata\": []\r\n\t},\r\n\t\"url\": {\r\n\t\t\"raw\": \"http://localhost:17456/api/apilogs?start_date_time=2017-09-27%2010%3A20%3A00&end_date_time=2017-09-30%2010%3A20%3A00\",\r\n\t\t\"protocol\": \"http\",\r\n\t\t\"host\": [\r\n\t\t\t\"localhost\"\r\n\t\t],\r\n\t\t\"port\": \"17456\",\r\n\t\t\"path\": [\r\n\t\t\t\"api\",\r\n\t\t\t\"apilogs\"\r\n\t\t],\r\n\t\t\"query\": [\r\n\t\t\t{\r\n\t\t\t\t\"key\": \"start_date_time\",\r\n\t\t\t\t\"value\": \"2017-09-27%2010%3A20%3A00\"\r\n\t\t\t},\r\n\t\t\t{\r\n\t\t\t\t\"key\": \"end_date_time\",\r\n\t\t\t\t\"value\": \"2017-09-30%2010%3A20%3A00\"\r\n\t\t\t}\r\n\t\t]\r\n\t}\r\n}";
        Request? request = JsonSerializer.Deserialize<Request>(mockRequestAsJson);

        // Act
        var result = PostmanCollectionMappingHelper.GetServerUrlFromItemRequest(request);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public void MapPostmanCollectionItemToExploreConnection_ShouldReturnConnection()
    {
        // Arrange
        var mockItemAsJson = "{\r\n\t\"name\": \"GET apilogs\",\r\n\t\"request\": {\r\n\t\t\"method\": \"GET\",\r\n\t\t\"header\": [],\r\n\t\t\"body\": {\r\n\t\t\t\"mode\": \"formdata\",\r\n\t\t\t\"formdata\": []\r\n\t\t},\r\n\t\t\"url\": {\r\n\t\t\t\"raw\": \"http://localhost:17456/api/apilogs?start_date_time=2017-09-27%2010%3A20%3A00&end_date_time=2017-09-30%2010%3A20%3A00\",\r\n\t\t\t\"protocol\": \"http\",\r\n\t\t\t\"host\": [\r\n\t\t\t\t\"localhost\"\r\n\t\t\t],\r\n\t\t\t\"port\": \"17456\",\r\n\t\t\t\"path\": [\r\n\t\t\t\t\"api\",\r\n\t\t\t\t\"apilogs\"\r\n\t\t\t],\r\n\t\t\t\"query\": [\r\n\t\t\t\t{\r\n\t\t\t\t\t\"key\": \"start_date_time\",\r\n\t\t\t\t\t\"value\": \"2017-09-27%2010%3A20%3A00\"\r\n\t\t\t\t},\r\n\t\t\t\t{\r\n\t\t\t\t\t\"key\": \"end_date_time\",\r\n\t\t\t\t\t\"value\": \"2017-09-30%2010%3A20%3A00\"\r\n\t\t\t\t}\r\n\t\t\t]\r\n\t\t}\r\n\t}\r\n}";
        Item? item = JsonSerializer.Deserialize<Item>(mockItemAsJson);

        // Act
        var result = PostmanCollectionMappingHelper.MapPostmanCollectionItemToExploreConnection(item ?? new Item());

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Connection>(result);
    }

    [Fact]
    public void IsCollectionVersion2_1_ShouldReturnTrue()
    {
        // Arrange
        var mockCollectionAsJson = "{\r\n\t\"info\": {\r\n\t\t\"_postman_id\": \"d5e3f3a0-5f1e-4b0e-8f2a-2b1b8b7c9b1a\",\r\n\t\t\"name\": \"Explore\",\r\n\t\t\"schema\": \"https://schema.getpostman.com/json/collection/v2.1.0/collection.json\"\r\n\t}\r\n}";

        // Act
        var result = PostmanCollectionMappingHelper.IsCollectionVersion2_1(mockCollectionAsJson);

        // Assert
        Assert.True(result);
    }
    [Fact]
    public void IsCollectionVersion2_1_ShouldReturnFalse()
    {
        // Arrange
        
        var mockCollectionAsJson2 = "{\r\n\t\"info\": {\r\n\t\t\"_postman_id\": \"d5e3f3a0-5f1e-4b0e-8f2a-2b1b8b7c9b1a\",\r\n\t\t\"name\": \"Explore\",\r\n\t\t\"schema\": \"https://schema.getpostman.com/json/collection/v2.0.0/collection.json\"\r\n\t}\r\n}";
        var mockCollectionAsJson3 = "{\r\n\t\"info\": {\r\n\t\t\"_postman_id\": \"d5e3f3a0-5f1e-4b0e-8f2a-2b1b8b7c9b1a\",\r\n\t\t\"name\": \"Explore\",\r\n\t\t\"schema\": \"https://schema.getpostman.com/json/collection/v2.2.0/collection.json\"\r\n\t}\r\n}";

        // Act
        
        var result2 = PostmanCollectionMappingHelper.IsCollectionVersion2_1(mockCollectionAsJson2);
        var result3 = PostmanCollectionMappingHelper.IsCollectionVersion2_1(mockCollectionAsJson3);

        // Assert
        
        Assert.False(result2);
        Assert.False(result3);
    }   

    [Fact]
    public void ProcessesDescriptions()
    {
        // Arrange
        var filePath = "../../../fixtures/API_.documentation_postman_collection.json";
        var mockCollectionAsJson = File.ReadAllText(filePath);
        var postmanCollection = JsonSerializer.Deserialize<PostmanCollection>(mockCollectionAsJson);
        // Act
        Assert.Equal("Get authenticated user", postmanCollection?.Item?[0].ItemList?[0].Name);
        Assert.Equal("GET", postmanCollection?.Item?[0].ItemList?[0].Request?.Method?.ToString());
        Assert.Equal("Gets information about the authenticated user.", postmanCollection?.Item?[0].ItemList?[0].Request?.Description?.Content?.ToString());
    }
}