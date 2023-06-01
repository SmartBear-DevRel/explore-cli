namespace Explore.Cli.Models;
using System.Text.Json.Serialization;



public partial class Transaction
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("connection")]
    public Connection? Connection { get; set; }
}

public partial class Connection
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("schema")]
    public string? Schema { get; set; } //OpenAPI, AsyncAPI, Internal

    [JsonPropertyName("schemaVersion")]
    public string? SchemaVersion { get; set; } //3.0.3

    [JsonPropertyName("connectionDefinition")]
    public ConnectionDefinition? ConnectionDefinition { get; set; }

    [JsonPropertyName("settings")]
    public Settings? Settings { get; set; }

    [JsonPropertyName("credentials")]
    public Credentials? Credentials { get; set; }    
}

public partial class Components
{
    [JsonPropertyName("securitySchemes")]
    public SecuritySchemes? SecuritySchemes { get; set; }    
}

public partial class SecuritySchemes
{
    [JsonPropertyName("BasicAuthCredentials")]
    public SecuritySchemeBasicAuthCredentials? BasicAuthCredentials { get; set; } 

    [JsonPropertyName("TokenCredentials")]
    public SecuritySchemeTokenCredentials? TokenCredentials { get; set; }      
}

public partial class SecuritySchemeTokenCredentials
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("scheme")]
    public string? Scheme { get; set; }    
}

public partial class SecuritySchemeBasicAuthCredentials
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("scheme")]
    public string? Scheme { get; set; }    
}

public partial class UsernamePasswordCredentials
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }    
}

public partial class BasicAuthCredentials
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }    
}

public partial class SaslPlainSslCredentials
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }    
}

public partial class SaslPlainCredentials
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }    
}

public partial class TokenCredentials
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }
}

public partial class ApiKeyCredentials
{
    [JsonPropertyName("apiKey")]
    public string? ApiKey { get; set; }
}


public partial class Credentials
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("apiKey")]
    public string? ApiKey { get; set; }    

    [JsonPropertyName("token")]
    public string? Token { get; set; }    

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }     
}

public partial class ConnectionDefinition
{
    //[JsonPropertyName("openapi")]
    //public string Openapi { get; set; }

    [JsonPropertyName("servers")]
    public List<Server>? Servers { get; set; }

    [JsonPropertyName("paths")]
    public Dictionary<string, object>? Paths { get; set; }
}

public class PathsContent
{
    [JsonPropertyName("parameters")]
    public List<Parameter>? Parameters { get; set; }

    [JsonPropertyName("requestBody")]
    public RequestBody? RequestBody { get; set; }

}

public class RequestBody
{
    [JsonPropertyName("content")]
    public Dictionary<string, object>? Content { get; set; }
}


public class Parameter
{
    [JsonPropertyName("in")]
    public string? In { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }    

    [JsonPropertyName("examples")]
    public Examples? Examples { get; set; }


}

public class Examples
{
    [JsonPropertyName("example")]
    public Example? Example { get; set; }        
}

public class Example
{
    [JsonPropertyName("value")]
    public string? Value { get; set; }        
}

public partial class Server
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class Settings
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("connectTimeout")]
    public long ConnectTimeout { get; set; }

    [JsonPropertyName("followRedirects")]
    public bool FollowRedirects { get; set; }

    [JsonPropertyName("encodeUrl")]
    public bool EncodeUrl { get; set; }
}


public class SpaceRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

    public partial class SpaceResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("_links")]
        public Links? Links { get; set; }
    }

    public partial class Links
    {
        [JsonPropertyName("self")]
        public Apis? Self { get; set; }

        [JsonPropertyName("apis")]
        public Apis? Apis { get; set; }
    }

    public partial class Apis
    {
        [JsonPropertyName("href")]
        public Uri? Href { get; set; }
    }

public class ApiRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }       
}

    public partial class ApiResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

    }

