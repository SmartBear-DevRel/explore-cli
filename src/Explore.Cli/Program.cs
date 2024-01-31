using System.CommandLine;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Explore.Cli.Models;
using Spectre.Console;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        import-postman-collection
        var rootCommand = new RootCommand
        {
            Name = "Explore.CLI",
            Description = "Simple utility CLI for importing data into and out of SwaggerHub Explore"
        };

        var exploreCookie = new Option<string>(name: "--explore-cookie", description: "A valid and active SwaggerHub Explore session cookie") { IsRequired = true };
        exploreCookie.AddAlias("-ec");

        var importFilePath = new Option<string>(name: "--file-path", description: "The path to the file used for importing data") { IsRequired = true };
        importFilePath.AddAlias("-fp");

        var exportFilePath = new Option<string>(name: "--file-path", description: "The path for exporting files. It can be either a relative or absolute path") { IsRequired = false };
        exportFilePath.AddAlias("-fp");

        var exportFileName = new Option<string>(name: "--export-name", description: "The name of the file to export") { IsRequired = false };
        exportFileName.AddAlias("-en");

        var names = new Option<string>(name: "--names", description: "The names of the spaces to export or import") { IsRequired = false };
        names.AddAlias("-n");

        var verbose = new Option<bool?>(name: "--verbose", description: "Include verbose output during processing") { IsRequired = false };
        verbose.AddAlias("-v");

        var exportSpacesCommand = new Command("export-spaces") { exploreCookie, exportFilePath, exportFileName, names, verbose };
        exportSpacesCommand.Description = "Export SwaggerHub Explore spaces to filesystem";
        rootCommand.Add(exportSpacesCommand);

        exportSpacesCommand.SetHandler(async (ec, fp, en, n, v) =>
        { await ExportSpaces(ec, fp, en, n, v); }, exploreCookie, exportFilePath, exportFileName, names, verbose);

        var importSpacesCommand = new Command("import-spaces") { exploreCookie, importFilePath, names, verbose };
        importSpacesCommand.Description = "Import SwaggerHub Explore spaces from a file";
        rootCommand.Add(importSpacesCommand);

        importSpacesCommand.SetHandler(async (ec, fp, v, n) =>
        { await ImportSpaces(ec, fp, v, n); }, exploreCookie, importFilePath, names, verbose);

        var importPostmanCollectionCommand = new Command("import-postman-collection") { exploreCookie, importFilePath, verbose };
        importPostmanCollectionCommand.Description = "Import a Postman collection into SwaggerHub Explore";
        rootCommand.Add(importPostmanCollectionCommand);

        importPostmanCollectionCommand.SetHandler(async (ec, fp, v) =>
        { await ImportPostmanCollection(ec, fp, v); }, exploreCookie, importFilePath, verbose);
        
        AnsiConsole.Write(new FigletText("Explore.Cli").Color(new Color(133, 234, 45)));

        return await rootCommand.InvokeAsync(args);
    }

    internal static async Task ImportPostmanCollection(string exploreCookie, string filePath, bool? verboseOutput)
    {
        //check file existence and read permissions
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                //let's verify it's a JSON file now
                if (!UtilityHelper.IsJsonFile(filePath))
                {
                    AnsiConsole.MarkupLine($"[red]The file provided is not a JSON file. Please review.[/]");
                    return;
                }

                // You can read from the file if this point is reached
                AnsiConsole.MarkupLine($"processing ...");
            }
        }
        catch (UnauthorizedAccessException)
        {
            AnsiConsole.MarkupLine($"[red]Access to {filePath} is denied. Please review file permissions any try again.[/]");
            return;
        }
        catch (FileNotFoundException)
        {
            AnsiConsole.MarkupLine($"[red]The file {filePath} does not exist. Please review the provided file path.[/]");
            return;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]An error occurred accessing the file: {ex.Message}[/]");
            return;
        }

        try 
        {
            //validate collection against postman collection schema
            string json = File.ReadAllText(filePath);
            var postmanCollection = JsonSerializer.Deserialize<PostmanCollection>(json);

            //validate json against known (high level) schema
            var validationResult = await UtilityHelper.ValidateSchema(json, "postman");

            if (!validationResult.isValid)
            {
                Console.WriteLine($"The provide json does not conform to the expected schema. Errors: {validationResult.Message}");
                return;
            }

            var panel = new Panel($"You have [green]{postmanCollection!.Item!.Count} items[/] to import")
            {
                Width = 100,
                Header = new PanelHeader("Postman Collection Data").Centered()
            };
            AnsiConsole.Write(panel);
            Console.WriteLine("");

            var exploreHttpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.explore.swaggerhub.com")
            };
            
            //iterate over the items and import
            if(postmanCollection != null && postmanCollection.Item != null)
            {
                //create an initial space to hold the collection items
                var resultTable = new Table() { Title = new TableTitle(text: $"PROCESSING [green]{postmanCollection.Info?.Name}[/]"), Width = 100, UseSafeBorder = true };
                resultTable.AddColumn("Result");
                resultTable.AddColumn(new TableColumn("Details").Centered());   

                var cleanedCollectionName = UtilityHelper.CleanString(postmanCollection.Info?.Name);
                var spaceContent = new StringContent(JsonSerializer.Serialize(new SpaceRequest() { Name = cleanedCollectionName }), Encoding.UTF8, "application/json");

                exploreHttpClient.DefaultRequestHeaders.Clear();
                exploreHttpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
                exploreHttpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");
                var spacesResponse = await exploreHttpClient.PostAsync("/spaces-api/v1/spaces", spaceContent);        

                if (spacesResponse.StatusCode == HttpStatusCode.Created)
                {
                    var apiImportResults = new Table() { Title = new TableTitle(text: $"SPACE [green]{cleanedCollectionName}[/] CREATED"), Width = 75, UseSafeBorder = true };
                    apiImportResults.AddColumn("Result");
                    apiImportResults.AddColumn("API Imported");
                    apiImportResults.AddColumn("Connection Imported");

                    //parse the response
                    var spaceResponse = spacesResponse.Content.ReadFromJsonAsync<SpaceResponse>();

                    //Postman Items cant contain nested items, so we can flatten the list
                    var flattenedItems = PostmanCollectionMappingHelper.FlattenItems(postmanCollection.Item);
                    
                    foreach(var item in flattenedItems)
                    {
                        if(item.Request != null)
                        {
                            //check if request format is supported
                            if(!PostmanCollectionMappingHelper.IsItemRequestModeSupported(item.Request))
                            {
                                apiImportResults.AddRow("[orange3]skipped[/]", $"Item '{item.Name}' skipped", $"Request method not supported");
                                continue;
                            }
                            
                            //now let's create an API entry in the space
                            var cleanedAPIName = UtilityHelper.CleanString(item.Name);
                            var apiContent = new StringContent(JsonSerializer.Serialize(new ApiRequest() { Name = cleanedAPIName, Type = "REST", Description = $"imported from postman on {DateTime.UtcNow.ToShortDateString()}" }), Encoding.UTF8, "application/json");

                            exploreHttpClient.DefaultRequestHeaders.Clear();
                            exploreHttpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
                            exploreHttpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");
                            var apiResponse = await exploreHttpClient.PostAsync($"/spaces-api/v1/spaces/{spaceResponse.Result?.Id}/apis", apiContent);

                            if (apiResponse.StatusCode == HttpStatusCode.Created)
                            {
                                var createdApiResponse = apiResponse.Content.ReadFromJsonAsync<ApiResponse>();
                                var connectionRequestBody = JsonSerializer.Serialize(PostmanCollectionMappingHelper.MapPostmanCollectionItemToExploreConnection(item));
                                var connectionContent = new StringContent(connectionRequestBody, Encoding.UTF8, "application/json");

                                //now let's do the work and import the connection
                                exploreHttpClient.DefaultRequestHeaders.Clear();
                                exploreHttpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
                                exploreHttpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");
                                var connectionResponse = await exploreHttpClient.PostAsync($"/spaces-api/v1/spaces/{spaceResponse.Result?.Id}/apis/{createdApiResponse.Result?.Id}/connections", connectionContent);

                                if (connectionResponse.StatusCode == HttpStatusCode.Created)
                                {
                                    apiImportResults.AddRow("[green]OK[/]", $"API '{cleanedAPIName}' created", "Connection created");
                                }
                                else
                                {
                                    apiImportResults.AddRow("[orange3]OK[/]", $"API '{cleanedAPIName}' created", "[orange3]Connection NOT created[/]");
                                }
                            }
                            else
                            {
                                apiImportResults.AddRow("[red]NOK[/]", $"API creation failed. StatusCode {apiResponse.StatusCode}", "");
                            }   
                        }
                    }


                    resultTable.AddRow(new Markup("[green]success[/]"), apiImportResults);

                    if (verboseOutput == null || verboseOutput == false)
                    {
                        AnsiConsole.MarkupLine($"[green]\u2713 [/]{cleanedCollectionName}");
                    }                      
                    
                }  
                else
                {
                    switch (spacesResponse.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            // not expecting a 200 OK here - this would be returned for a failed auth and a redirect to SB ID
                            resultTable.AddRow(new Markup("[red]failure[/]"), new Markup($"[red] Auth failed connecting to SwaggerHub Explore. Please review provided cookie.[/]"));
                            AnsiConsole.Write(resultTable);
                            Console.WriteLine("");
                            break;

                        case HttpStatusCode.Conflict:
                            var apiImportResults = new Table() { Title = new TableTitle(text: $"[orange3]SPACE[/] {cleanedCollectionName} [orange3]ALREADY EXISTS[/]") };
                            apiImportResults.AddColumn("Result");
                            apiImportResults.AddColumn("API Imported");
                            apiImportResults.AddColumn("Connection Imported");
                            apiImportResults.AddRow("skipped", "", "");

                            resultTable.AddRow(new Markup("[orange3]skipped[/]"), apiImportResults);
                            AnsiConsole.Write(resultTable);
                            Console.WriteLine("");
                            break;

                        default:
                            resultTable.AddRow(new Markup("[red]failure[/]"), new Markup($"[red] StatusCode: {spacesResponse.StatusCode}, Details: {spacesResponse.ReasonPhrase}[/]"));
                            AnsiConsole.Write(resultTable);
                            Console.WriteLine("");
                            break;
                    }
                }

                if (verboseOutput != null && verboseOutput == true)
                {
                    AnsiConsole.Write(resultTable);
                }                    

            }

            Console.WriteLine();
            AnsiConsole.MarkupLine($"[green]Import completed[/]");            
            
            //ToDo - deal with scenario of item-groups
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("File not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

    }

    internal static async Task ExportSpaces(string exploreCookie, string filePath, string exportFileName, string names, bool? verboseOutput)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.explore.swaggerhub.com")
        };

        httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");

        //get spaces
        var spacesResponse = await httpClient.GetAsync("/spaces-api/v1/spaces?page=0&size=2000");

        if (spacesResponse.StatusCode == HttpStatusCode.OK)
        {
            //ensure expected spaces response (not silent redirect to Auth provider)
            if (!UtilityHelper.IsContentTypeExpected(spacesResponse.Content.Headers, "application/hal+json"))
            {
                AnsiConsole.MarkupLine($"[red]Please review your credentials, Unexpected response GET spaces endpoint[/]");
                return;
            }

            var namesList = names?.Split(',')
                .Select(name => name.Trim())
                .ToList();
            var spaces = await spacesResponse.Content.ReadFromJsonAsync<PagedSpaces>();
            var panel = new Panel($"You have [green]{spaces!.Embedded!.Spaces!.Count} spaces[/] in explore");
            panel.Width = 100;
            panel.Header = new PanelHeader("SwaggerHub Explore Data").Centered();

            // validate the file name if provided
            if (string.IsNullOrEmpty(exportFileName))
            {
                // use default if not provided
                exportFileName = "ExploreSpaces.json";
            }
            else if (!UtilityHelper.IsValidFileName(ref exportFileName))
            {
                return; // file name is invalid, exit
            }

            // validate the export path if provided
            // string filePath;
            if (string.IsNullOrEmpty(filePath))
            {
                // use default (current directory) if not provided
                filePath = Environment.CurrentDirectory;
            }
            else if (!UtilityHelper.IsValidFilePath(ref filePath))
            {
                return; // file path is invalid, exit
            }

            // combine the path and filename
            filePath = Path.Combine(filePath, exportFileName);

            AnsiConsole.Write(panel);
            Console.WriteLine(namesList?.Count > 0 ? $"Exporting spaces: {string.Join(", ", namesList)}" : "Exporting all spaces");
            Console.WriteLine("processing...");

            var spacesToExport = new List<ExploreSpace>();

            foreach (var space in spaces.Embedded.Spaces)
            {
                if (namesList?.Count > 0 && space.Name != null && !namesList.Contains(space.Name))
                {
                    AnsiConsole.MarkupLine($"[orange3]'Skipped {space.Name}': Name not found in list of names to export[/]");
                    continue;
                }

                var resultTable = new Table() { Title = new TableTitle(text: $"PROCESSING [green]{space.Name}[/]"), Width = 100, UseSafeBorder = true };
                resultTable.AddColumn("Result");
                resultTable.AddColumn(new TableColumn("Details").Centered());


                var spaceToExport = new ExploreSpace() { Name = space.Name, Id = space.Id };

                // get the APIs
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
                httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");

                //get space APIs
                var apisResponse = await httpClient.GetAsync($"/spaces-api/v1/spaces/{space.Id}/apis?page=0&size=2000");

                if (apisResponse.StatusCode == HttpStatusCode.OK)
                {
                    var apiImportResults = new Table() { Title = new TableTitle(text: $"Processing [green]APIs[/]"), Width = 75, UseSafeBorder = true };
                    apiImportResults.AddColumn("Result");
                    apiImportResults.AddColumn("APIs Processed");
                    apiImportResults.AddColumn("Connections Processed");

                    var spaceAPIs = new List<ExploreApi>();
                    var apis = await apisResponse.Content.ReadFromJsonAsync<PagedApis>();

                    if (apis?.Embedded != null)
                    {
                        foreach (var api in apis!.Embedded!.Apis!)
                        {
                            if (string.Equals(api.Type, "REST", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var apiToExport = new ExploreApi() { Id = api.Id, Name = api.Name, Type = api.Type };

                                // get the API connections
                                httpClient.DefaultRequestHeaders.Clear();
                                httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
                                httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");
                                var connectionsResponse = await httpClient.GetAsync($"/spaces-api/v1/spaces/{space.Id}/apis/{api.Id}/connections?page=0&size=2000");

                                if (connectionsResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    var connections = await connectionsResponse.Content.ReadFromJsonAsync<PagedConnections>();
                                    apiToExport.connections = new List<Connection>();

                                    foreach (var connection in connections!.Embedded!.Connections!)
                                    {
                                        apiToExport.connections.Add(connection);

                                        apiImportResults.AddRow("[green]OK[/]", $"API '{api.Name}' processed", $"Connection {connection.Name} processed");
                                    }
                                }

                                spaceAPIs.Add(apiToExport);
                            }
                            else
                            {
                                apiImportResults.AddRow("[orange3]skipped[/]", $"API '{api.Name}' skipped", $"Kafka not yet supported by export");
                            }

                        }

                        spaceToExport.apis = spaceAPIs;
                        spacesToExport.Add(spaceToExport);
                    }
                    else
                    {
                        apiImportResults.AddRow("[orange3]skipped[/]", $"Space '{space.Name}' skipped", $"No APIs to export");
                    }

                    resultTable.AddRow(new Markup("[green]success[/]"), apiImportResults);
                }

                if (verboseOutput != null && verboseOutput == true)
                {
                    AnsiConsole.Write(resultTable);
                }
            }

            if (spacesToExport.Count == 0)
            {
                AnsiConsole.MarkupLine($"[orange3]No spaces matched the provided names[/]");
                return;
            }

            // construct the export object
            var export = new ExportSpaces()
            {
                Info = new Info() { ExportedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                ExploreSpaces = spacesToExport
            };

            // export the file
            string exploreSpacesJson = JsonSerializer.Serialize(export);
            try
            {
                using StreamWriter streamWriter = new StreamWriter(filePath);
                streamWriter.Write(exploreSpacesJson);
            }
            catch (UnauthorizedAccessException)
            {
                AnsiConsole.MarkupLine($"[red]Access to {filePath} is denied. Please review file permissions any try again.[/]");
                return;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred accessing the file: {ex.Message}[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[green] All done! {spacesToExport.Count()} of {spaces!.Embedded!.Spaces!.Count} spaces exported to: {filePath} [/]");
        }
    }

    internal static async Task ImportSpaces(string exploreCookie, string filePath, string names, bool? verboseOutput)
    {
        //check file existence and read permissions
        try
        {
            using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            //let's verify it's a JSON file now
            if (!UtilityHelper.IsJsonFile(filePath))
            {
                AnsiConsole.MarkupLine($"[red]The file provided is not a JSON file. Please review.[/]");
                return;
            }

            // You can read from the file if this point is reached
            AnsiConsole.MarkupLine($"processing ...");
        }
        catch (UnauthorizedAccessException)
        {
            AnsiConsole.MarkupLine($"[red]Access to {filePath} is denied. Please review file permissions any try again.[/]");
            return;
        }
        catch (FileNotFoundException)
        {
            AnsiConsole.MarkupLine($"[red]The file {filePath} does not exist. Please review the provided file path.[/]");
            return;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]An error occurred accessing the file: {ex.Message}[/]");
            return;
        }

        var namesList = names?.Split(',')
                        .Select(name => name.Trim())
                        .ToList();

        //Read and serialize
        try
        {
            string json = File.ReadAllText(filePath);
            var exportedSpaces = JsonSerializer.Deserialize<ExportSpaces>(json);

            //validate json against known (high level) schema
            var validationResult = await UtilityHelper.ValidateSchema(json, "explore");

            if (!validationResult.isValid)
            {
                Console.WriteLine($"The provided JSON does not conform to the expected schema. Errors: {validationResult.Message}");
                return;
            }

            var panel = new Panel($"You have [green]{exportedSpaces!.ExploreSpaces!.Count} spaces[/] to import")
            {
                Width = 100,
                Header = new PanelHeader("SwaggerHub Explore Data").Centered()
            };
            AnsiConsole.Write(panel);
            Console.WriteLine("");

            //iterate over spaces
            if (exportedSpaces != null && exportedSpaces.ExploreSpaces != null)
            {
                foreach (var exportedSpace in exportedSpaces.ExploreSpaces)
                {
                    // check if the space name is in the list of names to import
                    if (namesList?.Count > 0 && exportedSpace.Name != null && !namesList.Contains(exportedSpace.Name))
                    {
                        AnsiConsole.MarkupLine($"[orange3]'Skipped {exportedSpace.Name}': Name not found in list of names to import[/]");
                        continue;
                    }

                    var spaceExists = await CheckSpaceExists(exploreCookie, exportedSpace.Id?.ToString(), verboseOutput);

                    var resultTable = new Table() { Title = new TableTitle(text: $"PROCESSING [green]{exportedSpace.Name}[/]"), Width = 100, UseSafeBorder = true };
                    resultTable.AddColumn("Result");
                    resultTable.AddColumn(new TableColumn("Details").Centered());

                    var importedSpace = await UpsertSpace(exploreCookie, spaceExists, exportedSpace.Name, exportedSpace.Id.ToString());

                    if (!string.IsNullOrEmpty(importedSpace.Name))
                    {
                        var apiImportResults = new Table() { Title = new TableTitle(text: $"Importing Space [green]{importedSpace.Name}[/]"), Width = 75, UseSafeBorder = true };
                        apiImportResults.AddColumn("Result");
                        apiImportResults.AddColumn("API Imported");
                        apiImportResults.AddColumn("Connection Imported");

                        //iterate over APIs
                        if (exportedSpace.apis != null)
                        {
                            foreach (var exportedAPI in exportedSpace.apis) //add type filter for now
                            {
                                if (string.Equals(exportedAPI.Type, "REST", StringComparison.OrdinalIgnoreCase))
                                {
                                    //remark - Improve DTO mapping here
                                    var importedApi = await UpsertApi(exploreCookie, spaceExists, importedSpace.Id.ToString(), exportedAPI.Id.ToString(), exportedAPI.Name, exportedAPI.Type, verboseOutput);

                                    if (!string.IsNullOrEmpty(importedApi.Name))
                                    {
                                        apiImportResults.AddRow("[green]OK[/]", $"API '{importedApi.Name}' imported", "");
                                        //iterate over Connections
                                        if (exportedAPI.connections != null)
                                        {
                                            foreach (var exportedConnection in exportedAPI.connections) //add type filter for now
                                            {
                                                var importedConnection = await UpsertConnection(exploreCookie, spaceExists, importedSpace.Id.ToString(), importedApi.Id.ToString(), exportedConnection?.Id?.ToString(), exportedConnection, verboseOutput);

                                                if (importedConnection)
                                                {
                                                    apiImportResults.AddRow("[green]OK[/]", "", $"Connection '{exportedConnection?.Name}' imported");
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    apiImportResults.AddRow("[orange3]skipped[/]", $"API '{exportedAPI.Name}' skipped", $"Kafka not yet supported by export");
                                }                                
                            }
                        }

                        resultTable.AddRow(new Markup("[green]success[/]"), apiImportResults);

                        if (verboseOutput == null || verboseOutput == false)
                        {
                            AnsiConsole.MarkupLine($"[green]\u2713 [/]{importedSpace.Name}");
                        }
                    }

                    if (verboseOutput != null && verboseOutput == true)
                    {
                        AnsiConsole.Write(resultTable);
                    }
                }
            }

            // Now, 'obj' contains the deserialized data
            Console.WriteLine();
            AnsiConsole.MarkupLine($"[green]Import completed[/]");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("File not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static async Task<bool> CheckSpaceExists(string exploreCookie, string? id, bool? verboseOutput)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.explore.swaggerhub.com")
        };

        httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");

        var spacesResponse = await httpClient.GetAsync($"/spaces-api/v1/spaces/{id}");

        if (spacesResponse.StatusCode == HttpStatusCode.OK)
        {
            if (!UtilityHelper.IsContentTypeExpected(spacesResponse.Content.Headers, "application/hal+json"))
            {
                AnsiConsole.MarkupLine($"[red]Please review your credentials, Unexpected response GET spaces endpoint[/]");
                throw new HttpRequestException("Please review your credentials, Unexpected response GET spaces endpoint");
            }

            return true;
        }

        if (verboseOutput != null && verboseOutput == true)
        {
            AnsiConsole.MarkupLine($"[orange3]StatusCode {spacesResponse.StatusCode} returned from the GetSpaceById API. New space will be created[/]");
        }

        return false;
    }

    private static async Task<bool> CheckApiExists(string exploreCookie, string spaceId, string? id, bool? verboseOutput)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.explore.swaggerhub.com")
        };

        httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");

        var spacesResponse = await httpClient.GetAsync($"/spaces-api/v1/spaces/{spaceId}/apis/{id}");

        if (spacesResponse.StatusCode == HttpStatusCode.OK)
        {
            return true;
        }

        if (verboseOutput != null && verboseOutput == true)
        {
            AnsiConsole.MarkupLine($"[orange3]StatusCode {spacesResponse.StatusCode} returned from the GetApiById API. New API will be created in the space.[/]");
        }

        return false;
    }

    private static async Task<bool> CheckConnectionExists(string exploreCookie, string spaceId, string apiId, string? id, bool? verboseOutput)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.explore.swaggerhub.com")
        };

        httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");

        var response = await httpClient.GetAsync($"/spaces-api/v1/spaces/{spaceId}/apis/{apiId}/connections/{id}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return true;
        }

        if (verboseOutput != null && verboseOutput == true)
        {
            AnsiConsole.MarkupLine($"[orange3]StatusCode {response.StatusCode} returned from the GetConnectionById API. New connection within API will be created.[/]");
        }

        return false;
    }

    private static async Task<SpaceResponse> UpsertSpace(string exploreCookie, bool spaceExists, string? name, string? id)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.explore.swaggerhub.com")
        };

        var spaceContent = new StringContent(JsonSerializer.Serialize(
            new SpaceRequest()
            {
                Name = name
            }
        ), Encoding.UTF8, "application/json");

        httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");

        HttpResponseMessage? spacesResponse;

        if (string.IsNullOrEmpty(id) || !spaceExists)
        {
            spacesResponse = await httpClient.PostAsync("/spaces-api/v1/spaces", spaceContent);
        }
        else
        {
            spacesResponse = await httpClient.PutAsync($"/spaces-api/v1/spaces/{id}", spaceContent);

            if (spacesResponse.StatusCode == HttpStatusCode.Conflict)
            {
                // swallow 409 as server is being overly strict
                return new SpaceResponse() { Id = Guid.Parse(id), Name = name };
            }
        }

        if (spacesResponse.IsSuccessStatusCode)
        {
            return await spacesResponse.Content.ReadFromJsonAsync<SpaceResponse>() ?? new SpaceResponse();
        }

        if (!UtilityHelper.IsContentTypeExpected(spacesResponse.Content.Headers, "application/hal+json") && !UtilityHelper.IsContentTypeExpected(spacesResponse.Content.Headers, "application/json"))
        {
            AnsiConsole.MarkupLine($"[red]Please review your credentials, Unexpected response from POST/PUT spaces API for name: {name}, id:{id}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]StatusCode {spacesResponse.StatusCode} returned from the POST/PUT spaces API for name: {name}, id:{id}[/]");
        }

        return new SpaceResponse();
    }

    private static async Task<ApiResponse> UpsertApi(string exploreCookie, bool spaceExists, string spaceId, string? id, string? name, string? type, bool? verboseOutput)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.explore.swaggerhub.com")
        };

        var apiContent = new StringContent(JsonSerializer.Serialize(
            new ApiRequest()
            {
                Name = name,
                Type = type
            }
        ), Encoding.UTF8, "application/json");

        httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");

        HttpResponseMessage? apiResponse;

        if (spaceExists && await CheckApiExists(exploreCookie, spaceId, id, verboseOutput))
        {
            // update the api
            apiResponse = await httpClient.PutAsync($"/spaces-api/v1/spaces/{spaceId}/apis/{id}", apiContent);

            if (apiResponse.StatusCode == HttpStatusCode.Conflict)
            {
                // swallow 409 as server is being overly strict
                return new ApiResponse() { Id = Guid.Parse(id ?? string.Empty), Name = name, Type = type };
            }
        }
        else
        {
            //create the api
            apiResponse = await httpClient.PostAsync($"/spaces-api/v1/spaces/{spaceId}/apis", apiContent);
        }

        if (apiResponse.IsSuccessStatusCode)
        {
            return await apiResponse.Content.ReadFromJsonAsync<ApiResponse>() ?? new ApiResponse();
        }

        if (!UtilityHelper.IsContentTypeExpected(apiResponse.Content.Headers, "application/hal+json") && !UtilityHelper.IsContentTypeExpected(apiResponse.Content.Headers, "application/json"))
        {
            AnsiConsole.MarkupLine($"[red]Please review your credentials, Unexpected response from POST/PUT spaces API for name: {name}, id:{id}[/]");
        }
        else
        {
            AnsiConsole.WriteLine($"[red]StatusCode {apiResponse.StatusCode} returned from the POST spaces/{{id}}/apis for {name}[/]");
        }

        return new ApiResponse();
    }

    private static async Task<bool> UpsertConnection(string exploreCookie, bool spaceExists, string spaceId, string apiId, string? connectionId, Connection? connection, bool? verboseOutput)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.explore.swaggerhub.com")
        };

        httpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
        httpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{UtilityHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");

        var connectionContent = new StringContent(JsonSerializer.Serialize(MappingHelper.MassageConnectionExportForImport(connection)), Encoding.UTF8, "application/json");

        HttpResponseMessage? connectionResponse;

        if (spaceExists && await CheckConnectionExists(exploreCookie, spaceId, apiId, connectionId, verboseOutput))
        {
            connectionResponse = await httpClient.PutAsync($"/spaces-api/v1/spaces/{spaceId}/apis/{apiId}/connections/{connectionId}", connectionContent);
        }
        else
        {
            connectionResponse = await httpClient.PostAsync($"/spaces-api/v1/spaces/{spaceId}/apis/{apiId}/connections", connectionContent);
        }

        if (connectionResponse.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            if (!UtilityHelper.IsContentTypeExpected(connectionResponse.Content.Headers, "application/hal+json") && !UtilityHelper.IsContentTypeExpected(connectionResponse.Content.Headers, "application/json"))
            {
                AnsiConsole.MarkupLine($"[red]Please review your credentials, Unexpected response from the connections API for api: {apiId} and {connection?.Name}[/]");
            }
            else
            {
                AnsiConsole.WriteLine($"[red]StatusCode {connectionResponse.StatusCode} returned from the connections API for api: {apiId} and {connection?.Name}[/]");

                var message = await connectionResponse.Content.ReadAsStringAsync();
                AnsiConsole.WriteLine($"error: {message}");
            }
        }
        return false;
    }
}