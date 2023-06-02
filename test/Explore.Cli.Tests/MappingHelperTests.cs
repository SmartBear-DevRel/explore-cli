using System.Text.Json;
using Explore.Cli.Models;

namespace Explore.Cli.Tests;

public class MappingHelperTests
{
    [Fact]
    public void CollectionEntriesNotLimitedToSoap_ShouldReturnFalseForNull()
    {
        bool expected = false;
        var actual = MappingHelper.CollectionEntriesNotLimitedToSoap(null);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CollectionEntriesNotLimitedToSoap_FalseWhenOnlySoap()
    {
        bool expected = false;

        List<CollectionEntry> setupCollection = new List<CollectionEntry>()
        {
            new CollectionEntry()
            {
                Type = "SOAP"
            }
        };

        var actual = MappingHelper.CollectionEntriesNotLimitedToSoap(setupCollection);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CollectionEntriesNotLimitedToSoap_FalseWhenOnlyWSDL()
    {
        bool expected = false;

        List<CollectionEntry> setupCollection = new List<CollectionEntry>()
        {
            new CollectionEntry()
            {
                Type = "WSDL"
            }
        };

        var actual = MappingHelper.CollectionEntriesNotLimitedToSoap(setupCollection);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CollectionEntriesNotLimitedToSoap_TrueWhenOther()
    {
        bool expected = true;

        List<CollectionEntry> setupCollection = new List<CollectionEntry>()
        {
            new CollectionEntry()
            {
                Type = "OTHER"
            }
        };

        var actual = MappingHelper.CollectionEntriesNotLimitedToSoap(setupCollection);

        Assert.Equal(expected, actual);
    } 

    [Theory]
    [InlineData("XSRF-TOKEN=be05885a-41fc-4820-83fb-5db17015ed4a", "be05885a-41fc-4820-83fb-5db17015ed4a")]
    [InlineData("xsrf-token=dd3424c9-17ec-4b20-a89c-ca89d98bbd3b", "dd3424c9-17ec-4b20-a89c-ca89d98bbd3b")]
    [InlineData("bf936dc3-6c70-43a0-a4c5-ddb42569a9c8", null)]
    public void ExtractXSRFTokenFromCookie_Tests(string cookie, string expected)
    {
        var actual = MappingHelper.ExtractXSRFTokenFromCookie(cookie);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MapEntryBodyToContentExamples_ShouldMapJson()
    {
        var expected = "{\n    \"owner\": \"frank-kilcommins\",\n    \"name\": \"Common Domains\",\n    \"description\": \"common components for all APIs\",\n    \"apis\": [],\n    \"domains\": [\n        \"Problem\",\n        \"ErrorResponses\"\n    ]\n}";
        var actual = MappingHelper.MapEntryBodyToContentExamples(expected);

        Assert.Equal(expected, actual.Example?.Value);
    }

    [Fact]
    public void MapEntryBodyToContentExamples_ShouldMapXML()
    {
        var expected = "<soap:Envelope\n    xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\"\n    xmlns:tem=\"http://tempuri.org/\">\n    <soap:Header/>\n    <soap:Body>\n        <tem:Multiply>\n            <tem:intA>10</tem:intA>\n            <tem:intB>10</tem:intB>\n        </tem:Multiply>\n    </soap:Body>\n</soap:Envelope>";
        var actual = MappingHelper.MapEntryBodyToContentExamples(expected);

        Assert.Equal(expected, actual.Example?.Value);
    }

    [Fact]
    public void MapInspectorParamsToExploreParams_ShouldMapHeadersParams()
    {
        Models.Parameter expectedHeader = new Models.Parameter()
        {
            In = "header",
            Name = "Authorization",
            Examples = new Models.Examples()
            {
                Example = new Models.Example()
                {
                    Value = "0648454d-8307-40c9-b0ac-73fa4a48354e"
                }
            }
        };

        var entryAsJsonString = @"      {
        ""_id"": {
          ""timestamp"": 1684423109,
          ""counter"": 13169316,
          ""time"": 1684423109000,
          ""date"": ""2023-05-18T15:18:29Z"",
          ""machineIdentifier"": 8669634,
          ""processIdentifier"": 30503,
          ""timeSecond"": 1684423109
        },
        ""modelClass"": ""com.smartbear.readyapi.inspector.services.repository.models.HistoryEntry"",
        ""userId"": ""59bff4e3-89be-4078-bf9c-76cff2b9f2dc"",
        ""endpoint"": ""https://sbdevrel-fua-smartbearcoin-prd.azurewebsites.net/api/payees?country_of_registration=IE&name=ltd"",
        ""uri"": {
          ""modelClass"": ""com.smartbear.readyapi.inspector.services.repository.models.URI"",
          ""scheme"": ""https"",
          ""host"": ""sbdevrel-fua-smartbearcoin-prd.azurewebsites.net"",
          ""path"": ""/api/payees"",
          ""query"": ""country_of_registration=IE&name=ltd""
        },
        ""method"": ""GET"",
        ""body"": ""{\n    \""owner\"": \""frank-kilcommins\"",\n    \""name\"": \""Common Domains\"",\n    \""description\"": \""common components for all APIs\"",\n    \""apis\"": [],\n    \""domains\"": [\n        \""Problem\"",\n        \""ErrorResponses\""\n    ]\n}"",
        ""authentication"": """",
        ""headers"": [
          {
            ""modelClass"": ""com.smartbear.readyapi.inspector.services.repository.models.HeaderWithValue"",
            ""name"": ""Authorization"",
            ""value"": ""0648454d-8307-40c9-b0ac-73fa4a48354e""
          }
        ],
        ""type"": ""OTHER"",
        ""entryId"": ""f5f0874f-d656-4b3f-9707-b879fadd7e8c"",
        ""timestamp"": ""2023-05-18T15:18:37Z"",
        ""ciHost"": ""https://sbdevrel-fua-smartbearcoin-prd.azurewebsites.net"",
        ""name"": ""FinTech Workshop Request 0""
      }";

        CollectionEntry? entry = JsonSerializer.Deserialize<CollectionEntry>(entryAsJsonString);
        var result = MappingHelper.MapInspectorParamsToExploreParams(entry == null ? new CollectionEntry(){} : entry);

        Assert.Equal(1, result.Count(x => x.Name?.ToLowerInvariant() == "authorization"));
        Assert.Equal(1, result.Count(x => x.In == expectedHeader.In && x.Name == expectedHeader.Name && x.Examples?.Example?.Value == expectedHeader.Examples.Example.Value));      

    }

    [Fact]
    public void MapInspectorParamsToExploreParams_ShouldMapQueryParams()
    {
        Models.Parameter expectedQuery = new Models.Parameter()
        {
            In = "query",
            Name = "name",
            Examples = new Models.Examples()
            {
                Example = new Models.Example()
                {
                    Value = "ltd"
                }
            }
        };

        var entryAsJsonString = @"      {
        ""_id"": {
          ""timestamp"": 1684423109,
          ""counter"": 13169316,
          ""time"": 1684423109000,
          ""date"": ""2023-05-18T15:18:29Z"",
          ""machineIdentifier"": 8669634,
          ""processIdentifier"": 30503,
          ""timeSecond"": 1684423109
        },
        ""modelClass"": ""com.smartbear.readyapi.inspector.services.repository.models.HistoryEntry"",
        ""userId"": ""59bff4e3-89be-4078-bf9c-76cff2b9f2dc"",
        ""endpoint"": ""https://sbdevrel-fua-smartbearcoin-prd.azurewebsites.net/api/payees?country_of_registration=IE&name=ltd"",
        ""uri"": {
          ""modelClass"": ""com.smartbear.readyapi.inspector.services.repository.models.URI"",
          ""scheme"": ""https"",
          ""host"": ""sbdevrel-fua-smartbearcoin-prd.azurewebsites.net"",
          ""path"": ""/api/payees"",
          ""query"": ""country_of_registration=IE&name=ltd""
        },
        ""method"": ""GET"",
        ""body"": ""{\n    \""owner\"": \""frank-kilcommins\"",\n    \""name\"": \""Common Domains\"",\n    \""description\"": \""common components for all APIs\"",\n    \""apis\"": [],\n    \""domains\"": [\n        \""Problem\"",\n        \""ErrorResponses\""\n    ]\n}"",
        ""authentication"": """",
        ""headers"": [
          {
            ""modelClass"": ""com.smartbear.readyapi.inspector.services.repository.models.HeaderWithValue"",
            ""name"": ""Authorization"",
            ""value"": ""0648454d-8307-40c9-b0ac-73fa4a48354e""
          }
        ],
        ""type"": ""OTHER"",
        ""entryId"": ""f5f0874f-d656-4b3f-9707-b879fadd7e8c"",
        ""timestamp"": ""2023-05-18T15:18:37Z"",
        ""ciHost"": ""https://sbdevrel-fua-smartbearcoin-prd.azurewebsites.net"",
        ""name"": ""FinTech Workshop Request 0""
      }";

        CollectionEntry? entry = JsonSerializer.Deserialize<CollectionEntry>(entryAsJsonString);
        var result = MappingHelper.MapInspectorParamsToExploreParams(entry == null ? new CollectionEntry(){} : entry);

        Assert.Equal(1, result.Count(x => x.Name?.ToLowerInvariant() == "name"));
        Assert.Equal(1, result.Count(x => x.In == expectedQuery.In && x.Name == expectedQuery.Name && x.Examples?.Example?.Value == expectedQuery.Examples.Example.Value));      

    }    

    [Fact]
    public void MapInspectorAuthenticationToCredentials_BasicAuth()
    {
        var input = "Basic Authentication/username:password";
        var expected = new Credentials() { Type = "BasicAuthCredentials", Username = "username", Password = "password" };
        var actual = MappingHelper.MapInspectorAuthenticationToCredentials(input);

        Assert.Equal(expected.Type, actual?.Type);
        Assert.Equal(expected.Username, actual?.Username);
        Assert.Equal(expected.Password, actual?.Password);
    }

    [Fact]
    public void MapInspectorAuthenticationToCredentials_TokenAuth()
    {
        var input = "OAuth 2.0/0648454d-8307-40c9-b0ac-73fa4a48354e";
        var expected = new Credentials() { Type = "TokenCredentials", Token = "0648454d-8307-40c9-b0ac-73fa4a48354e"};
        var actual = MappingHelper.MapInspectorAuthenticationToCredentials(input);

        Assert.Equal(expected.Type, actual?.Type);
        Assert.Equal(expected.Token, actual?.Token);
    }    
}