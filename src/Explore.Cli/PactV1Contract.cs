namespace PactV1Contract
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Schema for a Pact file
    /// </summary>
    public partial class Coordinate
    {
        [JsonProperty("consumer")]
        public Pacticipant Consumer { get; set; }

        [JsonProperty("interactions")]
        public Interaction[] Interactions { get; set; }

        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public Metadata Metadata { get; set; }

        [JsonProperty("provider")]
        public Pacticipant Provider { get; set; }
    }

    public partial class Pacticipant
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Interaction
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("provider_state", NullValueHandling = NullValueHandling.Ignore)]
        public string InteractionProviderState { get; set; }

        [JsonProperty("providerState", NullValueHandling = NullValueHandling.Ignore)]
        public string ProviderState { get; set; }

        [JsonProperty("request")]
        public Request Request { get; set; }

        [JsonProperty("response")]
        public Response Response { get; set; }
    }

    public partial class Request
    {
        [JsonProperty("body")]
        public object Body { get; set; }

        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Headers { get; set; }

        [JsonProperty("method")]
        public Method Method { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("query", NullValueHandling = NullValueHandling.Ignore)]
        public string Query { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("body")]
        public object Body { get; set; }

        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Headers { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }
    }

    public partial class Metadata
    {
        [JsonProperty("pact-specification", NullValueHandling = NullValueHandling.Ignore)]
        public PactSpecification PactSpecification { get; set; }

        [JsonProperty("pactSpecification", NullValueHandling = NullValueHandling.Ignore)]
        public PactSpecificationClass MetadataPactSpecification { get; set; }

        [JsonProperty("pactSpecificationVersion", NullValueHandling = NullValueHandling.Ignore)]
        public string PactSpecificationVersion { get; set; }
    }

    public partial class PactSpecificationClass
    {
        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public partial class PactSpecification
    {
        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public enum Method { Connect, Delete, Get, Head, MethodConnect, MethodDelete, MethodGet, MethodHead, MethodOptions, MethodPost, MethodPut, MethodTrace, Options, Post, Put, Trace };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                MethodConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class MethodConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Method) || t == typeof(Method?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "CONNECT":
                    return Method.MethodConnect;
                case "DELETE":
                    return Method.MethodDelete;
                case "GET":
                    return Method.MethodGet;
                case "HEAD":
                    return Method.MethodHead;
                case "OPTIONS":
                    return Method.MethodOptions;
                case "POST":
                    return Method.MethodPost;
                case "PUT":
                    return Method.MethodPut;
                case "TRACE":
                    return Method.MethodTrace;
                case "connect":
                    return Method.Connect;
                case "delete":
                    return Method.Delete;
                case "get":
                    return Method.Get;
                case "head":
                    return Method.Head;
                case "options":
                    return Method.Options;
                case "post":
                    return Method.Post;
                case "put":
                    return Method.Put;
                case "trace":
                    return Method.Trace;
            }
            throw new Exception("Cannot unmarshal type Method");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Method)untypedValue;
            switch (value)
            {
                case Method.MethodConnect:
                    serializer.Serialize(writer, "CONNECT");
                    return;
                case Method.MethodDelete:
                    serializer.Serialize(writer, "DELETE");
                    return;
                case Method.MethodGet:
                    serializer.Serialize(writer, "GET");
                    return;
                case Method.MethodHead:
                    serializer.Serialize(writer, "HEAD");
                    return;
                case Method.MethodOptions:
                    serializer.Serialize(writer, "OPTIONS");
                    return;
                case Method.MethodPost:
                    serializer.Serialize(writer, "POST");
                    return;
                case Method.MethodPut:
                    serializer.Serialize(writer, "PUT");
                    return;
                case Method.MethodTrace:
                    serializer.Serialize(writer, "TRACE");
                    return;
                case Method.Connect:
                    serializer.Serialize(writer, "connect");
                    return;
                case Method.Delete:
                    serializer.Serialize(writer, "delete");
                    return;
                case Method.Get:
                    serializer.Serialize(writer, "get");
                    return;
                case Method.Head:
                    serializer.Serialize(writer, "head");
                    return;
                case Method.Options:
                    serializer.Serialize(writer, "options");
                    return;
                case Method.Post:
                    serializer.Serialize(writer, "post");
                    return;
                case Method.Put:
                    serializer.Serialize(writer, "put");
                    return;
                case Method.Trace:
                    serializer.Serialize(writer, "trace");
                    return;
            }
            throw new Exception("Cannot marshal type Method");
        }

        public static readonly MethodConverter Singleton = new MethodConverter();
    }
}
