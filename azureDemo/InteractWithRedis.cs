// 03.
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azureDemo;

public class InteractWithRedis
{
    /*
    //nuget packages StackExchange.Redis
    preparation for  Azure Cache for Redis instance 

RESOURCE_GROUP="dath-tech-25"
LOCATION="northeurope"  # Change to your preferred region
REDIS_NAME="athredis2025ab"  # Must be globally unique
SKU="Basic"  # Options: Basic, Standard, Premium
SIZE="C1"  # Options: C0, C1, C2, C3, etc.

az group create --name $RESOURCE_GROUP --location $LOCATION

az redis create \
  --name $REDIS_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku $SKU \
  --vm-size $SIZE

REDIS_HOSTNAME=$(az redis show \
  --name $REDIS_NAME \
  --resource-group $RESOURCE_GROUP \
  --query hostName \
  --output tsv)

echo "Redis Hostname: $REDIS_HOSTNAME"

REDIS_ACCESS_KEY=$(az redis list-keys \
  --name $REDIS_NAME \
  --resource-group $RESOURCE_GROUP \
  --query primaryKey \
  --output tsv)

echo "Redis Access Key: $REDIS_ACCESS_KEY"

 

REDIS_HOST=$(az redis show --name $REDIS_NAME --resource-group $RESOURCE_GROUP --query "hostName" --output tsv)
REDIS_KEY=$(az redis list-keys --name $REDIS_NAME --resource-group $RESOURCE_GROUP --query "primaryKey" --output tsv)

echo "Redis Connection String: $REDIS_HOST:6380,password=$REDIS_KEY,ssl=True"

    */
    //secret to be hidden
    private const string RedisConnectionString = "";

    public static async Task Main2()
    {
        try
        {
            // Create a connection to Redis
            using (var connection = await ConnectionMultiplexer.ConnectAsync(RedisConnectionString))
            {
                Console.WriteLine("Connected to Azure Redis Cache!");

                // Get a database instance
                IDatabase database = connection.GetDatabase(1);

                // Set a value in the cache
                await SetCacheValueAsync(database, "MyKey", "Hello, Azure Redis Cache!");

                // Get a value from the cache
                var value = await GetCacheValueAsync(database, "MyKey");
                Console.WriteLine($"Retrieved value from cache: {value}");

                // Delete the key from the cache
                await DeleteCacheKeyAsync(database, "MyKey");
                Console.WriteLine("Deleted key from cache.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static async Task SetCacheValueAsync(IDatabase database, string key, string value)
    {
        await database.StringSetAsync(key, value);
        Console.WriteLine($"Key '{key}' set with value: {value}");
    }

    private static async Task<string> GetCacheValueAsync(IDatabase database, string key)
    {
        var value = await database.StringGetAsync(key);
        return value.HasValue ? value.ToString() : "Key not found.";
    }

    private static async Task DeleteCacheKeyAsync(IDatabase database, string key)
    {
        await database.KeyDeleteAsync(key);
        Console.WriteLine($"Key '{key}' deleted.");
    }

//public static async Task InteractWithRedis()
//    {
//        var person = new { Name = "John", Age = 30 };
//        var jsonData = JsonSerializer.Serialize(person);
//        await database.StringSetAsync("PersonKey", jsonData);
//        await database.StringSetAsync("MyKey", "Temporary Value", TimeSpan.FromMinutes(10));

//        var subscriber = connection.GetSubscriber();
//        await subscriber.SubscribeAsync("my-channel", (channel, message) =>
//        {
//            Console.WriteLine($"Message received: {message}");
//        });

//        await subscriber.PublishAsync("my-channel", "Hello, subscribers!");

//    }

}
