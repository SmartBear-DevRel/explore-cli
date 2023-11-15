using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.StaticFiles;
using NJsonSchema;
using Spectre.Console;

public static class UtilityHelper
{
    public static string CleanString(string? inputName)
    {
        if (string.IsNullOrEmpty(inputName))
        {
            return string.Empty;
        }

        try
        {
            return Regex.Replace(inputName, @"[^a-zA-Z0-9 ._-]", "", RegexOptions.None, TimeSpan.FromSeconds(2));
        }
        // return empty string rather than timeout
        catch (RegexMatchTimeoutException)
        {
            return String.Empty;
        }
    }

    public static bool IsContentTypeExpected(HttpContentHeaders? headers, string expectedContentType)
    {
        if (headers == null)
        {
            return false;
        }

        foreach (var header in headers)
        {

            if (string.Equals(header.Key, "Content-Type", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(header.Value.FirstOrDefault(), expectedContentType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        //no content type
        return false;
    }

    public static bool IsJsonFile(string filePath)
    {
        string extension = Path.GetExtension(filePath);

        if (string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase))
        {
            // Check the MIME type to further validate
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                return false;
            }

            return string.Equals(contentType, MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public static async Task<SchemaValidationResult> ValidateSchema(string jsonAsString, string schemaName)
    {
        var validationResult = new SchemaValidationResult();
        var schemaAsString = @"{
  ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
  ""type"": ""object"",
  ""description"": ""an object storing SwaggerHub Explore spaces which have been exported (or crafted to import via the Explore.cli)."",
  ""properties"": {
    ""info"": {
        ""type"": ""object"",
        ""properties"": {
            ""version"": {
                ""type"": ""string"",
                ""description"": ""the version of the explore spaces export/import capability"",
                ""pattern"": ""^([0-9]+)\\.([0-9]+)\\.([0-9]+)(?:-([0-9A-Za-z-]+(?:\\.[0-9A-Za-z-]+)*))?(?:\\+[0-9A-Za-z-]+)?$"",
                ""example"": ""0.0.1""
            },
            ""exportedAt"": {
                ""type"": ""string"",
                ""description"": ""the timestamp when the export was created""
            }
        },
        ""required"": [
            ""version""
        ]
    },
    ""exploreSpaces"": {
        ""type"": ""array"",
        ""description"": ""an array of exported SwaggerHub Explore spaces, apis, and connections"",
        ""items"": [
            {
                ""type"": ""object"",
                ""description"": ""a SwaggerHub Explore space"",
                ""properties"": {
                    ""id"": {
                        ""type"": ""string"",
                        ""description"": ""the space identifier"",
                        ""pattern"": ""^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$""
                    },
                    ""name"": {
                        ""type"": ""string"",
                        ""description"": ""the name of the space""
                    },
                    ""apis"":
                    {
                        ""type"": ""array"",
                        ""description"": ""apis contained within a space"",
                        ""items"": [
                            {
                                ""type"": ""object"",
                                ""description"": ""an API contained within a space"",
                                ""properties"": {
                                    ""id"": {
                                        ""type"": ""string"",
                                        ""description"": ""the api identifier"",
                                        ""pattern"": ""^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$""
                                    },
                                    ""name"": {
                                        ""type"": ""string"",
                                        ""description"": ""the name of the api""
                                    },
                                    ""type"": {
                                        ""type"": ""string"",
                                        ""description"": ""the type of API"",
                                        ""enum"": [""REST"", ""KAFKA"", ""OTHER""]                                            
                                    },
                                    ""connections"": {
                                        ""type"": ""array"",
                                        ""description"": ""an array of connections to an API"",
                                        ""items"": [
                                            {
                                                ""type"": ""object""
                                            }
                                        ]
                                    }
                                },
                                ""required"": [
                                    ""name"",
                                    ""type""
                                ]
                            }
                        ]
                    }
                },
                ""required"": [
                    ""name"",
                    ""apis""
                ]
            }
        ]
    }
  },
  ""required"": [
    ""info"",
    ""exploreSpaces""
  ]
}";

        //var schema = await JsonSchema.FromFileAsync($"/schemas/{schemaName}");        
        var schema = await JsonSchema.FromJsonAsync(schemaAsString);
        var errors = schema.Validate(jsonAsString);

        if (errors.Any())
        {
            var msg = $"‣ {errors.Count} total errors\n" +
            string.Join("", errors
                .Select(e => $"  ‣ {e}[/] at " +
                            $"{e.LineNumber}:{e.LinePosition}[/]\n"));

            validationResult.Message = msg;
        }
        else
        {
            validationResult.isValid = true;
        }

        return validationResult;
    }

    public static bool IsValidateFileName(ref string fileName)
    {
        if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
        {
            AnsiConsole.MarkupLine($"[red]The file name '{fileName}' contains invalid characters. Please review.[/]");
            return false;
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            AnsiConsole.MarkupLine($"[red]The file name cannot be empty. Please review.[/]");
            return false;
        }

        if (fileName.Contains('.'))
        {
            if (fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                fileName = $"{fileName}";
                return true;
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]The file name '{fileName}' has an invalid extension. Please review.[/]");
                return false;
            }
        }
        else
        {
            fileName = $"{fileName}.json";
        }

        return true;
    }

    private static bool IsValidFilePath(string filePath)
    {
        return filePath.IndexOfAny(Path.GetInvalidPathChars()) <= 0;
    }
}