# explore-cli

Simple utility CLI for importing data into API Hub Explore.

![Can I Deploy](https://smartbear.pactflow.io/pacticipants/explore-cli/branches/main/latest-version/can-i-deploy/to-environment/production/badge)

```
  _____                  _                              ____   _   _
 | ____| __  __  _ __   | |   ___    _ __    ___       / ___| | | (_)
 |  _|   \ \/ / | '_ \  | |  / _ \  | '__|  / _ \     | |     | | | |
 | |___   >  <  | |_) | | | | (_) | | |    |  __/  _  | |___  | | | |
 |_____| /_/\_\ | .__/  |_|  \___/  |_|     \___| (_)  \____| |_| |_|
                |_|
```
**Description:**
>   Simple utility CLI for importing data into and out of API Hub Explore

**Usage:**
> Explore.CLI [command] [options]

**Options:**
>  `--version`       Show version information
>
>  `-?`, `-h`, `--help`  Show help and usage information

**Commands:**
>  `export-spaces`                 Export API Hub Explore spaces to filesystem
>
>  `import-spaces`                 Import API Hub Explore spaces from a file
>
>  `import-postman-collection`     Import Postman Collection (v2.1) from a file into API Hub Explore
>
>  `import-insomnia-collection`     Import Insomnia Collection (v4) from a file into API Hub Explore
>
>  `import-pact-file`               Import a Pact file (v2/v3/v4) into API Hub Explore (HTTP interactions only)  

### Prerequisites

You will need the following:

- .NET 7.0 (or above). Follow instructions for [Windows](https://learn.microsoft.com/en-us/dotnet/core/install/windows?tabs=net70), [Linux](https://learn.microsoft.com/en-us/dotnet/core/install/linux), or [MacOS](https://learn.microsoft.com/en-us/dotnet/core/install/macos).
- A API Hub Explore account, register at https://try.smartbear.com/swaggerhub-explore (if required).

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

`apt update && apt install -y libicu-dev`

#### Docker

Dockerfiles are provided for amd64 & arm64 flavours

- Alpine
  - `./Dockerfile.alpine`
- Debian
  - `./Dockerfile.debian`

##### Building images

You can build the Docker images with a specific Explore.CLI version by passing the `VERSION` build argument. If the `VERSION` argument is not provided, the default value specified in the Dockerfile will be used.

```sh
docker build . --platform=linux/arm64 -f Dockerfile.debian  -t explore-cli:debian-arm64 --build-arg VERSION=<VERSION>
docker build . --platform=linux/arm64 -f Dockerfile.debian  -t explore-cli:debian-amd64 --build-arg VERSION=<VERSION>
docker build . --platform=linux/arm64 -f Dockerfile.alpine  -t explore-cli:alpine-arm64 --build-arg VERSION=<VERSION>
docker build . --platform=linux/arm64 -f Dockerfile.alpine  -t explore-cli:alpine-amd64 --build-arg VERSION=<VERSION>
```

##### Using images

The `entrypoint` is the `explore-cli` application.

Set environment variables in your shell

```sh
export EXPLORE_SESSION_TOKEN=<SESSION_TOKEN>
export EXPLORE_XSRF_TOKEN=<XSRF-TOKEN>
```

Run your created docker image, with your required explore-cli command.

In our example we are using `import-spaces` which we have in our local directory under the `spaces` folder.

The `spaces` folder is volume mounted into our container in `/spaces`, and commands to file paths should 
reference this folder.

```sh
docker run --platform=linux/amd64 \
  --rm \
  -it \
  -v $PWD/spaces:/spaces \
  explore-cli:debian \
  import-spaces \
  --explore-cookie "SESSION=${EXPLORE_SESSION_TOKEN}; XSRF-TOKEN=${EXPLORE_XSRF_TOKEN}" \
  -fp /spaces/explore_demo_spaces.json
```

### Session Cookies for CLI command

You will need to obtain certain cookies from an active session in API Hub Explore to invoke the `CLI` commands.

From API Hub Explore, navigate to your browser development tools, locate the application cookies and extract the `SESSION` and `XSRF-TOKEN` cookies.

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
  > Export API Hub Explore spaces to filesystem

**Usage:**
  > Explore.CLI export-spaces [options]

**Options:**
  > `-ec`, `--explore-cookie` <explore-cookie> (REQUIRED)  A valid and active API Hub Explore session cookie

  > `-fp`, `--file-path` <file-path>                       The path to the directory used for exporting data. It can be either relative or absolute

  > `-en`, `--export-name` <export-name>                   The name of the exported file

  > `-n`, `--names` <names>                                A comma-separated list of space names to export

  > `-v`, `--verbose`                                      Include verbose output during processing

  > `-?`, `-h`, `--help`                                     Show help and usage information

**Note** - the format for API Hub Explore cookies is as follows: `"cookie-name=cookie-value; cookie-name=cookie-value"`

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
  > Import API Hub Explore spaces from a file

**Usage:**
  > Explore.CLI import-spaces [options]

**Options:**
  > `-ec`, `--explore-cookie` <explore-cookie> (REQUIRED)  A valid and active API Hub Explore session cookie

  > `-fp`, `--file-path` <file-path> (REQUIRED)            The path to the file used for importing data

  > `-n`, `--names` <names>                                A comma-separated list of space names to import

  > `-v`, `--verbose`                                      Include verbose output during processing

  > `-?`, `-h`, `--help`                                     Show help and usage information

**Note** - the format for API Hub Explore cookies is as follows: `"cookie-name=cookie-value; cookie-name=cookie-value"`

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
  > `-ec`, `--explore-cookie` <explore-cookie> (REQUIRED)  A valid and active API Hub Explore session cookie

  > `-fp`, `--file-path` <file-path> (REQUIRED)            The path to the Postman collection

  > `-v`, `--verbose`                                      Include verbose output during processing

  > `-?`, `-h`, `--help`                                     Show help and usage information

**Note** - the format for API Hub Explore cookies is as follows: `"cookie-name=cookie-value; cookie-name=cookie-value"`

>Example: `"SESSION=5a0a2e2f-97c6-4405-b72a-299fa8ce07c8; XSRF-TOKEN=3310cb20-2ec1-4655-b1e3-4ab76a2ac2c8"`

> **Notes:**

> - Compatible with Postman Collections v2.1
> - Root level request get bundled into API folder with same name as collection
> - Nested collections get added to an API folder with naming format (`parent folder - nested folder`)
> - GraphQL collections/requests not supported
> - Environments, Authorization data (not including explicit headers), Pre-request Scripts, Tests are not included in import

### Running the `import-insomnia-collection` command

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
  > Import Insomnia collections (v4) from a file

**Usage:**
  > Explore.CLI import-insomnia-collection [options]

**Options:**
  > `-ec`, `--explore-cookie` <explore-cookie> (REQUIRED)  A valid and active API Hub Explore session cookie

  > `-fp`, `--file-path` <file-path> (REQUIRED)            The path to the Insomnia collection

  > `-v`, `--verbose`                                      Include verbose output during processing

  > `-?`, `-h`, `--help`                                   Show help and usage information

**Note** - the format for API Hub Explore cookies is as follows: `"cookie-name=cookie-value; cookie-name=cookie-value"`

>Example: `"SESSION=5a0a2e2f-97c6-4405-b72a-299fa8ce07c8; XSRF-TOKEN=3310cb20-2ec1-4655-b1e3-4ab76a2ac2c8"`

> **Notes:**

> - Compatible with Insomnia Collection Exports v4
> - GraphQL collections/requests not supported
> - gRPC collections/requests are not supported
> - Environments variables are inlined and set within the Explore Space
> - Authorization - only Basic and Bearer Token variants are supported

### Running the `import-pact-file` command

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
  > Import a Pact file (v2/v3/v4) into API Hub Explore (HTTP interactions only)

**Usage:**
  > Explore.CLI import-pact-file [options]

**Options:**
  > `-ec`, `--explore-cookie` <explore-cookie> (REQUIRED)  A valid and active API Hub Explore session cookie

  > `-fp`, `--file-path` <file-path> (REQUIRED)            The path to the Insomnia collection

  > `-b`, `--base-uri` <base-uri>                         The base url to use for all imported files

  > `-v`, `--verbose`                                      Include verbose output during processing

  > `-?`, `-h`, `--help`                                   Show help and usage information

**Note** - the format for API Hub Explore cookies is as follows: `"cookie-name=cookie-value; cookie-name=cookie-value"`

>Example: `"SESSION=5a0a2e2f-97c6-4405-b72a-299fa8ce07c8; XSRF-TOKEN=3310cb20-2ec1-4655-b1e3-4ab76a2ac2c8"`

> **Notes:**

> - Compatible with valid Pact v2 / v3 / v4 specification files
> - Users are advised to provide the base url when importing pact files with `--base-uri` / `-b`, to the required server you wish to explore.
> Pact files do not contain this information
> - Currently only supports HTTP interactions.
>   - V3 message based pacts are unsupported
>   - V4 interactions other than synchronous/http will be ignored

## More Information on API Hub Explore

- For API Hub Explore info, see - https://swagger.io/api-hub/explore/
- For API Hub Explore docs, see - https://support.smartbear.com/api-hub/explore/docs/?lang=en
- Try API Hub Explore - https://try.smartbear.com/swaggerhub-explore

## Development

### Prerequisites

You will need the following:

- .NET 7.0 (or above). Follow instructions for [Windows](https://learn.microsoft.com/en-us/dotnet/core/install/windows?tabs=net70), [Linux](https://learn.microsoft.com/en-us/dotnet/core/install/linux), or [MacOS](https://learn.microsoft.com/en-us/dotnet/core/install/macos).

### Setting up

Run the following commands to setup the repository for local development:

```text

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
