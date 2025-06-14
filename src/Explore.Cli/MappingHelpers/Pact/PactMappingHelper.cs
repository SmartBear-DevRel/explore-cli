using System.Text.Json;
using Explore.Cli.Models.Explore;

public static class PactMappingHelper
{

    public static bool hasPactVersion(string json)
    {
        var jsonObject = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        if (jsonObject != null && jsonObject.ContainsKey("metadata"))
        {
            var metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObject["metadata"].ToString() ?? string.Empty);
            if (metadata != null && metadata.ContainsKey("pactSpecification"))
            {
                var pactSpecification = JsonSerializer.Deserialize<Dictionary<string, object>>(metadata["pactSpecification"].ToString() ?? string.Empty);
                if (pactSpecification != null && pactSpecification["version"] != null && pactSpecification["version"].ToString() != null)
                {
                    var version = pactSpecification["version"].ToString();
                    var validVersions = new List<string> { "1.0.0", "2.0.0", "3.0.0", "4.0.0", "4.0" };
                    if (version != null && validVersions.Contains(version))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static string getPactVersion(string json)
    {
        var jsonObject = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        if (jsonObject != null && jsonObject.ContainsKey("metadata"))
        {
            var metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObject["metadata"].ToString() ?? string.Empty);
            if (metadata != null && metadata.ContainsKey("pactSpecification"))
            {
                var pactSpecification = JsonSerializer.Deserialize<Dictionary<string, object>>(metadata["pactSpecification"].ToString() ?? string.Empty);
                if (pactSpecification != null && pactSpecification["version"] != null && pactSpecification["version"].ToString() != null)
                {
                    var version = pactSpecification["version"].ToString();
                    var validVersions = new List<string> { "1.0.0", "2.0.0", "3.0.0", "4.0.0", "4.0" };
                    if (version != null && validVersions.Contains(version))
                    {
                        var formattedVersion = version.Split('.')[0];
                        return $"pact-v{formattedVersion}";
                    }
                }
            }
        }

        return string.Empty;
    }
    public static Dictionary<string, object> CreatePactPathsDictionary(object request)
    {
        if (request is PactV4.Request v4Request)
        {
            if (v4Request?.Path != null)
            {
                var pathsContent = new PathsContent()
                {
                    Parameters = MapHeaderAndQueryParams(request)
                };

                // //add request body
                if (v4Request.Body?.Content != null)
                {
                    var examplesJson = new Dictionary<string, object>
                {
                    { "examples", MapEntryBodyToContentExamples(v4Request.Body.Content.ToString()) }
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


                // add header and query params
                if (v4Request.Method != null)
                {
                    var methodJson = new Dictionary<string, object>
                {
                    { v4Request.Method?.ToString()?.Replace("Method", string.Empty).ToLower() ?? string.Empty, pathsContent }
                };

                    var json = new Dictionary<string, object>
                {
                    { v4Request.Path, methodJson }
                };

                    return json;
                }
            }

            return new Dictionary<string, object>();
        }
        else if (request is PactV3.Request v3Request)
        {
            if (v3Request?.Path != null)
            {
                var pathsContent = new PathsContent()
                {
                    Parameters = MapHeaderAndQueryParams(request)
                };

                // //add request body
                if (v3Request.Body != null)
                {
                    var examplesJson = new Dictionary<string, object>
                {
                    { "examples", MapEntryBodyToContentExamples(v3Request.Body.ToString()) }
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


                // add header and query params
                if (v3Request.Method != null)
                {
                    var methodJson = new Dictionary<string, object>
                {
                    { v3Request.Method?.ToString()?.Replace("Method", string.Empty).ToLower() ?? string.Empty, pathsContent }
                };

                    var json = new Dictionary<string, object>
                {
                    { v3Request.Path, methodJson }
                };

                    return json;
                }
            }

            return new Dictionary<string, object>();
        }
        else if (request is PactV2.Request v2Request)
        {
            if (v2Request?.Path != null)
            {
                var pathsContent = new PathsContent()
                {
                    Parameters = MapHeaderAndQueryParams(request)
                };

                // //add request body
                if (v2Request.Body != null)
                {
                    var examplesJson = new Dictionary<string, object>
                {
                    { "examples", MapEntryBodyToContentExamples(v2Request.Body.ToString()) }
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


                // add header and query params
                if (v2Request.Method != null)
                {
                    var methodJson = new Dictionary<string, object>
                {
                    { v2Request.Method?.ToString()?.Replace("Method", string.Empty).ToLower() ?? string.Empty, pathsContent }
                };

                    var json = new Dictionary<string, object>
                {
                    { v2Request.Path, methodJson }
                };

                    return json;
                }
            }

            return new Dictionary<string, object>();
        }
        else
        {
            return new Dictionary<string, object>();
        }
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

    public static List<Parameter> MapHeaderAndQueryParams(object request)
    {
        List<Parameter> parameters = new List<Parameter>();

        if (request is PactV2.Request v2Request)
        {
            if (v2Request.Headers != null && v2Request.Headers.Any())
            {
                // map the headers
                foreach (var hdr in v2Request.Headers)
                {
                    parameters.Add(new Parameter()
                    {
                        In = "header",
                        Name = hdr.Key,
                        Examples = new Examples()
                        {
                            Example = new Example()
                            {
                                Value = hdr.Value.ToString()
                            }
                        }
                    });
                }
            }

            if (v2Request.Query != null)
            {
                foreach (var param in v2Request.Query.Split("&"))
                {
                    parameters.Add(new Parameter()
                    {
                        In = "query",
                        Name = param.Split("=")[0],
                        Examples = new Examples()
                        {
                            Example = new Example()
                            {
                                Value = param.Split("=")[1]
                            }
                        }
                    });
                }
            }
        }
        else if (request is PactV3.Request v3Request)
        {
            if (v3Request.Headers != null && v3Request.Headers.Any())
            {
                // map the headers
                foreach (var hdr in v3Request.Headers)
                {
                    parameters.Add(new Parameter()
                    {
                        In = "header",
                        Name = hdr.Key,
                        Examples = new Examples()
                        {
                            Example = new Example()
                            {
                                Value = hdr.Value.ToString()
                            }
                        }
                    });
                }
            }

            if (v3Request.Query != null)
            {
                foreach (var param in v3Request.Query)
                {
                    if (param.Value.Length > 1)
                    {
                        foreach (var value in param.Value)
                        {
                            parameters.Add(new Parameter()
                            {
                                In = "query",
                                Name = $"{param.Key}[]",
                                Examples = new Examples()
                                {
                                    Example = new Example()
                                    {
                                        Value = value.ToString()
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        parameters.Add(new Parameter()
                        {
                            In = "query",
                            Name = param.Key,
                            Examples = new Examples()
                            {
                                Example = new Example()
                                {
                                    Value = param.Value.First().ToString()
                                }
                            }
                        });
                    }
                }
            }
        }
        else if (request is PactV4.Request v4Request)
        {
            if (v4Request.Headers != null && v4Request.Headers.Any())
            {
                // map the headers
                foreach (var hdr in v4Request.Headers)
                {
                    parameters.Add(new Parameter()
                    {
                        In = "header",
                        Name = hdr.Key,
                        Examples = new Examples()
                        {
                            Example = new Example()
                            {
                                Value = string.Join(",", hdr.Value.Select(x => x.ToString()))
                            }
                        }
                    });
                }
            }

            if (v4Request.Query != null)
            {
                foreach (var param in v4Request.Query)
                {
                    if (param.Value.Length > 1)
                    {
                        foreach (var value in param.Value)
                        {
                            parameters.Add(new Parameter()
                            {
                                In = "query",
                                Name = $"{param.Key}[]",
                                Examples = new Examples()
                                {
                                    Example = new Example()
                                    {
                                        Value = value.ToString()
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        parameters.Add(new Parameter()
                        {
                            In = "query",
                            Name = param.Key,
                            Examples = new Examples()
                            {
                                Example = new Example()
                                {
                                    Value = param.Value.First().ToString()
                                }
                            }
                        });
                    }
                }
            }
        }

        return parameters;
    }


    public static Connection MapPactInteractionToExploreConnection(object pactInteraction, string url = "")
    {
        var connection = new Connection()
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
                        Url = url
                    }
                },
            },
            Settings = new Settings()
            {
                Type = "RestConnectionSettings",
                ConnectTimeout = 30,
                FollowRedirects = true,
                EncodeUrl = true
            },
        };

        if (pactInteraction is PactV2.Interaction v2Interaction)
        {
            connection.ConnectionDefinition.Paths = CreatePactPathsDictionary(v2Interaction.Request);
        }
        else if (pactInteraction is PactV3.Interaction v3Interaction)
        {
            connection.ConnectionDefinition.Paths = CreatePactPathsDictionary(v3Interaction.Request);
        }
        else if (pactInteraction is PactV4.Interaction v4Interaction)
        {
            connection.ConnectionDefinition.Paths = CreatePactPathsDictionary(v4Interaction.Request);
        }

        return connection;
    }

}

