using System.Text.Json.Serialization;

namespace Explore.Cli.Models.Insomnia;

public class Resource
{
    // common properties
    [JsonPropertyName("_id")]
    public string? Id { get; set; }

    [JsonPropertyName("parentId")]
    public string? ParentId { get; set; }

    [JsonPropertyName("_type")]
    public string? Type { get; set; }

    // common for request, environment, workspace
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    // workspace specific properties
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    // request specific properties

    [JsonPropertyName("url")]
    public string? Url { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("body")]
    public Body? Body { get; set; }

    [JsonPropertyName("parameters")]
    public List<Parameter>? Parameters { get; set; }

    [JsonPropertyName("headers")]
    public List<Header>? Headers { get; set; }

    [JsonPropertyName("authentication")]
    public Authentication? Authentication { get; set; }

    // environment specific properties
    [JsonPropertyName("data")]
    public Dictionary<string, string>? Data { get; set; }

    // api spec specific properties
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }

    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }
}

public class Body
{
    [JsonPropertyName("mimeType")]
    public string? MimeType { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("params")]
    public List<Param>? Params { get; set; }
}

public class Param
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }
}
public class Parameter
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }
}

public class Header
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }    
}

public class Authentication
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }

    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("disabled")]
    public bool? Disabled { get; set; }
}