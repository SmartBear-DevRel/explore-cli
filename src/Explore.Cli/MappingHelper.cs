using Explore.Cli.Models;

public static class MappingHelper
{
    public static bool CollectionEntriesNotLimitedToSoap(List<CollectionEntry>? entries)
    {
        if(entries == null)
        {
            return false;
        }

        return entries.Exists(c => c.Type?.ToUpper() != "SOAP" && c.Type?.ToUpper() != "WSDL") ? true : false;
    }

   public static Connection MapInspectorCollectionEntryToExploreConnection(CollectionEntry entry)
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
                        Url = $"{entry.Uri?.Scheme + "://" + entry.Uri?.Host}"
                    }
                },
                Paths = CreatePathsDictionary(entry)
            },
            Settings = new Settings()
            {
                Type = "RestConnectionSettings",
                ConnectTimeout = 30,
                FollowRedirects = true,
                EncodeUrl = true
            },
            Credentials = MapInspectorAuthenticationToCredentials(entry.Authentication)
        };
        
   }

    public static Credentials? MapInspectorAuthenticationToCredentials(string? authentication)
    {
        if(!string.IsNullOrEmpty(authentication))
        {
            var parsedAuth = string.Empty;
            var authenticationKvp = authentication.Split("/");

            if(authentication.Any())
            {
                switch(authenticationKvp[0].ToLowerInvariant())
                {
                    case ("oauth 2.0"):
                        return new Credentials() { Type = "TokenCredentials", Token = $"{authenticationKvp[1]}" };
                     case ("basic authentication"):
                        var basicAuth = authenticationKvp[1].Split(":");
                        return new Credentials() { Type = "BasicAuthCredentials", Username = $"{basicAuth[0]}", Password = $"{basicAuth[1]}"};              
                }
            }
        }

        return null;
    }

    public static string ParseInspectorAuthentication(string authentication)
    {
        var parsedAuth = string.Empty;
        var authenticationKvp = authentication.Split("/");

        if(authentication.Any())
        {
            switch(authenticationKvp[0].ToLowerInvariant())
            {
                case ("oauth 2.0"):
                    parsedAuth = $"Bearer {authenticationKvp[1]}";
                    break;
                case ("basic authentication"):
                    parsedAuth = $"Basic {authenticationKvp[1]}";
                    break;                
            }
        }

        return parsedAuth;
    }

    public static List<Parameter> MapInspectorParamsToExploreParams(CollectionEntry entry)
    {
        List<Parameter> parameters = new List<Parameter>();

        if(entry.Headers != null)
        {
            // map the headers
            foreach(var header in entry.Headers)
            {
                parameters.Add(new Parameter()
                {
                    In = "header",
                    Name = header.Name,
                    Examples = new Examples()
                    {
                        Example = new Example()
                        {
                            Value = header.Value
                        }
                    }
                });
            }

            // map the authorization property
            //if(!string.IsNullOrEmpty(entry.Authentication))
            //{
            //    parameters.Add(new Parameter()
            //    {
            //        In = "Header",
            //        Name = "Authorization",
            //        Examples = new Examples()
            //        {
            //            Example = new Example()
            //            {
            //                Value = ParseInspectorAuthentication(entry.Authentication)
            //            }
            //        }
            //        
            //    });
            //}
        }

        // parse and map the query string
        if(entry.Uri != null)
        {
            if(!string.IsNullOrEmpty(entry.Uri.Query))
            {
                var queryParams = entry.Uri.Query.Split('&');

                foreach(var param in queryParams)
                {
                    var keyValue = param.Split('=');

                    parameters.Add(new Parameter()
                    {
                        In = "query",
                        Name = keyValue[0],
                        Examples = new Examples()
                        {
                            Example = new Example()
                            {
                                Value = keyValue[1] ?? string.Empty
                            }
                        }
                    });
                }
            }
        }

        return parameters;
    }

    public static Components? CreateComponentsFromInspectorAuthentication(string authentication)
    {
        if(!string.IsNullOrEmpty(authentication))
        {
            var parsedAuth = string.Empty;
            var authenticationKvp = authentication.Split("/");

            if(authentication.Any())
            {
                switch(authenticationKvp[0].ToLowerInvariant())
                {
                    case ("oauth 2.0"):
                        return new Components()
                        { 
                            SecuritySchemes = new SecuritySchemes() 
                            { 
                                TokenCredentials = new SecuritySchemeTokenCredentials()
                                {
                                    Type = "http",
                                    Scheme = "bearer"
                                }
                            }
                        };
                     case ("basic authentication"):
                        return new Components()
                        { 
                            SecuritySchemes = new SecuritySchemes() 
                            { 
                                BasicAuthCredentials = new SecuritySchemeBasicAuthCredentials()
                                {
                                    Type = "http",
                                    Scheme = "basic"
                                }
                            }
                        };          
                }
            }            
        }

        return null;
    }
    public static Dictionary<string, object> CreatePathsDictionary(CollectionEntry entry)
    {        
            if(entry.Uri != null)
            {     
                if(!string.IsNullOrEmpty(entry.Method) && !string.IsNullOrEmpty(entry.Uri?.Path))
                {
                    var pathsContent = new PathsContent()
                    {
                        Parameters = MapInspectorParamsToExploreParams(entry)
                    };

                    //add request body
                    if(!string.IsNullOrEmpty(entry.Body))
                    {
                        var examplesJson = new Dictionary<string, object>();
                        examplesJson.Add("examples", MapEntryBodyToContentExamples(entry.Body));

                        var contentJson = new Dictionary<string, object>();
                        contentJson.Add("*/*", examplesJson);

                        pathsContent.RequestBody = new RequestBody()
                        {
                            Content = contentJson
                        };
                    }

                    // add header and query params
                    var methodJson = new Dictionary<string, object>();
                    //methodJson.Add(entry.Method, MapInspectorParamsToExploreParams(entry));
                    methodJson.Add(entry.Method.ToLowerInvariant(), pathsContent);

                    var json = new Dictionary<string, object>();
                    json.Add(entry.Uri.Path, methodJson);

                    return json;
                }            
            }

            return new Dictionary<string, object>();
    }

    public static Examples MapEntryBodyToContentExamples(string body)
    {
            return new Examples()
            {
                Example = new Example()
                {
                    Value = body ?? string.Empty
                }
            };
    }

    public static Connection MassageConnectionExportForImport(Connection? exportedConnection)
    {
        if(exportedConnection == null)
        {
            return new Connection();
        }
        
        //connection type is not set on exports, yet it needed when sending back to Explore
        exportedConnection.Type = "ConnectionRequest";
        
        return exportedConnection;
    }

    public static string? ExtractXSRFTokenFromCookie(string sessionCookie)
    {
        var cookieComponents = sessionCookie.Split(";");
        var xsrfComponent = cookieComponents.FirstOrDefault(x => x.Trim().ToUpperInvariant().StartsWith("XSRF-TOKEN"));
        return xsrfComponent?.Split("=")[1];
    }
}