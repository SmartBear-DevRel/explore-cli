using Explore.Cli.Models;
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

}