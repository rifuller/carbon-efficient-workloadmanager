# Carbon-Efficient Workload Manager
A solution and library for shifting workloads geographically and temporally to reduce carbon emitted.

## Prequisites

* .NET 6+ SDK
* Azure CLI (az)
* [Azure Functions Core Tools CLI](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=v4%2Cwindows%2Ccsharp%2Cportal%2Cbash#v2) (func)

## Projects

**CarbonAwareWebApiClient**
**CarbonAwareWebApiClient.Abstractions**

Client for communication with Carbon Aware SDK WebAPI. Translates their responses to our object model.

**Dispatcher**

Azure Function for generating jobs, routing them, and reading and writing to/from the service bus.

**JobRouter.Abstractions**
**JobRouter.CarbonOptimisedRouter**

The routing library and abstractions. Uses the web api CLI to get lowest emissions region.

**Model**

Object model using Entity Framework

**TestDataCLI**

Command-line tool for generating and running synthetic tests.

## CLI Quick Start

1. Deploy the Carbon Aware SDK WebAPI to an accessible location (locally or cloud).
2. Go to `\src\TestDataCLI`
3. Rename `appsettings.Development.json.sample` to `appsettings.Development.json`
4. Open it and change `BaseUri` to the app you deployed.

**To generate a test data set:**
```pwsh
dotnet run create  -out "data.csv" -count 1000  
```
evaluate
-in testdata.simple10000.csv
-out testdata.simple10000.eval.csv
Jobs will be randomly spread across a week-long window in the past. This was because watttime was not returning data reliably closer to the present.

**To run the jobs through the router and determine the emissions savings:**
```pwsh
dotnet run evaluate -in "data.csv" -out "data.out.csv"
```

Evaluation will use a batch size of 10.

## To run the dispatcher locally

1. Deploy the Carbon Aware SDK WebAPI to an accessible location (locally or cloud).
2. Create a storage account for Azure Functions host to use.
3. Create a service bus namespace. Note the connection string. (if you want, you can deploy a prod instance using the bicep files and reuse that)
4. Create a service bus queue called "dispatch-queue" 
5. Go to `\src\Dispatcher`
6. Create a `local.settings.json` file with the contents and fill in the connection strings:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "DefaultEndpointsProtocol=https;AccountName=.....",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ServiceBusConnectionString": "Endpoint=sb://...."
  }
}
```

7. Run `func start`
8. Open `http://localhost:7071/api/hello` in your browser and it will print the timstamp.
9. Open `http://localhost:7071/api/ProduceOneJob` in your browser and it will print the JobID and pass it to the router. See the log for the details.

## To Deploy to Azure
Deploying to Azure uses Bicep which creates and configures resources for you. You will then need to use Azure Functions CLI to push the code to the App Service.

1. Set parameters in `deploy\params.prod.json`

2. Create the Azure resources:
From the `\deploy` folder:
```pwsh
az login

# Create a resource group
$rg = 'gsf-project-example'
az group create -l australiaeast -n $rg

# Deploy the project
az deployment group create --resource-group $rg --template-file dispatcher.bicep --parameters params.prod.json
```

Note the function app name that was generated for you.

3. Deploy the project

From the `\src\dispatcher` folder:
```pwsh
func azure functionapp publish <FunctionAppName>
```

4. Relax

## Database Migrations
Entity Framework is used. See the EF documentation for initialising or migrating a database if required.

## Solution Architecture

Below are some diagrams that were produced early on and are out of date but should still give you a clear idea of how this can be used.

![Solution Diagram](https://github.com/rifuller/carbon-efficient-workloadmanager/blob/main/docs/Solution%20Architecture.png?raw=true)

![Database Schema](https://github.com/rifuller/carbon-efficient-workloadmanager/blob/main/docs/Database.png?raw=true)


## FAQ

> System.Private.CoreLib: Exception while executing function: Functions.RouteJob. System.Private.CoreLib: Result: Failure
> Exception: System.AggregateException: One or more errors occurred. (The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, when isFinalBlock is true

The Carbon-aware SDK WebAPI sometimes a 204 No Content which bubbles up. This can be handled better with additional time.
