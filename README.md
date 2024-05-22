# explore-cli
Simple utility CLI for importing data into SwaggerHub Explore.


```
  _____                  _                              ____   _   _
 | ____| __  __  _ __   | |   ___    _ __    ___       / ___| | | (_)
 |  _|   \ \/ / | '_ \  | |  / _ \  | '__|  / _ \     | |     | | | |
 | |___   >  <  | |_) | | | | (_) | | |    |  __/  _  | |___  | | | |
 |_____| /_/\_\ | .__/  |_|  \___/  |_|     \___| (_)  \____| |_| |_|
                |_|
```
**Description:**
>   Simple utility CLI for importing data into and out of SwaggerHub Explore

**Usage:**
> Explore.CLI [command] [options]

**Options:**
>  `--version`       Show version information
>
>  `-?`, `-h`, `--help`  Show help and usage information

**Commands:**
>  `export-spaces`                 Export SwaggerHub Explore spaces to filesystem
>
>  `import-spaces`                 Import SwaggerHub Explore spaces from a file
>
>  `import-postman-collection`     Import a Postman Collection (v2.1) from a file into SwaggerHub Explore


### Prerequisites
You will need the following:
- .NET 7.0 (or above). Follow instructions for [Windows](https://learn.microsoft.com/en-us/dotnet/core/install/windows?tabs=net70), [Linux](https://learn.microsoft.com/en-us/dotnet/core/install/linux), or [MacOS](https://learn.microsoft.com/en-us/dotnet/core/install/macos).
- A SwaggerHub Explore account, register at https://try.smartbear.com/swaggerhub-explore (if required).

### Install the CLI

#### Nuget

Download and install the CLI tool from Nuget: https://www.nuget.org/packages/Explore.Cli

`dotnet tool install --global Explore.Cli`

#### GitHub Releases

Standalone releases of the Explore.CLI tool are published to GitHub Releases.

##### Supported Platforms

| OS      | Architecture | Supported  |
| ------- | ------------ | ---------  |
| OSX     | x86_64       | ✅         |
| Linux   | x86_64       | ✅         |
| Windows | x86_64       | ✅         |
| OSX     | arm64        | ✅         |
| Linux   | arm64        | ✅         |
| Windows | arm64        | ✅         |
| Alpine  | x86_64       | ✅         |
| Alpine  | arm64        | ✅         | 

###### Pre-requisites

- `libicu` - Linux musl and glibc based systems
- `gcc` - Linux musl based systems

###### Alpine

`apk add -y gcc icu`

###### Debian

`apt update && apt install -y libicu`

### Session Cookies for CLI command

You will need to obtain certain cookies from an active session in SwaggerHub Explore to invoke the `CLI` commands.

From SwaggerHub Explore, navigate to your browser development tools, locate the application cookies and extract the `SESSION` and `XSRF-TOKEN` cookies.

#### How to get cookie values from your browser

##### Keyboard
- Windows/Linux: Ctrl + Shift + I or F12
- macOS: ⌘ + ⌥ + I

##### Other Options
**Chrome** 
> Use one of the following methods:
> - click the three-dots icon in the upper-right-hand corner of the browser window `>`  click More tools `>` Developer Tools
> - F12 (on Windows/Linux), and Option + ⌘ + J (on macOS)

**Firefox**
> Use one of the following methods:
> - click the hamburger (three-lines) icon in the upper-right-hand corner of the browser window `>`  click More tools `>` Web Developer Tools
> - F12 (on Windows/Linux), and Option + ⌘ + J (on macOS)

**Edge**
> Use one of the following methods:
> - click the three-dots icon in the upper-right-hand corner of the browser window `>`  click More tools `>` Developer Tools
> - F12 (on Windows/Linux), and Option + ⌘ + J (on macOS)

**Safari**
> Develop `>` Show Web Inspector. If you can't see the Develop menu, go to Safari `>` Preferences `>` Advanced, and check the Show Develop menu in menu bar checkbox.

### Running the `export-spaces` command

**Command Options**
```
  _____                  _                              ____   _   _
 | ____| __  __  _ __   | |   ___    _ __    ___       / ___| | | (_)
 |  _|   \ \/ / | '_ \  | |  / _ \  | '__|  / _ \     | |     | | | |
 | |___   >  <  | |_) | | | | (_) | | |    |  __/  _  | |___  | | | |
 |_____| /_/\_\ | .__/  |_|  \___/  |_|     \___| (_)  \____| |_| |_|
                |_|
```
**Description:**
  > Export SwaggerHub Explore spaces to filesystem

**Usage:**
  > Explore.CLI export-spaces [options]

**Options:**
  > `-ec`, `--explore-cookie` <explore-cookie> (REQUIRED)  A valid and active SwaggerHub Explore session cookie

  > `-fp`, `--file-path` <file-path>                       The path to the directory used for exporting data. It can be either relative or absolute

  > `-en`, `--export-name` <export-name>                   The name of the exported file

  > `-n`, `--names` <names>                                A comma-separated list of space names to export

  > `-v`, `--verbose`                                      Include verbose output during processing

  > `-?`, `-h`, `--help`                                     Show help and usage information

**Note** - the format for SwaggerHub Explore cookies is as follows: `"cookie-name=cookie-value; cookie-name=cookie-value"`

>Example: `"SESSION=5a0a2e2f-97c6-4405-b72a-299fa8ce07c8; XSRF-TOKEN=3310cb20-2ec1-4655-b1e3-4ab76a2ac2c8"`

> Please note - the current `export-spaces` does not support exporting KAFKA APIs

### Running the `import-spaces` command

**Command Options**
```
  _____                  _                              ____   _   _
 | ____| __  __  _ __   | |   ___    _ __    ___       / ___| | | (_)
 |  _|   \ \/ / | '_ \  | |  / _ \  | '__|  / _ \     | |     | | | |
 | |___   >  <  | |_) | | | | (_) | | |    |  __/  _  | |___  | | | |
 |_____| /_/\_\ | .__/  |_|  \___/  |_|     \___| (_)  \____| |_| |_|
                |_|
```
**Description:**
  > Import SwaggerHub Explore spaces from a file

**Usage:**
  > Explore.CLI import-spaces [options]

**Options:**
  > `-ec`, `--explore-cookie` <explore-cookie> (REQUIRED)  A valid and active SwaggerHub Explore session cookie

  > `-fp`, `--file-path` <file-path> (REQUIRED)            The path to the file used for importing data

  > `-n`, `--names` <names>                                A comma-separated list of space names to import

  > `-v`, `--verbose`                                      Include verbose output during processing

  > `-?`, `-h`, `--help`                                     Show help and usage information

**Note** - the format for SwaggerHub Explore cookies is as follows: `"cookie-name=cookie-value; cookie-name=cookie-value"`

>Example: `"SESSION=5a0a2e2f-97c6-4405-b72a-299fa8ce07c8; XSRF-TOKEN=3310cb20-2ec1-4655-b1e3-4ab76a2ac2c8"`

> Please note - the current `import-spaces` does not support importing KAFKA APIs

### Running the `import-postman-collection` command

**Command Options**
```
  _____                  _                              ____   _   _
 | ____| __  __  _ __   | |   ___    _ __    ___       / ___| | | (_)
 |  _|   \ \/ / | '_ \  | |  / _ \  | '__|  / _ \     | |     | | | |
 | |___   >  <  | |_) | | | | (_) | | |    |  __/  _  | |___  | | | |
 |_____| /_/\_\ | .__/  |_|  \___/  |_|     \___| (_)  \____| |_| |_|
                |_|
```
**Description:**
  > Import Postman collections (v2.1) from a file

**Usage:**
  > Explore.CLI import-postman-collection [options]

**Options:**
  > `-ec`, `--explore-cookie` <explore-cookie> (REQUIRED)  A valid and active SwaggerHub Explore session cookie

  > `-fp`, `--file-path` <file-path> (REQUIRED)            The path to the Postman collection

  > `-v`, `--verbose`                                      Include verbose output during processing

  > `-?`, `-h`, `--help`                                     Show help and usage information

**Note** - the format for SwaggerHub Explore cookies is as follows: `"cookie-name=cookie-value; cookie-name=cookie-value"`

>Example: `"SESSION=5a0a2e2f-97c6-4405-b72a-299fa8ce07c8; XSRF-TOKEN=3310cb20-2ec1-4655-b1e3-4ab76a2ac2c8"`

> **Notes:**
> - Compatible with Postman Collections v2.1
> - Nested collections get flattened into a single Explore space
> - GraphQL collections/requests not supported
> - Environments, Authorization data (not including explicit headers), Pre-request Scripts, Tests are not included in import

## More Information on SwaggerHub Explore
      
- For SwaggerHub Explore info, see - https://swagger.io/tools/swaggerhub-explore/
- For SwaggerHub Explore docs, see - https://support.smartbear.com/swaggerhub-explore/docs
- Try SwaggerHub Explore - https://try.smartbear.com/swaggerhub-explore


## Development

### Prerequisites 

You will need the following:
- .NET 7.0 (or above). Follow instructions for [Windows](https://learn.microsoft.com/en-us/dotnet/core/install/windows?tabs=net70), [Linux](https://learn.microsoft.com/en-us/dotnet/core/install/linux), or [MacOS](https://learn.microsoft.com/en-us/dotnet/core/install/macos).

### Setting up

Run the following commands to setup the repository for local development:

```
$ git clone https://github.com/SmartBear-DevRel/explore-cli.git
$ cd explore-cli/src/explore.cli
$ dotnet add package System.CommandLine --prerelease
$ dotnet add package Microsoft.AspNetCore.StaticFiles
$ dotnet add package NJsonSchema
```

### Build

Run the following command to build the project (assumes you are in `src/Explore.CLI`):

```
$ dotnet build
```

### Test

Run the following command to test the project (assumes you are in `src/Explore.CLI`):

```
$ dotnet test ../../test/Explore.Cli.Tests/Explore.Cli.Tests.csproj
```

### Pack the CLI

Run the following command to pack the project (assumes you are in `src/Explore.CLI`):

```
$ dotnet pack
```

### Install and test the package locally

Run the following command to _uninstall_ previous local versions, and install the newly packed version globally on your machine (assumes you are in `src/Explore.CLI`):

```
$ dotnet tool uninstall --global explore.cli
$ dotnet tool install --global --add-source ./nupkg Explore.Cli
```

## Contributing

This projects uses [SmartBear-DevRel](https://github.com/SmartBear-DevRel) GitHub organization's contributing guide. 

You can obtain a copy of this contributing guide at [https://github.com/SmartBear-DevRel/.github/blob/main/profile/CONTRIBUTING.md](https://github.com/SmartBear-DevRel/.github/blob/main/profile/CONTRIBUTING.md).

Read our contributing guide to learn about our development process, how to propose bugfixes and improvements for `Explore.CLI`.
