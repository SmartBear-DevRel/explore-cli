using System.Text.Json;
using Explore.Cli.Models;
using Explore.Cli.Models.Insomnia;

public static class InsomniaCollectionMappingHelper
{
    public static bool IsCollectionExportVersion4(string json)
    {
        var jsonObject = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        if(jsonObject != null && jsonObject.ContainsKey("__export_format"))
        {
            if(jsonObject["__export_format"] != null && jsonObject["__export_format"].ToString() != null)
            {
                var exportFormat = jsonObject["__export_format"].ToString();
                if(string.Equals(exportFormat, "4", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    public static bool IsItemRequestModeSupported(Resource resource)
    {
        if(resource.Method != null && resource.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if(resource.Body != null && resource.Body.MimeType != null && (resource.Body.MimeType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) || resource.Body.MimeType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return false;
    } 

    public static Connection MapInsomniaRequestResourceToExploreConnection(Resource resource, List<Resource> environmentResources)
    {
        return new Connection()
        {
            Type = "ConnectionRequest",
            Name = "REST",
            Schema = "OpenAPI",
            SchemaVersion = "3.0.1",
            ConnectionDefinition = new ConnectionDefinition()
            {
                Servers = new List<Server>()
                {
                    new Server()
                    {
                        Url = ParseUrl(resource?.Url, environmentResources)
                    }
                },
                Paths = CreatePathsDictionary(resource, environmentResources),                
            },
            Settings = new Settings()
            {
                Type = "RestConnectionSettings",
                ConnectTimeout = 30,
                FollowRedirects = true,
                EncodeUrl = true
            },
            Credentials = MapInsomniaAuthenticationToExploreCredentials(resource?.Authentication)
        };
    }

    public static string ParseUrl(string? url, List<Resource> environmentResources)
    {
        if(string.IsNullOrEmpty(url))
        {
            return string.Empty;
        }

        url = ReplaceEnvironmentVariables(url, environmentResources);
        return url.Substring(0, UtilityHelper.IndexOfNth(url, '/', 3));
    }

    public static string ReplaceEnvironmentVariables(string? value, List<Resource> environmentResources)
    {
        if(string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        if(!value.Contains("{{ _."))
        {
            return value;
        }

        // replace the environment variables in the string
        foreach(var variable in environmentResources)
        {
            if(variable.Data != null && variable.Data.Any())
            {                
                foreach(KeyValuePair<string, string> entry in variable.Data)
                {
                    value = value.Replace($"{{{{ _.{entry.Key} }}}}", entry.Value);
                }
            }
        }

        return value;
    }

    public static Examples MapEntryBodyToContentExamples(string? rawBody)
    {
        return new Examples()
        {
            Example = new Example()
            {
                Value = rawBody
            }
        };        
    }

    public static Dictionary<string, object> CreatePathsDictionary(Resource? resource, List<Resource> environmentResources)
    {   
        if(!string.IsNullOrEmpty(resource?.Url))
        {
            var pathsContent = new PathsContent()
            {
                Parameters = MapHeaderAndQueryParams(resource, environmentResources)
            };

            //add request body
            if(resource.Body != null && resource.Body.MimeType != null && resource.Body.MimeType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                var examplesJson = new Dictionary<string, object>
                {
                    { "examples", MapEntryBodyToContentExamples(resource.Body.Text) }
                };

                var contentJson = new Dictionary<string, object>
                {
                    { "*/*", examplesJson }
                };

                pathsContent.RequestBody = new RequestBody()
                {
                    Content = contentJson
                };                
                                
            }
            else if(resource.Body != null && resource.Body.MimeType != null && resource.Body.MimeType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            {
                var examplesJson = new Dictionary<string, object>
                {
                    { "examples", MapUrlEncodedBodyToContentExamples(resource.Body.Params, environmentResources) }
                };

                var contentJson = new Dictionary<string, object>
                {
                    { "application/x-www-form-urlencoded", examplesJson }
                };

                pathsContent.RequestBody = new RequestBody()
                {
                    Content = contentJson
                };
            }         

            var baseUrl = resource.Url.Split("?")[0];
            var path = baseUrl.Substring(UtilityHelper.IndexOfNth(baseUrl, '/', 3));

            // add header and query params
            if(resource.Method != null)
            {
                var methodJson = new Dictionary<string, object>
                {
                    { resource.Method.ToLowerInvariant(), pathsContent }
                };

                var json = new Dictionary<string, object>
                {
                    { $"{path}", methodJson }
                };

                return json;                
            }
        }

        return new Dictionary<string, object>();
    }

    public static Examples MapUrlEncodedBodyToContentExamples(List<Param>? formParams, List<Resource> environmentResources)
    {
        var rawBody = string.Empty;

        if(formParams != null)
        {
            foreach(var param in formParams)
            {
                rawBody += $"{ReplaceEnvironmentVariables(param.Name, environmentResources)}={ReplaceEnvironmentVariables(param.Value, environmentResources)}&";
            }
        }

        return new Examples()
        {
            Example = new Example()
            {
                Value = rawBody
            }
        };        
    }    

    public static List<Explore.Cli.Models.Parameter> MapHeaderAndQueryParams(Resource resource, List<Resource> environmentResources)
    {
        List<Explore.Cli.Models.Parameter> parameters = new List<Explore.Cli.Models.Parameter>();

        if(resource.Headers != null && resource.Headers.Any())
        {
            // map the headers
            foreach(var hdr in resource.Headers)
            {
                parameters.Add(new Explore.Cli.Models.Parameter()
                {
                    In = "header",
                    Name = ReplaceEnvironmentVariables(hdr.Name, environmentResources),
                    Examples = new Examples()
                    {
                        Example = new Example()
                        {
                            Value = ReplaceEnvironmentVariables(hdr.Value, environmentResources)
                        }
                    }
                });
            }
            
        }

        //if we have urlencoded body then force the content type header as plaintext (Explore doesn't support urlencoded natively)
        if(resource?.Body != null && resource.Body.Params != null && resource.Body.Params.Any())
        {
            parameters.Add(new Explore.Cli.Models.Parameter()
            {
                In = "header",
                Name = "Content-Type",
                Schema = new Schema()
                {
                    type = "string"
                },
                Examples = new Examples()
                {
                    Example = new Example()
                    {
                        Value = "application/x-www-form-urlencoded"
                    }
                }
            });
        }

        // parse and map the query string
        if(resource?.Parameters != null && resource.Parameters.Any())
        {
            foreach(var param in resource.Parameters)
            {
                parameters.Add(new Explore.Cli.Models.Parameter()
                {
                    In = "query",
                    Name = param.Name,
                    Examples = new Examples()
                    {
                        Example = new Example()
                        {
                            Value = ReplaceEnvironmentVariables(param.Value, environmentResources)
                        }
                    }
                });
            }
        }

        // parse the raw url for query parameters not explicitly defined
        if(!string.IsNullOrEmpty(resource?.Url) && resource.Url.Contains("?"))
        {
            var query = resource.Url.Split("?")[1];
            var queryParameters = query.Split("&");

            foreach(var queryParameter in queryParameters)
            {
                var keyValuePair = queryParameter.Split("=");

                if(parameters.Any(p => p.Name == keyValuePair[0]))
                {
                    continue;
                }

                parameters.Add(new Explore.Cli.Models.Parameter()
                {
                    In = "query",
                    Name = keyValuePair[0],
                    Examples = new Examples()
                    {
                        Example = new Example()
                        {
                            Value = ReplaceEnvironmentVariables(keyValuePair[1], environmentResources)
                        }
                    }
                });
            }
        }

        return parameters;
    }   
   
    public static Credentials? MapInsomniaAuthenticationToExploreCredentials(Authentication? authentication)
    {
        if(authentication == null || authentication.Type != null)
        {
            return null;
        }

        if(authentication.Disabled == true)
        {
            return null;
        }

        switch(authentication?.Type?.ToLowerInvariant())
        {
            case ("basic"):
                return new Credentials()
                {
                    Type = "Basic",
                    Username = authentication.Username,
                    Password = authentication.Password
                };
            
            case ("bearer"):
                return new Credentials()
                {
                    Type = "Bearer",
                    Token = authentication.Token
                };            
        }

        return null;
    }
   
}