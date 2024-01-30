#nullable enable

using System.Text.Json.Serialization;

public class PostmanCollection
{
    [JsonPropertyName("info")]
    public PostmanCollectionInfo? Info { get; set; }

    [JsonPropertyName("item")]
    public List<Item>? Item { get; set; }
}

public partial class PostmanCollectionInfo
{
    [JsonPropertyName("_postman_id")]
    public string? PostmanId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("schema")]
    public string? Schema { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class Item
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("request")]
    public Request? Request { get; set; }

    [JsonPropertyName("item")]
    public List<Item>? ItemList { get; set; }

    //[JsonPropertyName("event")]
    //public Event[]? Event { get; set; }
}

public class Request
{
    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("header")]
    public List<Header>? Header { get; set; }

    [JsonPropertyName("body")]
    public Body? Body { get; set; }

    [JsonPropertyName("url")]
    public Url? Url { get; set; }

    [JsonPropertyName("description")]
    public Description? Description { get; set; }
}

public class Header
{
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class Body
{
    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    [JsonPropertyName("raw")]
    public string? Raw { get; set; }

    [JsonPropertyName("formdata")]
    public List<Formdata>? Formdata { get; set; }

    [JsonPropertyName("urlencoded")]
    public List<Urlencoded>? Urlencoded { get; set; }

    [JsonPropertyName("graphql")]
    public Graphql? GraphQL { get; set; }
}

public class Formdata
{
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class Urlencoded
{
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class Graphql
{
    [JsonPropertyName("query")]
    public string? Query { get; set; }

    [JsonPropertyName("variables")]
    public string? Variables { get; set; }

    [JsonPropertyName("operationName")]
    public string? OperationName { get; set; }
}

public class Url
{
    [JsonPropertyName("raw")]
    public string? Raw { get; set; }

    [JsonPropertyName("protocol")]
    public string? Protocol { get; set; }

    [JsonPropertyName("host")]
    public List<string>? Host { get; set; }

    [JsonPropertyName("port")]
    public string? Port { get; set; }

    [JsonPropertyName("path")]
    public List<string>? Path { get; set; }

    [JsonPropertyName("query")]
    public List<Query>? Query { get; set; }
}

public class Query
{
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

public class Description
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
