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
>   Simple utility CLI for importing data into SwaggerHub Explore

**Usage:**
> Explore.CLI [command] [options]

**Options:**
>  `--version`       Show version information
>
>  `-?`, `-h`, `--help`  Show help and usage information

**Commands:**
>  `import-inspector-collections`  Import Swagger Inspector collections into SwaggerHub Explore


## Importing Swagger Inspector Collections to SwaggerHub Explore

The `Explore.cli` can import any collection you might have in Swagger Inspector as spaces within SwaggerHub Explore via the 'import-inspector-collections` command.

A sample call to the command is as follows:
```
explore.cli import-inspector-collections -u JoeBloggs -ic "inspector-token=34c5921e-fdf8-4531-8d7a-ed2940076444" -ec "SESSION=5a0a2e2f-97c6-4405-b72a-299fa8ce07c8; XSRF-TOKEN=3310cb20-2ec1-4655-b1e3-4ab76a2ac2c8"
```

### Prerequisites
You will need the following:
- A SwaggerHub Explore account, register at https://try.smartbear.com/swaggerhub-explore (if required)
- A Swagger Inspector account, register by clicking the `Sign Up` button at https://inspector.swagger.io/builder (if required)

### Install the CLI

Download and install the CLI tool from Nuget: https://www.nuget.org/packages/Explore.Cli

`dotnet tool install --global Explore.Cli`

### Session Cookies for CLI command

You will need to obtain certain cookies from active sessions in both Swagger Inspector and SwaggerHub Explore to invoke the `import-inspector-collections` CLI command.

From Swagger Inspector, navigate to your browser development tools, locate the application cookies and extract the `inspector-token` cookie.

From SwaggerHub Explore, navigate to your browser development tools, locate the application cookies and extract the `session` and `XSRF-TOKEN` cookies.

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

### Running the `import-inspector-collections` command

**Command Options**:
 ``` _____                  _                              ____   _   _
 | ____| __  __  _ __   | |   ___    _ __    ___       / ___| | | (_)
 |  _|   \ \/ / | '_ \  | |  / _ \  | '__|  / _ \     | |     | | | |
 | |___   >  <  | |_) | | | | (_) | | |    |  __/  _  | |___  | | | |
 |_____| /_/\_\ | .__/  |_|  \___/  |_|     \___| (_)  \____| |_| |_|
                |_|
```
**Description:**
  > Import Swagger Inspector collections into SwaggerHub Explore

**Usage:**
  > `Explore.CLI import-inspector-collections [options]`

**Options:**


  > `-u`, `--username` <username> (REQUIRED)                   Username from Swagger Inspector.
  
  > `-ic`, `--inspector-cookie` <inspector-cookie> (REQUIRED)  A valid and active Swagger Inspector session cookie associated with
                                                         provided username
  
  > `-ec`, `--explore-cookie` <explore-cookie> (REQUIRED)      A valid and active SwaggerHub Explore session cookie
  
  > `-?`, `-h`, `--help`                                         Show help and usage information


**Note** - the format for inspector cookie is as follows: `"cookie-name=cookie-value"`

> Example `"inspector-token=34c5921e-fdf8-4531-8d7a-ed2940076444"`

**Note** - the format for SwaggerHub Explore cookies is as follows: `"cookie-name=cookie-value; cookie-name=cookie-value"`

>Example: `"SESSION=5a0a2e2f-97c6-4405-b72a-299fa8ce07c8; XSRF-TOKEN=3310cb20-2ec1-4655-b1e3-4ab76a2ac2c8"`


## More Information
      
- For SwaggerHub Explore info, see - https://swagger.io/tools/swaggerhub-explore/
- For SwaggerHub Explore docs, see - https://support.smartbear.com/swaggerhub-explore/docs
- Try SwaggerHub Explore - https://try.smartbear.com/swaggerhub-explore