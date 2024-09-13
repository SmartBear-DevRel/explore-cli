using System.Text.Json.Serialization;

namespace Explore.Cli.Models.Explore;

public partial class Transaction
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("connection")]
    public Connection? Connection { get; set; }
}

public partial class Connection
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

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

    [JsonPropertyName("paths")]
    public Dictionary<string, object>? Paths {get; set;}

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
    [JsonPropertyName("openapi")]
    public string? OpenApi { get; set; }

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

    [JsonPropertyName("schema")]
    public Schema? Schema { get; set; }

    [JsonPropertyName("examples")]
    public Examples? Examples { get; set; }


}

public class Schema
{
    [JsonPropertyName("type")]
    public string? type { get; set; }
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
    //[RegularExpression(@"^(http(s):\/\/.)[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)$")]
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

    }

    public partial class Apis
    {
        //[RegularExpression(@"^(http(s):\/\/.)[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)$")]
        [JsonPropertyName("href")]
        public Uri? Href { get; set; }
    }

public class ApiRequest
{
    [JsonRequired]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonRequired]
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }      

    [JsonPropertyName("servers")]
    public List<Server>? Servers { get; set; } 
}

    public partial class ApiResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonRequired]
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonRequired]
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }      

        [JsonPropertyName("servers")]
        public List<Server>? Servers { get; set; } 
    }

public partial class PagedSpaces
{
    [JsonPropertyName("_embedded")]
    public  EmbeddedSpaces? Embedded { get; set; }
}

public partial class EmbeddedSpaces 
{
    [JsonPropertyName("spaces")]
    public  List<SpaceResponse>? Spaces { get; set; }
}

public partial class PagedApis
{
    [JsonPropertyName("_embedded")]
    public  EmbeddedApis? Embedded { get; set; }
}

public partial class EmbeddedApis
{
    [JsonPropertyName("apis")]
    public  List<ApiResponse>? Apis { get; set; }
}

public partial class PagedConnections
{
    [JsonPropertyName("_embedded")]
    public  EmbeddedConnections? Embedded { get; set; }
}

public partial class EmbeddedConnections
{
    [JsonPropertyName("connections")]
    public  List<Connection>? Connections { get; set; }
}
