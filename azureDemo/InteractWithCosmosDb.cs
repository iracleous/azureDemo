// 01.
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azureDemo;


/*
 * 
 * subscription -> resource providers -> Microsoft.DocumentDB -> Register

 az provider register --namespace Microsoft.DocumentDB
 
 * 
Preparation

RESOURCE_GROUP="dath-tech-25"
LOCATION="northeurope"  # Change to your preferred region
COSMOS_DB_NAME="eshopdatabase"  # Must be globally unique, lower case

az group create --name $RESOURCE_GROUP --location $LOCATION
az cosmosdb create \
  --name $COSMOS_DB_NAME \
  --resource-group $RESOURCE_GROUP \
  --locations regionName=$LOCATION failoverPriority=0

ENDPOINT_URI=$(az cosmosdb show \
  --name $COSMOS_DB_NAME \
  --resource-group $RESOURCE_GROUP \
  --query documentEndpoint \
  --output tsv)

echo "Endpoint URI: $ENDPOINT_URI"

PRIMARY_KEY=$(az cosmosdb keys list \
  --name $COSMOS_DB_NAME \
  --resource-group $RESOURCE_GROUP \
  --type keys \
  --query primaryMasterKey \
  --output tsv)

echo "Primary Key: $PRIMARY_KEY"



dotnet add package Microsoft.Azure.Cosmos
 * 
 */

public class InteractWithCosmosDb
{
    private const string EndpointUri = @"https://eshopdatabase.documents.azure.com:443/";
    //secret to be hidden
    private const string PrimaryKey = "";
    private const string DatabaseId = "eshopdatabase";
    private const string ContainerId = "ItemsContainer";

    private static CosmosClient? cosmosClient;
    private static Database? database;
    private static Container? container;

    public static async Task Main2()
    {
        try
        {
            // Initialize Cosmos Client
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            Console.WriteLine("Connecting to Cosmos DB...");

            // Create database and container if not exists
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
            Console.WriteLine($"Created or connected to database: {DatabaseId}");

            container = await database.CreateContainerIfNotExistsAsync(ContainerId, "/Category");
            Console.WriteLine($"Created or connected to container: {ContainerId}");

            var item1 = new Item
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Sample Item",
                Description = "This is a sample item",
                Category = "Example",
                Price = 19.99
            };

            // Add sample data
            await AddItemAsync(item1);

            // Query data
            await QueryItemsAsync();

            

            // Update data
            await UpdateItemAsync(item1.Id, item1.Category );

            // Delete data
        //    await DeleteItemAsync(item1.Id, item1.Category);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            cosmosClient?.Dispose();
        }
    }

 
    private static async Task AddItemAsync(Item item)
    {
        if (container == null) { return; }
        await container.CreateItemAsync(item, new PartitionKey(item.Category));
        Console.WriteLine($"Added item: {item.Name}");
    }

    private static async Task QueryItemsAsync()
    {
        if (container == null) { return; }
        var sqlQueryText = "SELECT * FROM c WHERE c.Category = 'Example'";
        var queryDefinition = new QueryDefinition(sqlQueryText);
        var queryResultSetIterator = container.GetItemQueryIterator<Item>(queryDefinition);

        Console.WriteLine("Querying items...");

        while (queryResultSetIterator.HasMoreResults)
        {
            var response = await queryResultSetIterator.ReadNextAsync();
            foreach (var item in response)
            {
                Console.WriteLine($"Item found: {item.Name}, Price: {item.Price}");
            }
        }
    }

    private static async Task UpdateItemAsync(string id, string partitionKey)
    {
        if (container == null) { return; }
        var response = await container.ReadItemAsync<Item>(id, new PartitionKey(partitionKey));
        var item = response.Resource;

        item.Price += 10; // Example update
        await container.UpsertItemAsync(item, new PartitionKey(partitionKey));

        Console.WriteLine($"Updated item: {item.Name}, New Price: {item.Price}");
    }

    private static async Task DeleteItemAsync(string id, string partitionKey)
    {
        if (container == null) { return; }
        await container.DeleteItemAsync<Item>(id, new PartitionKey(partitionKey));
        Console.WriteLine($"Deleted item with id: {id}");
    }
}

public class Item
{
    [JsonProperty("id")]
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public double? Price { get; set; }
}
