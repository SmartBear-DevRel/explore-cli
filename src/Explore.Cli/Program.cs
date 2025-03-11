using System.CommandLine;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Linq;
using Spectre.Console;
using Explore.Cli.Models.Explore;
using Explore.Cli.Models.Postman;
using Explore.Cli.Models.Insomnia;
using Explore.Cli.ExploreHttpClient;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
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
        importPostmanCollectionCommand.Description = "Import Postman collection (v2.1) into SwaggerHub Explore";
        rootCommand.Add(importPostmanCollectionCommand);

        importPostmanCollectionCommand.SetHandler(async (ec, fp, v) =>
        { await ImportPostmanCollection(ec, fp, v); }, exploreCookie, importFilePath, verbose);

        var importInsomniaCollectionCommand = new Command("import-insomnia-collection") { exploreCookie, importFilePath, verbose };
        importInsomniaCollectionCommand.Description = "Import Insomnia collection (v4) into SwaggerHub Explore";
        rootCommand.Add(importInsomniaCollectionCommand);

        importInsomniaCollectionCommand.SetHandler(async (ec, fp, v) =>
        { await ImportInsomniaCollection(ec, fp, v); }, exploreCookie, importFilePath, verbose);

        var baseUri = new Option<string>(name: "--base-uri", description: "The base uri to use for all imported requests. ie: http://localhost:3000") { IsRequired = false };
        baseUri.AddAlias("-b");
        var ignorePactFileSchemaValidationResult = new Option<bool>(name: "--ignore-pact-schema-verification-result", description: "Ignore pact schema verification result, performed prior to upload") { IsRequired = false };
        var importPactFileCommand = new Command("import-pact-file") { exploreCookie, importFilePath, baseUri, verbose, ignorePactFileSchemaValidationResult };
        importPactFileCommand.Description = "Import a Pact file (v2/v3/v4) into SwaggerHub Explore (HTTP interactions only)";
        rootCommand.Add(importPactFileCommand);

        importPactFileCommand.SetHandler(async (ec, fp, b, v, ignorePactFileSchemaValidationResult) =>
        { await ImportPactFile(ec, fp, b, v, ignorePactFileSchemaValidationResult); }, exploreCookie, importFilePath, baseUri, verbose, ignorePactFileSchemaValidationResult);

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

            if (!PostmanCollectionMappingHelper.IsCollectionVersion2_1(json))
            {
                Console.WriteLine($"The provided JSON does not conform to the expected schema. Errors: Only Postman Collection v2.1 are supported at this time.");
                return;
            }

            var postmanCollection = JsonSerializer.Deserialize<PostmanCollection>(json);

            //validate json against known (high level) schema
            var validationResult = await UtilityHelper.ValidateSchema(json, "postman");

            if (!validationResult.isValid)
            {
                Console.WriteLine($"The provided json does not conform to the expected schema. Errors: {validationResult.Message}");
                return;
            }

            var panel = new Panel($"You have [green]{postmanCollection!.Item!.Count} items[/] to import")
            {
                Width = 100,
                Header = new PanelHeader("Postman Collection Data").Centered()
            };
            AnsiConsole.Write(panel);
            Console.WriteLine("");

            var exploreHttpClient = new ExploreHttpClient();

            //iterate over the items and import
            if (postmanCollection != null && postmanCollection.Item != null)
            {
                //create an initial space to hold the collection items
                var resultTable = new Table() { Title = new TableTitle(text: $"PROCESSING [green]{postmanCollection.Info?.Name}[/]"), Width = 100, UseSafeBorder = true };
                resultTable.AddColumn("Result");
                resultTable.AddColumn(new TableColumn("Details").Centered());

                var cleanedCollectionName = UtilityHelper.CleanString(postmanCollection.Info?.Name);
                var createSpacesResult = await exploreHttpClient.CreateSpace(exploreCookie, cleanedCollectionName);

                if (createSpacesResult.Result)
                {
                    var apiImportResults = new Table() { Title = new TableTitle(text: $"SPACE [green]{cleanedCollectionName}[/] CREATED"), Width = 75, UseSafeBorder = true };
                    apiImportResults.AddColumn("Result");
                    apiImportResults.AddColumn("API Imported");
                    apiImportResults.AddColumn("Connection Imported");

                    var apisToImport = PostmanCollectionMappingHelper.MapPostmanCollectionToStagedAPI(postmanCollection, cleanedCollectionName);

                    foreach (var item in apisToImport)
                    {
                        if (item.APIName == null || item.Connections == null)
                        {
                            apiImportResults.AddRow("[orange3]skipped[/]", $"API '{item.APIName ?? "Unknown"}' skipped", $"No supported request found in collection");
                            continue;
                        }

                        AnsiConsole.MarkupLine($"Processing API: {item.APIName} with {item.Connections.Count} connections");

                        if(item.Connections == null || item.Connections.Count == 0)
                        {
                            apiImportResults.AddRow("[orange3]skipped[/]", $"API '{item.APIName}' skipped", $"No supported request found in collection");
                            continue;
                        }

                        //now let's create an API entry in the space
                        var cleanedAPIName = UtilityHelper.CleanString(item.APIName);
                        //var description = item.Connections.FirstOrDefault(c => c.Description != null)?.Description?.Content;

                        var createApiEntryResult = await exploreHttpClient.CreateApiEntry(exploreCookie, createSpacesResult.Id, cleanedAPIName, "postman", null);
                       
                        if(createApiEntryResult.Result)
                        {
                            foreach(var connection in item.Connections)
                            {
                                var connectionRequestBody = JsonSerializer.Serialize(connection);
                                //now let's do the work and import the connection
                                var createConnectionResponse = await exploreHttpClient.CreateApiConnection(exploreCookie, createSpacesResult.Id, createApiEntryResult.Id, connectionRequestBody);
    
                                if (createConnectionResponse.Result)
                                {
                                    apiImportResults.AddRow("[green]OK[/]", $"API '{cleanedAPIName}' created", "Connection created");
                                }
                                else
                                {
                                    apiImportResults.AddRow("[orange3]OK[/]", $"API '{cleanedAPIName}' created", "[orange3]Connection NOT created[/]");
                                }
                            }
                        }
                        else
                        {
                            apiImportResults.AddRow("[red]NOK[/]", $"API creation failed. StatusCode {createApiEntryResult.StatusCode}", "");
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
                    switch (createSpacesResult.Reason)
                    {
                        case "AUTH_REQUIRED":
                            // not expecting a 200 OK here - this would be returned for a failed auth and a redirect to SB ID
                            resultTable.AddRow(new Markup("[red]failure[/]"), new Markup($"[red] Auth failed connecting to SwaggerHub Explore. Please review provided cookie.[/]"));
                            AnsiConsole.Write(resultTable);
                            Console.WriteLine("");
                            break;

                        case "SPACE_CONFLICT":
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
                            resultTable.AddRow(new Markup("[red]failure[/]"), new Markup($"[red] StatusCode: {createSpacesResult.StatusCode}, Details: {createSpacesResult.Reason}[/]"));
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
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"File not found: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

    }

    internal static async Task ImportPactFile(string exploreCookie, string filePath, string pactBaseUri, bool? verboseOutput, bool ignorePactFileSchemaValidationResult = false)
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
            // validate collection against pact schemas
            // https://github.com/pactflow/pact-schemas
            Console.WriteLine($"Reading file");
            string json = File.ReadAllText(filePath);

            if (!PactMappingHelper.hasPactVersion(json))
            {
                Console.WriteLine($"Cannot determine pact specification version");
                return;
            }
            var pactSpecVersion = PactMappingHelper.getPactVersion(json);
            dynamic pactContract;
            switch (pactSpecVersion)
            {
                case "pact-v1":
                    pactContract = PactV1.Contract.FromJson(json);
                    break;
                case "pact-v2":
                    pactContract = PactV2.Contract.FromJson(json);
                    break;
                case "pact-v3":
                    pactContract = PactV3.Contract.FromJson(json);
                    break;
                case "pact-v4":
                    pactContract = PactV4.Contract.FromJson(json);
                    break;
                default:
                    throw new Exception($"Invalid pact specification version");
            }
            //validate json against known (high level) schema
            var validationResult = await UtilityHelper.ValidateSchema(json, pactSpecVersion);

            if (!validationResult.isValid && ignorePactFileSchemaValidationResult)
            {
                Console.WriteLine($"WARN: The provided pact file does not conform to the expected schema. Errors: {validationResult.Message}");
            }
            else if (!validationResult.isValid)
            {
                throw new Exception($"The provided pact file does not conform to the expected schema. Errors: {validationResult.Message}");
            }

            int interactionCount = 0;
            switch (pactContract)
            {
                case PactV1.Contract:
                    PactV1.Contract pactV1Contract = pactContract;
                    pactSpecVersion = pactV1Contract.Metadata.MetadataPactSpecification.Version;
                    interactionCount = pactV1Contract.Interactions.Count();
                    break;
                case PactV2.Contract:
                    PactV2.Contract pactV2Contract = pactContract;
                    pactSpecVersion = pactV2Contract.Metadata.MetadataPactSpecification.Version;
                    interactionCount = pactV2Contract.Interactions.Count();
                    break;
                case PactV3.Contract:
                    PactV3.Contract pactV3Contract = pactContract;
                    pactSpecVersion = pactV3Contract.Metadata.MetadataPactSpecification.Version;
                    interactionCount = pactV3Contract.Interactions.Count();
                    break;
                case PactV4.Contract:
                    PactV4.Contract pactV4Contract = pactContract;
                    pactSpecVersion = pactV4Contract.Metadata.PactSpecification.Version;
                    interactionCount = pactV4Contract.Interactions.Count();
                    break;
                default:
                    throw new Exception($"The provided pact file is unsupported.");
            };

            var panel = new Panel($"You have [green]{interactionCount} items[/] to import")
            {
                Width = 100,
                Header = new PanelHeader("Pact Data").Centered()
            };
            AnsiConsole.Write(panel);
            Console.WriteLine("");

            var exploreHttpClient = new ExploreHttpClient();
            // iterate over the items and import
            if (pactContract != null && pactContract?.Interactions != null)
            {

                var resultTable = new Table() { Title = new TableTitle(text: $"PROCESSING [green]{pactContract?.Consumer.Name}-{pactContract?.Provider.Name}[/]"), Width = 100, UseSafeBorder = true };
                resultTable.AddColumn("Result");
                resultTable.AddColumn(new TableColumn("Details").Centered());

                var spaceName = UtilityHelper.CleanString($"{pactContract?.Consumer.Name}-{pactContract?.Provider.Name}");
                var createSpacesResult = await exploreHttpClient.CreateSpace(exploreCookie, spaceName);
                if (createSpacesResult.Result)
                {

                    var apiImportResults = new Table() { Title = new TableTitle(text: $"SPACE [green]{spaceName}[/] CREATED"), Width = 75, UseSafeBorder = true };
                    apiImportResults.AddColumn("Result");
                    apiImportResults.AddColumn("API Imported");
                    apiImportResults.AddColumn("Connection Imported");

                    var interactions = (pactContract?.Interactions) ?? throw new Exception("No interactions found in Pact file");
                    if (interactions.Count == 0)
                    {
                        throw new Exception("No interactions found in Pact file");
                    }

                    foreach (var interaction in interactions)
                    {
                        if (interaction.Request != null)
                        {
                            if (interaction is PactV4.Interaction pactV4Interaction)
                            {
                                if (pactV4Interaction.Type.GetValueOrDefault() != PactV4.InteractionType.SynchronousHttp)
                                {
                                    Console.WriteLine($"Skipping interaction, as {pactV4Interaction.Type} is not Synchronous/HTTP");
                                    break;
                                }
                            }
                            //now let's create an API entry in the space
                            var cleanedAPIName = UtilityHelper.CleanString(interaction.Description.ToString());
                            var createApiEntryResult = await exploreHttpClient.CreateApiEntry(exploreCookie, createSpacesResult.Id, cleanedAPIName, "pact file", $"Pact Specification: {pactSpecVersion}");
                            if (createApiEntryResult.Result)
                            {
                                var connectionRequestBody = JsonSerializer.Serialize(PactMappingHelper.MapPactInteractionToExploreConnection(interaction, pactBaseUri));

                                // //now let's do the work and import the connection
                                var createConnectionResponse = await exploreHttpClient.CreateApiConnection(exploreCookie, createSpacesResult.Id, createApiEntryResult.Id, connectionRequestBody);

                                if (createConnectionResponse.Result)
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
                                apiImportResults.AddRow("[red]NOK[/]", $"API creation failed. StatusCode {createApiEntryResult.StatusCode}", "");
                            }
                        }
                    }


                    resultTable.AddRow(new Markup("[green]success[/]"), apiImportResults);

                    if (verboseOutput == null || verboseOutput == false)
                    {
                        AnsiConsole.MarkupLine($"[green]\u2713 [/]{spaceName}");
                    }
                }
                else
                {
                    switch (createSpacesResult.Reason)
                    {
                        case "AUTH_REQUIRED":
                            // not expecting a 200 OK here - this would be returned for a failed auth and a redirect to SB ID
                            resultTable.AddRow(new Markup("[red]failure[/]"), new Markup($"[red] Auth failed connecting to SwaggerHub Explore. Please review provided cookie.[/]"));
                            AnsiConsole.Write(resultTable);
                            Console.WriteLine("");
                            break;
                        case "SPACE_CONFLICT":
                            var apiImportResults = new Table() { Title = new TableTitle(text: $"[orange3]SPACE[/] {spaceName} [orange3]ALREADY EXISTS[/]") };
                            apiImportResults.AddColumn("Result");
                            apiImportResults.AddColumn("API Imported");
                            apiImportResults.AddColumn("Connection Imported");
                            apiImportResults.AddRow("skipped", "", "");

                            resultTable.AddRow(new Markup("[orange3]skipped[/]"), apiImportResults);
                            AnsiConsole.Write(resultTable);
                            Console.WriteLine("");
                            break;
                        default:
                            resultTable.AddRow(new Markup("[red]failure[/]"), new Markup($"[red] StatusCode: {createSpacesResult.StatusCode}, Details: {createSpacesResult.Reason}[/]"));
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
            else
            {
                throw new Exception($"No interactions found");
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

    internal static async Task ImportInsomniaCollection(string exploreCookie, string filePath, bool? verboseOutput)
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
            string json = File.ReadAllText(filePath);

            if (!InsomniaCollectionMappingHelper.IsCollectionExportVersion4(json))
            {
                Console.WriteLine($"The provided JSON does not conform to the expected schema. Errors: Only Insomnia collection exports using `__export_version: 4` are supported at this time.");
                return;
            }

            var insomniaCollection = JsonSerializer.Deserialize<InsomniaCollection>(json);

            //validate json against known (high level) schema -- TODO - check if schema exists on web

            var panel = new Panel($"You have [green]{insomniaCollection!.Resources!.Where(r => string.Equals(r.Type, "request", StringComparison.OrdinalIgnoreCase)).Count()} items[/] to import")
            {
                Width = 100,
                Header = new PanelHeader("Insomnia Collection Data").Centered()
            };
            AnsiConsole.Write(panel);
            Console.WriteLine("");

            var exploreHttpClient = new ExploreHttpClient();

            //iterate over the items and import
            if (insomniaCollection != null && insomniaCollection.Resources != null)
            {
                //get the workspace resource which contains the collection name
                var collectionResource = insomniaCollection.Resources.FirstOrDefault(r => string.Equals(r.Type, "workspace", StringComparison.OrdinalIgnoreCase) && string.Equals(r.Scope, "collection", StringComparison.OrdinalIgnoreCase));

                //create an initial space to hold the collection items
                var resultTable = new Table() { Title = new TableTitle(text: $"PROCESSING [green]{collectionResource?.Name}[/]"), Width = 100, UseSafeBorder = true };
                resultTable.AddColumn("Result");
                resultTable.AddColumn(new TableColumn("Details").Centered());

                var cleanedCollectionName = UtilityHelper.CleanString(collectionResource?.Name);
                var createSpacesResult = await exploreHttpClient.CreateSpace(exploreCookie, cleanedCollectionName);
                if (createSpacesResult.Result)
                {
                    var apiImportResults = new Table() { Title = new TableTitle(text: $"SPACE [green]{cleanedCollectionName}[/] CREATED"), Width = 75, UseSafeBorder = true };
                    apiImportResults.AddColumn("Result");
                    apiImportResults.AddColumn("API Imported");
                    apiImportResults.AddColumn("Connection Imported");

                    //separate requests and environment resources
                    var requestResources = insomniaCollection.Resources.Where(r => string.Equals(r.Type, "request", StringComparison.OrdinalIgnoreCase)).ToList();
                    var environmentResources = insomniaCollection.Resources.Where(r => string.Equals(r.Type, "environment", StringComparison.OrdinalIgnoreCase)).ToList();

                    foreach (var resource in requestResources)
                    {
                        if (!InsomniaCollectionMappingHelper.IsItemRequestModeSupported(resource))
                        {
                            apiImportResults.AddRow("[orange3]skipped[/]", $"Resource '{resource.Name}' skipped", $"Request Method or MimeType not supported");
                            continue;
                        }

                        //now let's create an API entry in the space
                        var cleanedAPIName = UtilityHelper.CleanString(resource.Name);
                        var createApiEntryResult = await exploreHttpClient.CreateApiEntry(exploreCookie, createSpacesResult.Id, cleanedAPIName, "Insomnia", null);
                        if (createApiEntryResult.Result)
                        {
                            var connectionRequestBody = JsonSerializer.Serialize(InsomniaCollectionMappingHelper.MapInsomniaRequestResourceToExploreConnection(resource, environmentResources));
                            //Console.WriteLine($"Connection Request Body for {resource.Name}: {connectionRequestBody}");

                            //now let's do the work and import the connection
                            var createConnectionResponse = await exploreHttpClient.CreateApiConnection(exploreCookie, createSpacesResult.Id, createApiEntryResult.Id, connectionRequestBody);

                            if (createConnectionResponse.Result)
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
                            apiImportResults.AddRow("[red]NOK[/]", $"API creation failed. StatusCode {createApiEntryResult.StatusCode}", "");
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
                    switch (createSpacesResult.Reason)
                    {
                        case "AUTH_REQUIRED":
                            // not expecting a 200 OK here - this would be returned for a failed auth and a redirect to SB ID
                            resultTable.AddRow(new Markup("[red]failure[/]"), new Markup($"[red] Auth failed connecting to SwaggerHub Explore. Please review provided cookie.[/]"));
                            AnsiConsole.Write(resultTable);
                            Console.WriteLine("");
                            break;

                        case "SPACE_CONFLICT":
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
                            resultTable.AddRow(new Markup("[red]failure[/]"), new Markup($"[red] StatusCode: {createSpacesResult.StatusCode}, Details: {createSpacesResult.Reason}[/]"));
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
        var exploreHttpClient = new ExploreHttpClient();


        //get spaces
        var spacesResponse = await exploreHttpClient.GetSpaces(exploreCookie);

        if (spacesResponse.Result)
        {
            //ensure expected spaces response (not silent redirect to Auth provider)

            var namesList = names?.Split(',')
                .Select(name => name.Trim())
                .ToList();
            var spaces = spacesResponse.Spaces;

            if (spaces?.Embedded == null){
                AnsiConsole.MarkupLine($"[red]No spaces to export[/]");
                return;
            }
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
                //get space APIs
                var apisResponse = await exploreHttpClient.GetSpaceApis(exploreCookie, space.Id);

                if (apisResponse.Result)
                {
                    var apiImportResults = new Table() { Title = new TableTitle(text: $"Processing [green]APIs[/]"), Width = 75, UseSafeBorder = true };
                    apiImportResults.AddColumn("Result");
                    apiImportResults.AddColumn("APIs Processed");
                    apiImportResults.AddColumn("Connections Processed");

                    var spaceAPIs = new List<ExploreApi>();
                    var apis = apisResponse.Apis;

                    if (apis?.Embedded != null)
                    {
                        foreach (var api in apis!.Embedded!.Apis!)
                        {
                            if (string.Equals(api.Type, "REST", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var apiToExport = new ExploreApi() { Id = api.Id, Name = api.Name, Type = api.Type };

                                // get the API connections
                                var connectionsResponse = await exploreHttpClient.GetApiConnectionsForSpace(exploreCookie, space.Id, api.Id);

                                if (connectionsResponse.Result)
                                {
                                    var connections = connectionsResponse.Connections;
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
        else if (spacesResponse.Reason == "AUTH_REQUIRED")
        {
            AnsiConsole.MarkupLine($"[red]Please review your credentials, Unexpected response GET spaces endpoint[/]");
            return;
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
            var exploreHttpClient = new ExploreHttpClient();
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

                    var spaceExists = await exploreHttpClient.CheckSpaceExists(exploreCookie, exportedSpace.Id?.ToString(), verboseOutput);

                    var resultTable = new Table() { Title = new TableTitle(text: $"PROCESSING [green]{exportedSpace.Name}[/]"), Width = 100, UseSafeBorder = true };
                    resultTable.AddColumn("Result");
                    resultTable.AddColumn(new TableColumn("Details").Centered());

                    var importedSpace = await exploreHttpClient.UpsertSpace(exploreCookie, spaceExists, exportedSpace.Name, exportedSpace.Id.ToString());

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
                                    var importedApi = await exploreHttpClient.UpsertApi(exploreCookie, spaceExists, importedSpace.Id.ToString(), exportedAPI.Id.ToString(), exportedAPI.Name, exportedAPI.Type, verboseOutput);

                                    if (!string.IsNullOrEmpty(importedApi.Name))
                                    {
                                        apiImportResults.AddRow("[green]OK[/]", $"API '{importedApi.Name}' imported", "");
                                        //iterate over Connections
                                        if (exportedAPI.connections != null)
                                        {
                                            foreach (var exportedConnection in exportedAPI.connections) //add type filter for now
                                            {
                                                var importedConnection = await exploreHttpClient.UpsertConnection(exploreCookie, spaceExists, importedSpace.Id.ToString(), importedApi.Id.ToString(), exportedConnection?.Id?.ToString(), exportedConnection, verboseOutput);

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

}