namespace PactV4Contract
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
        [JsonProperty("comments", NullValueHandling = NullValueHandling.Ignore)]
        public Comments Comments { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("interactionMarkup", NullValueHandling = NullValueHandling.Ignore)]
        public InteractionMarkup InteractionMarkup { get; set; }

        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("pending", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Pending { get; set; }

        [JsonProperty("pluginConfiguration", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> PluginConfiguration { get; set; }

        [JsonProperty("providerStates", NullValueHandling = NullValueHandling.Ignore)]
        public ProviderStates? ProviderStates { get; set; }

        [JsonProperty("request", NullValueHandling = NullValueHandling.Ignore)]
        public Request Request { get; set; }

        [JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
        public ResponseUnion? Response { get; set; }

        [JsonProperty("type")]
        public InteractionType Type { get; set; }

        [JsonProperty("contents")]
        public object Contents { get; set; }

        [JsonProperty("generators", NullValueHandling = NullValueHandling.Ignore)]
        public InteractionGenerators Generators { get; set; }

        [JsonProperty("matchingRules", NullValueHandling = NullValueHandling.Ignore)]
        public InteractionMatchingRules MatchingRules { get; set; }

        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonProperty("metaData", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> MetaData { get; set; }
    }

    public partial class Comments
    {
        [JsonProperty("testname", NullValueHandling = NullValueHandling.Ignore)]
        public string Testname { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Text { get; set; }
    }

    public partial class InteractionGenerators
    {
        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public BodyGenerator Body { get; set; }

        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public RecordGenerator Metadata { get; set; }
    }

    public partial class BodyGenerator
    {
    }

    public partial class RecordGenerator
    {
    }

    public partial class InteractionMarkup
    {
        [JsonProperty("markup")]
        public string Markup { get; set; }

        [JsonProperty("markupType")]
        public MarkupType MarkupType { get; set; }
    }

    public partial class InteractionMatchingRules
    {
        [JsonProperty("body")]
        public Dictionary<string, object> Body { get; set; }
    }

    public partial class ProviderState
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("params", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Params { get; set; }
    }

    public partial class Request
    {
        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public Body Body { get; set; }

        [JsonProperty("generators", NullValueHandling = NullValueHandling.Ignore)]
        public RequestGenerators Generators { get; set; }

        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Headers { get; set; }

        [JsonProperty("matchingRules", NullValueHandling = NullValueHandling.Ignore)]
        public RequestMatchingRules MatchingRules { get; set; }

        [JsonProperty("method", NullValueHandling = NullValueHandling.Ignore)]
        public Method? Method { get; set; }

        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty("query", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Query { get; set; }

        [JsonProperty("contents")]
        public object Contents { get; set; }

        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonProperty("metaData", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> MetaData { get; set; }
    }

    public partial class Body
    {
        [JsonProperty("content")]
        public object Content { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("contentTypeHint")]
        public ContentTypeHint ContentTypeHint { get; set; }

        [JsonProperty("encoded")]
        public Encoded Encoded { get; set; }
    }

    public partial class RequestGenerators
    {
        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public BodyGenerator Body { get; set; }

        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public RecordGenerator Headers { get; set; }

        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public RecordGenerator Path { get; set; }

        [JsonProperty("query", NullValueHandling = NullValueHandling.Ignore)]
        public Generator Query { get; set; }

        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public RecordGenerator Metadata { get; set; }
    }

    public partial class Generator
    {
        [JsonProperty("format", NullValueHandling = NullValueHandling.Ignore)]
        public string Format { get; set; }

        [JsonProperty("type")]
        public GeneratorType Type { get; set; }

        [JsonProperty("digits", NullValueHandling = NullValueHandling.Ignore)]
        public double? Digits { get; set; }

        [JsonProperty("max", NullValueHandling = NullValueHandling.Ignore)]
        public double? Max { get; set; }

        [JsonProperty("min", NullValueHandling = NullValueHandling.Ignore)]
        public double? Min { get; set; }

        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public double? Size { get; set; }

        [JsonProperty("regex", NullValueHandling = NullValueHandling.Ignore)]
        public string Regex { get; set; }

        [JsonProperty("example", NullValueHandling = NullValueHandling.Ignore)]
        public string Example { get; set; }

        [JsonProperty("expression", NullValueHandling = NullValueHandling.Ignore)]
        public string Expression { get; set; }
    }

    public partial class RequestMatchingRules
    {
        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Body { get; set; }

        [JsonProperty("header", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Header { get; set; }

        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public Matchers Path { get; set; }

        [JsonProperty("query", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Query { get; set; }
    }

    public partial class Matchers
    {
        [JsonProperty("combine", NullValueHandling = NullValueHandling.Ignore)]
        public Combine? Combine { get; set; }

        [JsonProperty("matchers")]
        public Match[] MatchersMatchers { get; set; }
    }

    public partial class Match
    {
        [JsonProperty("match")]
        public MatchEnum MatchMatch { get; set; }

        [JsonProperty("regex", NullValueHandling = NullValueHandling.Ignore)]
        public string Regex { get; set; }

        [JsonProperty("max", NullValueHandling = NullValueHandling.Ignore)]
        public double? Max { get; set; }

        [JsonProperty("min", NullValueHandling = NullValueHandling.Ignore)]
        public double? Min { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }

        [JsonProperty("format", NullValueHandling = NullValueHandling.Ignore)]
        public string Format { get; set; }

        [JsonProperty("variants", NullValueHandling = NullValueHandling.Ignore)]
        public object[] Variants { get; set; }

        [JsonProperty("rules", NullValueHandling = NullValueHandling.Ignore)]
        public object[] Rules { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
    }

    public partial class MessageContents
    {
        [JsonProperty("contents")]
        public object Contents { get; set; }

        [JsonProperty("generators", NullValueHandling = NullValueHandling.Ignore)]
        public PurpleGenerators Generators { get; set; }

        [JsonProperty("matchingRules", NullValueHandling = NullValueHandling.Ignore)]
        public PurpleMatchingRules MatchingRules { get; set; }

        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonProperty("metaData", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> MetaData { get; set; }
    }

    public partial class PurpleGenerators
    {
        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public BodyGenerator Body { get; set; }

        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public RecordGenerator Metadata { get; set; }
    }

    public partial class PurpleMatchingRules
    {
        [JsonProperty("body")]
        public Dictionary<string, object> Body { get; set; }
    }

    public partial class ResponseClass
    {
        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public Body Body { get; set; }

        [JsonProperty("generators", NullValueHandling = NullValueHandling.Ignore)]
        public FluffyGenerators Generators { get; set; }

        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Headers { get; set; }

        [JsonProperty("matchingRules", NullValueHandling = NullValueHandling.Ignore)]
        public FluffyMatchingRules MatchingRules { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }
    }

    public partial class FluffyGenerators
    {
        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public BodyGenerator Body { get; set; }

        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public RecordGenerator Headers { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public Generator Status { get; set; }
    }

    public partial class FluffyMatchingRules
    {
        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Body { get; set; }

        [JsonProperty("header", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Header { get; set; }

        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public Matchers Path { get; set; }

        [JsonProperty("query", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Query { get; set; }
    }

    public partial class Metadata
    {
        [JsonProperty("pactSpecification", NullValueHandling = NullValueHandling.Ignore)]
        public PactSpecification PactSpecification { get; set; }
    }

    public partial class PactSpecification
    {
        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public enum MarkupType { CommonMark, Html };

    public enum ContentTypeHint { Binary, Text };

    public enum GeneratorType { Date, DateTime, MockServerUrl, ProviderState, RandomBoolean, RandomDecimal, RandomHexadecimal, RandomInt, RandomString, Regex, Time, Uuid };

    public enum Combine { And, Or };

    public enum MatchEnum { ArrayContains, Boolean, ContentType, Date, Datetime, Decimal, EachKey, EachValue, Equality, Include, Integer, NotEmpty, Null, Number, Regex, Semver, StatusCode, Time, Type, Values };

    public enum Method { Connect, Delete, Get, Head, MethodConnect, MethodDelete, MethodGet, MethodHead, MethodOptions, MethodPost, MethodPut, MethodTrace, Options, Post, Put, Trace };

    public enum InteractionType { AsynchronousMessages, SynchronousHttp, SynchronousMessages };

    public partial struct ProviderStates
    {
        public ProviderState[] ProviderStateArray;
        public string String;

        public static implicit operator ProviderStates(ProviderState[] ProviderStateArray) => new ProviderStates { ProviderStateArray = ProviderStateArray };
        public static implicit operator ProviderStates(string String) => new ProviderStates { String = String };
    }

    public partial struct Encoded
    {
        public bool? Bool;
        public string String;

        public static implicit operator Encoded(bool Bool) => new Encoded { Bool = Bool };
        public static implicit operator Encoded(string String) => new Encoded { String = String };
    }

    public partial struct ResponseUnion
    {
        public MessageContents[] MessageContentsArray;
        public ResponseClass ResponseClass;

        public static implicit operator ResponseUnion(MessageContents[] MessageContentsArray) => new ResponseUnion { MessageContentsArray = MessageContentsArray };
        public static implicit operator ResponseUnion(ResponseClass ResponseClass) => new ResponseUnion { ResponseClass = ResponseClass };
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                MarkupTypeConverter.Singleton,
                ProviderStatesConverter.Singleton,
                ContentTypeHintConverter.Singleton,
                EncodedConverter.Singleton,
                GeneratorTypeConverter.Singleton,
                CombineConverter.Singleton,
                MatchEnumConverter.Singleton,
                MethodConverter.Singleton,
                ResponseUnionConverter.Singleton,
                InteractionTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class MarkupTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(MarkupType) || t == typeof(MarkupType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "COMMON_MARK":
                    return MarkupType.CommonMark;
                case "HTML":
                    return MarkupType.Html;
            }
            throw new Exception("Cannot unmarshal type MarkupType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (MarkupType)untypedValue;
            switch (value)
            {
                case MarkupType.CommonMark:
                    serializer.Serialize(writer, "COMMON_MARK");
                    return;
                case MarkupType.Html:
                    serializer.Serialize(writer, "HTML");
                    return;
            }
            throw new Exception("Cannot marshal type MarkupType");
        }

        public static readonly MarkupTypeConverter Singleton = new MarkupTypeConverter();
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

    internal class ContentTypeHintConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ContentTypeHint) || t == typeof(ContentTypeHint?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "BINARY":
                    return ContentTypeHint.Binary;
                case "TEXT":
                    return ContentTypeHint.Text;
            }
            throw new Exception("Cannot unmarshal type ContentTypeHint");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ContentTypeHint)untypedValue;
            switch (value)
            {
                case ContentTypeHint.Binary:
                    serializer.Serialize(writer, "BINARY");
                    return;
                case ContentTypeHint.Text:
                    serializer.Serialize(writer, "TEXT");
                    return;
            }
            throw new Exception("Cannot marshal type ContentTypeHint");
        }

        public static readonly ContentTypeHintConverter Singleton = new ContentTypeHintConverter();
    }

    internal class EncodedConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Encoded) || t == typeof(Encoded?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Boolean:
                    var boolValue = serializer.Deserialize<bool>(reader);
                    return new Encoded { Bool = boolValue };
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new Encoded { String = stringValue };
            }
            throw new Exception("Cannot unmarshal type Encoded");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (Encoded)untypedValue;
            if (value.Bool != null)
            {
                serializer.Serialize(writer, value.Bool.Value);
                return;
            }
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
            throw new Exception("Cannot marshal type Encoded");
        }

        public static readonly EncodedConverter Singleton = new EncodedConverter();
    }

    internal class GeneratorTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(GeneratorType) || t == typeof(GeneratorType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Date":
                    return GeneratorType.Date;
                case "DateTime":
                    return GeneratorType.DateTime;
                case "MockServerURL":
                    return GeneratorType.MockServerUrl;
                case "ProviderState":
                    return GeneratorType.ProviderState;
                case "RandomBoolean":
                    return GeneratorType.RandomBoolean;
                case "RandomDecimal":
                    return GeneratorType.RandomDecimal;
                case "RandomHexadecimal":
                    return GeneratorType.RandomHexadecimal;
                case "RandomInt":
                    return GeneratorType.RandomInt;
                case "RandomString":
                    return GeneratorType.RandomString;
                case "Regex":
                    return GeneratorType.Regex;
                case "Time":
                    return GeneratorType.Time;
                case "Uuid":
                    return GeneratorType.Uuid;
            }
            throw new Exception("Cannot unmarshal type GeneratorType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (GeneratorType)untypedValue;
            switch (value)
            {
                case GeneratorType.Date:
                    serializer.Serialize(writer, "Date");
                    return;
                case GeneratorType.DateTime:
                    serializer.Serialize(writer, "DateTime");
                    return;
                case GeneratorType.MockServerUrl:
                    serializer.Serialize(writer, "MockServerURL");
                    return;
                case GeneratorType.ProviderState:
                    serializer.Serialize(writer, "ProviderState");
                    return;
                case GeneratorType.RandomBoolean:
                    serializer.Serialize(writer, "RandomBoolean");
                    return;
                case GeneratorType.RandomDecimal:
                    serializer.Serialize(writer, "RandomDecimal");
                    return;
                case GeneratorType.RandomHexadecimal:
                    serializer.Serialize(writer, "RandomHexadecimal");
                    return;
                case GeneratorType.RandomInt:
                    serializer.Serialize(writer, "RandomInt");
                    return;
                case GeneratorType.RandomString:
                    serializer.Serialize(writer, "RandomString");
                    return;
                case GeneratorType.Regex:
                    serializer.Serialize(writer, "Regex");
                    return;
                case GeneratorType.Time:
                    serializer.Serialize(writer, "Time");
                    return;
                case GeneratorType.Uuid:
                    serializer.Serialize(writer, "Uuid");
                    return;
            }
            throw new Exception("Cannot marshal type GeneratorType");
        }

        public static readonly GeneratorTypeConverter Singleton = new GeneratorTypeConverter();
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
                case "arrayContains":
                    return MatchEnum.ArrayContains;
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
                case "eachKey":
                    return MatchEnum.EachKey;
                case "eachValue":
                    return MatchEnum.EachValue;
                case "equality":
                    return MatchEnum.Equality;
                case "include":
                    return MatchEnum.Include;
                case "integer":
                    return MatchEnum.Integer;
                case "notEmpty":
                    return MatchEnum.NotEmpty;
                case "null":
                    return MatchEnum.Null;
                case "number":
                    return MatchEnum.Number;
                case "regex":
                    return MatchEnum.Regex;
                case "semver":
                    return MatchEnum.Semver;
                case "statusCode":
                    return MatchEnum.StatusCode;
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
                case MatchEnum.ArrayContains:
                    serializer.Serialize(writer, "arrayContains");
                    return;
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
                case MatchEnum.EachKey:
                    serializer.Serialize(writer, "eachKey");
                    return;
                case MatchEnum.EachValue:
                    serializer.Serialize(writer, "eachValue");
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
                case MatchEnum.NotEmpty:
                    serializer.Serialize(writer, "notEmpty");
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
                case MatchEnum.Semver:
                    serializer.Serialize(writer, "semver");
                    return;
                case MatchEnum.StatusCode:
                    serializer.Serialize(writer, "statusCode");
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

    internal class ResponseUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ResponseUnion) || t == typeof(ResponseUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<ResponseClass>(reader);
                    return new ResponseUnion { ResponseClass = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<MessageContents[]>(reader);
                    return new ResponseUnion { MessageContentsArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type ResponseUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ResponseUnion)untypedValue;
            if (value.MessageContentsArray != null)
            {
                serializer.Serialize(writer, value.MessageContentsArray);
                return;
            }
            if (value.ResponseClass != null)
            {
                serializer.Serialize(writer, value.ResponseClass);
                return;
            }
            throw new Exception("Cannot marshal type ResponseUnion");
        }

        public static readonly ResponseUnionConverter Singleton = new ResponseUnionConverter();
    }

    internal class InteractionTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(InteractionType) || t == typeof(InteractionType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Asynchronous/Messages":
                    return InteractionType.AsynchronousMessages;
                case "Synchronous/HTTP":
                    return InteractionType.SynchronousHttp;
                case "Synchronous/Messages":
                    return InteractionType.SynchronousMessages;
            }
            throw new Exception("Cannot unmarshal type InteractionType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (InteractionType)untypedValue;
            switch (value)
            {
                case InteractionType.AsynchronousMessages:
                    serializer.Serialize(writer, "Asynchronous/Messages");
                    return;
                case InteractionType.SynchronousHttp:
                    serializer.Serialize(writer, "Synchronous/HTTP");
                    return;
                case InteractionType.SynchronousMessages:
                    serializer.Serialize(writer, "Synchronous/Messages");
                    return;
            }
            throw new Exception("Cannot marshal type InteractionType");
        }

        public static readonly InteractionTypeConverter Singleton = new InteractionTypeConverter();
    }
}
