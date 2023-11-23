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

    public static bool IsValidFileName(ref string fileName)
    {
        char[] invalidFileNameChars = new char[]
        {
            '<', '>', ':', '"', '/', '\\', '|', '?', '*', '\0',
            // Control characters (0x00-0x1F)
            '\x01', '\x02', '\x03', '\x04', '\x05', '\x06', '\x07',
            '\x08', '\x09', '\x0A', '\x0B', '\x0C', '\x0D', '\x0E', '\x0F',
            '\x10', '\x11', '\x12', '\x13', '\x14', '\x15', '\x16', '\x17',
            '\x18', '\x19', '\x1A', '\x1B', '\x1C', '\x1D', '\x1E', '\x1F'
        };

        if (fileName == null)
        {
            return false;
        }

        if (fileName.IndexOfAny(invalidFileNameChars) > 0)
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

    public static bool IsValidFilePath(ref string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            AnsiConsole.MarkupLine($"[red]The file path cannot be empty. Please review.[/]");
            return false;
        }

        char[] invalidChars = new char[]
        {
            '<', '>', ':', '"', '|', '?', '*', '\0',
            // Control characters (0x00-0x1F)
            '\x01', '\x02', '\x03', '\x04', '\x05', '\x06', '\x07',
            '\x08', '\x09', '\x0A', '\x0B', '\x0C', '\x0D', '\x0E', '\x0F',
            '\x10', '\x11', '\x12', '\x13', '\x14', '\x15', '\x16', '\x17',
            '\x18', '\x19', '\x1A', '\x1B', '\x1C', '\x1D', '\x1E', '\x1F'
        };

        if (filePath.IndexOfAny(invalidChars) > 0)
        {
            AnsiConsole.MarkupLine($"[red]The file path '{filePath}' contains invalid characters. Please review.[/]");
            return false;
        }

        // check if the exportPath is an absolute path
        if (!Path.IsPathRooted(filePath))
        {
            // if not, make it relative to the current directory
            filePath = Path.Combine(Environment.CurrentDirectory, filePath);
        }

        if (!Directory.Exists(filePath))
        {
            try
            {
                Directory.CreateDirectory(filePath);
            }
            catch (UnauthorizedAccessException)
            {
                AnsiConsole.MarkupLine($"[red]Access to {filePath} is denied. Please review file permissions any try again.[/]");
                return false;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred accessing the file: {ex.Message}[/]");
                return false;
            }
        }
        return true;
    }
}