// 06.

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azureDemo;

/*
 
RESOURCE_GROUP="dath-tech-25"
LOCATION="northeurope"  # Change to your preferred region
APP_INSIGHTS_NAME="diminsights2025"

az group create --name $RESOURCE_GROUP --location $LOCATION

az monitor app-insights component create \
  --app $APP_INSIGHTS_NAME \
  --location $LOCATION \
  --resource-group $RESOURCE_GROUP \
  --application-type web

**for older SDKs

INSTRUMENTATION_KEY=$(az monitor app-insights component show \
  --app $APP_INSIGHTS_NAME \
  --resource-group $RESOURCE_GROUP \
  --query instrumentationKey \
  --output tsv)

echo "Instrumentation Key: $INSTRUMENTATION_KEY"


**for newer SDKs
CONNECTION_STRING=$(az monitor app-insights component show \
  --app $APP_INSIGHTS_NAME \
  --resource-group $RESOURCE_GROUP \
  --query connectionString \
  --output tsv)

echo "Connection String: $CONNECTION_STRING"



 * 
Step 1: Add NuGet Packages
Install the necessary NuGet packages for Application Insights:
dotnet add package Microsoft.Extensions.Logging.ApplicationInsights
dotnet add package Microsoft.ApplicationInsights.AspNetCore

Step 2: Configure Application Insights in appsettings.json
Add the Application Insights instrumentation key (replace Your_Instrumentation_Key with the actual key from Azure):

{
  "ApplicationInsights": {
    "InstrumentationKey": "Your_Instrumentation_Key"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    },
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information"
      }
    }
  }
}

Step 3: Register Application Insights in Program.cs

Step 4: Deploy to Azure
*/
public class InteractWithLogs
{
    private const string InstrumentationKey = "";
    private const string ConnectionString = "";
    public static void Main2()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory) // Required for appsettings.json in console apps
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
         
        string? instrumentationKey = config["ApplicationInsights:InstrumentationKey"];
        string? logLevelString = config["Logging:LogLevel:Default"];

        // Print values
        Console.WriteLine($"Instrumentation Key: {instrumentationKey}");
        Console.WriteLine($"Default Log Level: {logLevelString}");
    }
}
