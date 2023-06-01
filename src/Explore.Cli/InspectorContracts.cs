using System;
using System.Collections.Generic;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

public class InspectorCollection
{
    [JsonPropertyName("_id")]
    public Id? Id { get; set; }

    [JsonPropertyName("modelClass")]
    public string? ModelClass { get; set; }

    [JsonPropertyName("collectionId")]
    public Guid CollectionId { get; set; }

    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("collectionEntries")]
    public List<CollectionEntry>? CollectionEntries { get; set; }
}
public class CollectionEntry
{
    [JsonPropertyName("_id")]
    public Id? Id { get; set; }

    [JsonPropertyName("modelClass")]
    public string? ModelClass { get; set; }

    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("endpoint")]
    public Uri? Endpoint { get; set; }

    [JsonPropertyName("uri")]
    public UriClass? Uri { get; set; }

    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("authentication")]
    public string? Authentication { get; set; }

    [JsonPropertyName("headers")]
    public List<Header>? Headers { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("entryId")]
    public Guid EntryId { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("ciHost")]
    public Uri? CiHost { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class Header
{
    [JsonPropertyName("modelClass")]
    public string? ModelClass { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

public class Id
{
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("counter")]
    public long Counter { get; set; }

    [JsonPropertyName("time")]
    public long Time { get; set; }

    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("machineIdentifier")]
    public long MachineIdentifier { get; set; }

    [JsonPropertyName("processIdentifier")]
    public long ProcessIdentifier { get; set; }

    [JsonPropertyName("timeSecond")]
    public long TimeSecond { get; set; }
}

public class UriClass
{
    [JsonPropertyName("modelClass")]
    public string? ModelClass { get; set; }

    [JsonPropertyName("scheme")]
    public string? Scheme { get; set; }

    [JsonPropertyName("host")]
    public string? Host { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("query")]
    public string? Query { get; set; }
}