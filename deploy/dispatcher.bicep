@description('The name of the service bus namespace')
param serviceBusNamespaceName string

@description('The location to deploy the resources to.')
param location string = resourceGroup().location

@description('The base URI of the Carbon Aware SDK WebAPI you deployed.')
param carbonAwareWebApiClientBaseUri string

//@description('The name of the function app that you wish to create.')
//param appName string = 'fnapp${uniqueString(resourceGroup().id)}'

var appName = 'dispatcher-prod-${uniqueString(resourceGroup().id)}'

// Function App Varibales
var serviceBusQueueName = 'dispatch-queue'
var functionAppName = appName
var hostingPlanName = appName
var applicationInsightsName = appName
var storageAccountName = '${uniqueString(resourceGroup().id)}azfunctions'
var functionAppServerPrivateEndpoint = '${appName}-privateendpoint'

// Virtual Network Variables
var virtualNetworkName = 'VNet-${uniqueString(resourceGroup().id)}'
var subnetName = 'default'
var natGatewayName = '${virtualNetworkName}-natGateway'
var publicIpAddressName = '${natGatewayName}-publicIp'

// SQL Server Variables
var sqlServerName = 'sqlserver${uniqueString(resourceGroup().id)}'
var databaseName = '${sqlServerName}/sample-db'
var sqlServerPrivateEndpoint = '${sqlServerName}-privateendpoint'

// Service Bus Variables
var serviceBusEndpoint = '${serviceBusNamespace.id}/AuthorizationRules/RootManageSharedAccessKey'
var serviceBusConnectionString = listKeys(serviceBusEndpoint, serviceBusNamespace.apiVersion).primaryConnectionString

// ==============================================================================================
//  Service Bus
// ==============================================================================================
resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-01-01-preview' = {
  name: serviceBusNamespaceName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {}
}

resource serviceBusQueue 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBusNamespace
  name: serviceBusQueueName
  properties: {}
}

// ==============================================================================================
//  Function App
// ==============================================================================================
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
}

resource hostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: hostingPlanName
  location: location
  sku: {
    name: 'B1'
    tier: 'Dynamic'
  }
  properties: {}
}

resource functionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: hostingPlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(functionAppName)
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'CarbonAwareWebApiClient__BaseUri'
          value: carbonAwareWebApiClientBaseUri
        }
        {
          name: 'ServiceBusConnectionString'
          value: '${serviceBusConnectionString}'
        }
      ]
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

/*
// ==============================================================================================
//  Virtual Network
// ==============================================================================================
resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-11-01' = {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/24'
      ]
    }
    subnets: [
      {
        name: subnetName
        properties: {
          addressPrefix: '10.0.0.0/24'
          natGateway: {
            id: natGateway.id
          }
          privateEndpointNetworkPolicies: 'Disabled'
        }
      }
    ]
    enableDdosProtection: false
    
  }
}

resource publicIpAddress 'Microsoft.Network/publicIPAddresses@2022-05-01' = {
  name: publicIpAddressName
  sku: {
    name: 'Standard'
  }
  location: location
  properties: {
    publicIPAllocationMethod: 'Static'
  }
}

resource natGateway 'Microsoft.Network/natGateways@2022-05-01' = {
  name: natGatewayName
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIpAddresses: [
      {
        id: publicIpAddress.id
      }
    ]
  }
}

// ==============================================================================================
//  SQL Server
// ==============================================================================================
resource sqlServer 'Microsoft.Sql/servers@2021-11-01-preview' = {
  name: sqlServerName
  location: location
  tags: {
    displayName: sqlServerName
  }
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorLoginPassword
    version: '12.0'
    publicNetworkAccess: 'Disabled'
  }
}

resource database 'Microsoft.Sql/servers/databases@2021-11-01-preview' = {
  name: databaseName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
  }
  tags: {
    displayName: databaseName
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 104857600
    sampleName: 'AdventureWorksLT'
  }
  dependsOn: [
    sqlServer
  ]
}

resource privateEndpoint 'Microsoft.Network/privateEndpoints@2021-05-01' = {
  name: functionAppServerPrivateEndpoint
  location: location
  properties: {
    subnet: {
      id: subnet.id
    }
    privateLinkServiceConnections: [
      {
        name: functionAppServerPrivateEndpoint
        properties: {
          privateLinkServiceId: sqlServer.id
          groupIds: [
            'sqlServer'
          ]
        }
      }
    ]
  }
}
*/

output functionAppName string = functionAppName
output storageAccountName string = storageAccountName
output applicationInsightsName string = applicationInsightsName
output serviceBusNamespaceName string = serviceBusNamespaceName
output serviceBusQueueName string = serviceBusQueueName

/*
output virtualNetworkName string = virtualNetwork.name
output subnetName string = subnetName
output natGatewayName string = natGateway.name
output publicIpAddressName string = publicIpAddress.name
output sqlServerName string = sqlServerName
output databaseName string = databaseName
output privateEndpointName string = functionAppServerPrivateEndpoint
*/
