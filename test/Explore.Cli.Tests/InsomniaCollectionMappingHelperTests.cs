using Explore.Cli.Models.Explore;
using Explore.Cli.Models.Insomnia;
using System.Text.Json;

public class InsomniaCollectionMappingHelperTests
{
    [Fact]
    public void MapEntryBodyToContentExamples_ShouldReturnExamples()
    {
        // Arrange
        var rawBody = "raw body";

        // Act
        var result = InsomniaCollectionMappingHelper.MapEntryBodyToContentExamples(rawBody);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Examples>(result);
    }

    [Fact]
    public void ReplaceEnvironmentVariables_ShouldReplaceSingleVariableWithValue()
    {
        // Arrange
        var environmentResources = new List<Resource>()
        {
            new Resource()
            {
                Name = "Environment",
                Data = new Dictionary<string, string>()
                {
                    { "baseUrl", "http://localhost:17456" }
                }
            }
        };

        var url = "{{ _.baseUrl }}/api/logs";

        // Act
        var result = InsomniaCollectionMappingHelper.ReplaceEnvironmentVariables(url, environmentResources);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("http://localhost:17456/api/logs", result);
    }

    [Fact]
    public void ReplaceEnvironmentVariables_ShouldReplaceMultipleVariablesWithValues()
    {
        // Arrange
        var environmentResources = new List<Resource>()
        {
            new Resource()
            {
                Name = "Environment",
                Data = new Dictionary<string, string>
                {           
                    { "baseUrl", "http://localhost:17456" },
                    { "apiVersion", "v1" }
                }
            }
        };

        var url = "{{ _.baseUrl }}/api/{{ _.apiVersion }}/logs";

        // Act
        var result = InsomniaCollectionMappingHelper.ReplaceEnvironmentVariables(url, environmentResources);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("http://localhost:17456/api/v1/logs", result);
    }   

    [Fact]
    public void CreatePathsDictionary_ShouldReturnDictionary()
    {
        // Arrange
        var resource = new Resource()
        {
            Url = "http://localhost:17456/api/apilogs?start_date_time=2017-09-27%2010%3A20%3A00&end_date_time=2017-09-30%2010%3A20%3A00"
        };

        var environmentResources = new List<Resource>();

        // Act
        var result = InsomniaCollectionMappingHelper.CreatePathsDictionary(resource, environmentResources);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Dictionary<string, object>>(result);
    }     

    [Fact]
    public void MapHeaderAndQueryParams_ShouldReturnListOfParameters()
    {
        // Arrange
        var resource = new Resource()
        {
            Url = "http://localhost:17456/api/apilogs?start_date_time=2017-09-27%2010%3A20%3A00&end_date_time=2017-09-30%2010%3A20%3A00",
            Parameters = new List<Explore.Cli.Models.Insomnia.Parameter>()
            {
                new Explore.Cli.Models.Insomnia.Parameter()
                {
                    Name = "sort",
                    Value = "desc"
                },
                new Explore.Cli.Models.Insomnia.Parameter()
                {
                    Name = "groupBy",
                    Value = "severity"
                }
            }
        };

        var environmentResources = new List<Resource>();

        // Act
        var result = InsomniaCollectionMappingHelper.MapHeaderAndQueryParams(resource, environmentResources);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<Explore.Cli.Models.Explore.Parameter>>(result);
        Assert.Equal(4, result.Count);
    }    

    [Fact]
    public void MapInsomniaRequestResourceToExploreConnection_ShouldReturnConnection()
    {
        // Arrange
        var resource = new Resource()
        {
            Url = "http://localhost:17456/api/apilogs?start_date_time=2017-09-27%2010%3A20%3A00&end_date_time=2017-09-30%2010%3A20%3A00",
            Method = "GET",
            Body = new Explore.Cli.Models.Insomnia.Body()
            {
                MimeType = "application/json"
            }
        };

        var environmentResources = new List<Resource>();

        // Act
        var result = InsomniaCollectionMappingHelper.MapInsomniaRequestResourceToExploreConnection(resource, environmentResources);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Connection>(result);
    }

    [Fact]
    public void IsCollectionExportVersion4_ShouldReturnTrue()
    {
        // Arrange
        var json = "{\"__export_format\": \"4\"}";

        // Act
        var result = InsomniaCollectionMappingHelper.IsCollectionExportVersion4(json);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsCollectionExportVersion4_ShouldReturnFalse()
    {
        // Arrange
        var json = "{\"__export_format\": \"3\"}";

        // Act
        var result = InsomniaCollectionMappingHelper.IsCollectionExportVersion4(json);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsItemRequestModeSupported_ShouldReturnTrue()
    {
        // Arrange
        var resource = new Resource()
        {
            Method = "GET"
        };

        // Act
        var result = InsomniaCollectionMappingHelper.IsItemRequestModeSupported(resource);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsItemRequestModeSupported_ForPOSTwithJSONBody_ShouldReturnTrue()    
    {
        // Arrange
        var resource = new Resource()
        {
            Method = "POST",
            Body = new Explore.Cli.Models.Insomnia.Body()
            {
                MimeType = "application/json"
            }
        };

        // Act
        var result = InsomniaCollectionMappingHelper.IsItemRequestModeSupported(resource);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsItemRequestModeSupported_ShouldReturnFalse()
    {
        // Arrange
        var resource = new Resource()
        {
            Method = "POST",
            Body = new Explore.Cli.Models.Insomnia.Body()
            {
                MimeType = "application/xml"
            }
        };

        // Act
        var result = InsomniaCollectionMappingHelper.IsItemRequestModeSupported(resource);

        // Assert
        Assert.False(result);
    }


    [Fact]
    public void ParseUrl_ShouldReturnBaseUrl()
    {
        // Arrange
        var url = "http://localhost:17456/api/apilogs?start_date_time=2017-09-27%2010%3A20%3A00&end_date_time=2017-09-30%2010%3A20%3A00";
        var environmentResources = new List<Resource>();

        // Act
        var result = InsomniaCollectionMappingHelper.ParseUrl(url, environmentResources);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("http://localhost:17456", result);
    }

    [Fact]
    public void ParseUrl_ShouldReplaceEnvironmentVariablesAndReturnBaseUrl()
    {
        // Arrange
        var url = "{{ _.baseUrl }}/api/apilogs?start_date_time=2017-09-27%2010%3A20%3A00&end_date_time=2017-09-30%2010%3A20%3A00";
        var environmentResources = new List<Resource>()
        {
            new Resource()
            {
                Name = "Environment",
                Data = new Dictionary<string, string>()
                {
                    { "baseUrl", "http://localhost:17456" }
                }
            }
        };

        // Act
        var result = InsomniaCollectionMappingHelper.ParseUrl(url, environmentResources);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("http://localhost:17456", result);
    }
}