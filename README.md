# Automation Framework - C# | SpecFlow | Reports | Logs

UI Automation testing framework in .NET 6

## Built With

- [Selenium](https://www.selenium.dev/)
- [SpecFlow](https://specflow.org/)
- [NUnit](https://nunit.org/)
- [Serilog](https://serilog.net/)
- [ExtentReports](https://www.extentreports.com/)
- [Oracle](https://www.nuget.org/packages/Oracle.ManagedDataAccess.Core/)
- [SQL](https://www.nuget.org/packages/System.Data.SqlClient)


## appSettings

 Move the global config variables to the **`appSettings.json`** file, they do not depend on the build. They overwrite the default values on

| Parameter | Description                |
| :-------- | :------------------------- |
| **`BrowserType`** | It could be chrome (default), chrome_headless, ie, firefox or edge |
| **`Reporter`** | It enable (true) / disable (false as default) the report generator. |
| **`Environment`** | It could be dev, test or preprod (default) |
| **`LogLevel`** | It could be Verbose/Debug/Information/Warning/Error/Fatal |


## Command line execution
*It could be executed from a command line using dotnet runtime library:*

```bash
dotnet test "<bin folder>\AutomationFW.Test.dll" --filter TestCategory=<tag, e.g.:smoke>

```

## Author

- [@fbenitezrtw](https://github.com/fbenitezrtw) - Fernando Benitez
