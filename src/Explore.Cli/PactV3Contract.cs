namespace PactV3Contract
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

        [JsonProperty("messages")]
        public Message[] Messages { get; set; }

        [JsonProperty("metadata")]
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

        [JsonProperty("providerStates")]
        public ProviderStates? ProviderStates { get; set; }

        [JsonProperty("request")]
        public Request Request { get; set; }

        [JsonProperty("response")]
        public Response Response { get; set; }
    }

    public partial class ProviderState
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("params")]
        public Dictionary<string, object> Params { get; set; }
    }

    public partial class Request
    {
        [JsonProperty("body")]
        public object Body { get; set; }

        [JsonProperty("generators")]
        public RequestGenerators Generators { get; set; }

        [JsonProperty("headers")]
        public Dictionary<string, object> Headers { get; set; }

        [JsonProperty("matchingRules")]
        public RequestMatchingRules MatchingRules { get; set; }

        [JsonProperty("method")]
        public Method Method { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("query")]
        public Dictionary<string, object> Query { get; set; }
    }

    public partial class RequestGenerators
    {
        [JsonProperty("body")]
        public BodyGenerator Body { get; set; }

        [JsonProperty("headers")]
        public RecordGenerator Headers { get; set; }

        [JsonProperty("path")]
        public RecordGenerator Path { get; set; }

        [JsonProperty("query")]
        public Generator Query { get; set; }
    }

    public partial class BodyGenerator
    {
    }

    public partial class RecordGenerator
    {
    }

    public partial class Generator
    {
        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("type")]
        public TypeEnum Type { get; set; }

        [JsonProperty("digits")]
        public double? Digits { get; set; }

        [JsonProperty("max")]
        public double? Max { get; set; }

        [JsonProperty("min")]
        public double? Min { get; set; }

        [JsonProperty("size")]
        public double? Size { get; set; }

        [JsonProperty("regex")]
        public string Regex { get; set; }
    }

    public partial class RequestMatchingRules
    {
        [JsonProperty("body")]
        public Dictionary<string, object> Body { get; set; }

        [JsonProperty("header")]
        public Dictionary<string, object> Header { get; set; }

        [JsonProperty("path")]
        public Matchers Path { get; set; }

        [JsonProperty("query")]
        public Dictionary<string, object> Query { get; set; }
    }

    public partial class Matchers
    {
        [JsonProperty("combine")]
        public Combine? Combine { get; set; }

        [JsonProperty("matchers")]
        public Match[] MatchersMatchers { get; set; }
    }

    public partial class Match
    {
        [JsonProperty("match")]
        public MatchEnum MatchMatch { get; set; }

        [JsonProperty("regex")]
        public string Regex { get; set; }

        [JsonProperty("max")]
        public double? Max { get; set; }

        [JsonProperty("min")]
        public double? Min { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("body")]
        public object Body { get; set; }

        [JsonProperty("generators")]
        public ResponseGenerators Generators { get; set; }

        [JsonProperty("headers")]
        public Dictionary<string, object> Headers { get; set; }

        [JsonProperty("matchingRules")]
        public RequestMatchingRules MatchingRules { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }
    }

    public partial class ResponseGenerators
    {
        [JsonProperty("body")]
        public BodyGenerator Body { get; set; }

        [JsonProperty("headers")]
        public RecordGenerator Headers { get; set; }

        [JsonProperty("status")]
        public Generator Status { get; set; }
    }

    public partial class Message
    {
        [JsonProperty("contents")]
        public object Contents { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("generators")]
        public MessageGenerators Generators { get; set; }

        [JsonProperty("matchingRules")]
        public MessageMatchingRules MatchingRules { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonProperty("metaData")]
        public Dictionary<string, object> MetaData { get; set; }

        [JsonProperty("providerState")]
        public string ProviderState { get; set; }
    }

    public partial class MessageGenerators
    {
        [JsonProperty("body")]
        public BodyGenerator Body { get; set; }

        [JsonProperty("metadata")]
        public RecordGenerator Metadata { get; set; }
    }

    public partial class MessageMatchingRules
    {
        [JsonProperty("body")]
        public Dictionary<string, object> Body { get; set; }
    }

    public partial class Metadata
    {
        [JsonProperty("pact-specification")]
        public PactSpecification PactSpecification { get; set; }

        [JsonProperty("pactSpecification")]
        public PactSpecificationClass MetadataPactSpecification { get; set; }

        [JsonProperty("pactSpecificationVersion")]
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

    public enum TypeEnum { Date, DateTime, RandomBoolean, RandomDecimal, RandomHexadecimal, RandomInt, RandomString, Regex, Time, Uuid };

    public enum Combine { And, Or };

    public enum MatchEnum { Boolean, ContentType, Date, Datetime, Decimal, Equality, Include, Integer, Null, Number, Regex, Time, Type, Values };

    public enum Method { Connect, Delete, Get, Head, MethodConnect, MethodDelete, MethodGet, MethodHead, MethodOptions, MethodPost, MethodPut, MethodTrace, Options, Post, Put, Trace };

    public partial struct ProviderStates
    {
        public ProviderState[] ProviderStateArray;
        public string String;

        public static implicit operator ProviderStates(ProviderState[] ProviderStateArray) => new ProviderStates { ProviderStateArray = ProviderStateArray };
        public static implicit operator ProviderStates(string String) => new ProviderStates { String = String };
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ProviderStatesConverter.Singleton,
                TypeEnumConverter.Singleton,
                CombineConverter.Singleton,
                MatchEnumConverter.Singleton,
                MethodConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ProviderStatesConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ProviderStates) || t == typeof(ProviderStates?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new ProviderStates { String = stringValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<ProviderState[]>(reader);
                    return new ProviderStates { ProviderStateArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type ProviderStates");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ProviderStates)untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
            if (value.ProviderStateArray != null)
            {
                serializer.Serialize(writer, value.ProviderStateArray);
                return;
            }
            throw new Exception("Cannot marshal type ProviderStates");
        }

        public static readonly ProviderStatesConverter Singleton = new ProviderStatesConverter();
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Date":
                    return TypeEnum.Date;
                case "DateTime":
                    return TypeEnum.DateTime;
                case "RandomBoolean":
                    return TypeEnum.RandomBoolean;
                case "RandomDecimal":
                    return TypeEnum.RandomDecimal;
                case "RandomHexadecimal":
                    return TypeEnum.RandomHexadecimal;
                case "RandomInt":
                    return TypeEnum.RandomInt;
                case "RandomString":
                    return TypeEnum.RandomString;
                case "Regex":
                    return TypeEnum.Regex;
                case "Time":
                    return TypeEnum.Time;
                case "Uuid":
                    return TypeEnum.Uuid;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            switch (value)
            {
                case TypeEnum.Date:
                    serializer.Serialize(writer, "Date");
                    return;
                case TypeEnum.DateTime:
                    serializer.Serialize(writer, "DateTime");
                    return;
                case TypeEnum.RandomBoolean:
                    serializer.Serialize(writer, "RandomBoolean");
                    return;
                case TypeEnum.RandomDecimal:
                    serializer.Serialize(writer, "RandomDecimal");
                    return;
                case TypeEnum.RandomHexadecimal:
                    serializer.Serialize(writer, "RandomHexadecimal");
                    return;
                case TypeEnum.RandomInt:
                    serializer.Serialize(writer, "RandomInt");
                    return;
                case TypeEnum.RandomString:
                    serializer.Serialize(writer, "RandomString");
                    return;
                case TypeEnum.Regex:
                    serializer.Serialize(writer, "Regex");
                    return;
                case TypeEnum.Time:
                    serializer.Serialize(writer, "Time");
                    return;
                case TypeEnum.Uuid:
                    serializer.Serialize(writer, "Uuid");
                    return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }

    internal class CombineConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Combine) || t == typeof(Combine?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "AND":
                    return Combine.And;
                case "OR":
                    return Combine.Or;
            }
            throw new Exception("Cannot unmarshal type Combine");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Combine)untypedValue;
            switch (value)
            {
                case Combine.And:
                    serializer.Serialize(writer, "AND");
                    return;
                case Combine.Or:
                    serializer.Serialize(writer, "OR");
                    return;
            }
            throw new Exception("Cannot marshal type Combine");
        }

        public static readonly CombineConverter Singleton = new CombineConverter();
    }

    internal class MatchEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(MatchEnum) || t == typeof(MatchEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "boolean":
                    return MatchEnum.Boolean;
                case "contentType":
                    return MatchEnum.ContentType;
                case "date":
                    return MatchEnum.Date;
                case "datetime":
                    return MatchEnum.Datetime;
                case "decimal":
                    return MatchEnum.Decimal;
                case "equality":
                    return MatchEnum.Equality;
                case "include":
                    return MatchEnum.Include;
                case "integer":
                    return MatchEnum.Integer;
                case "null":
                    return MatchEnum.Null;
                case "number":
                    return MatchEnum.Number;
                case "regex":
                    return MatchEnum.Regex;
                case "time":
                    return MatchEnum.Time;
                case "type":
                    return MatchEnum.Type;
                case "values":
                    return MatchEnum.Values;
            }
            throw new Exception("Cannot unmarshal type MatchEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (MatchEnum)untypedValue;
            switch (value)
            {
                case MatchEnum.Boolean:
                    serializer.Serialize(writer, "boolean");
                    return;
                case MatchEnum.ContentType:
                    serializer.Serialize(writer, "contentType");
                    return;
                case MatchEnum.Date:
                    serializer.Serialize(writer, "date");
                    return;
                case MatchEnum.Datetime:
                    serializer.Serialize(writer, "datetime");
                    return;
                case MatchEnum.Decimal:
                    serializer.Serialize(writer, "decimal");
                    return;
                case MatchEnum.Equality:
                    serializer.Serialize(writer, "equality");
                    return;
                case MatchEnum.Include:
                    serializer.Serialize(writer, "include");
                    return;
                case MatchEnum.Integer:
                    serializer.Serialize(writer, "integer");
                    return;
                case MatchEnum.Null:
                    serializer.Serialize(writer, "null");
                    return;
                case MatchEnum.Number:
                    serializer.Serialize(writer, "number");
                    return;
                case MatchEnum.Regex:
                    serializer.Serialize(writer, "regex");
                    return;
                case MatchEnum.Time:
                    serializer.Serialize(writer, "time");
                    return;
                case MatchEnum.Type:
                    serializer.Serialize(writer, "type");
                    return;
                case MatchEnum.Values:
                    serializer.Serialize(writer, "values");
                    return;
            }
            throw new Exception("Cannot marshal type MatchEnum");
        }

        public static readonly MatchEnumConverter Singleton = new MatchEnumConverter();
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
