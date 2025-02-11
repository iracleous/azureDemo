// 05.

using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azureDemo;

/*
 * 
Preparation using CLI (bash shell)
<myLocation> northeurope

myLocation=northeurope
myNameSpaceName=az204svcbus$RANDOM
myResourceGroupName=az204-svcbus-rg 
myQueueName=az204-queue

az group create --name $myResourceGroupName --location $myLocation
az servicebus namespace create \
    --resource-group $myResourceGroupName \
    --name $myNameSpaceName \
    --location $myLocation
az servicebus queue create --resource-group $myResourceGroupName \
    --namespace-name $myNameSpaceName \
    --name $myQueueName

// Retrieve the connection string for the Service Bus Namespace

az servicebus namespace authorization-rule keys list \
  --resource-group $myResourceGroupName \
  --namespace-name $myNameSpaceName \
  --name "RootManageSharedAccessKey" \
  --query primaryConnectionString \
  --output tsv


##################
az servicebus queue show \
    --resource-group  $myResourceGroupName \
    --name $myQueueName \
    --query messageCount \
    --namespace-name $myNameSpaceName





*
dotnet add package Azure.Messaging.ServiceBus

 * 
 */



public class InteractWithServiceBusQueue
{
    //secret to be hidden
    // connection string to your Service Bus namespace
    private static  string? connectionString = "";

    // name of your Service Bus topic
   private static readonly string queueName = "az204-queue";

    public static async Task WriteToQueue()
    {

        connectionString = ReadFromAppSettings("ServiceBus:ConnectionString");
        if (connectionString == null) 
        {
            Console.WriteLine("empty connection string. Ends");
            return; 
        }

        Console.WriteLine("Sending a message to the Sales Messages queue...");
        await SendSalesMessageAsync();
        Console.WriteLine("Message was sent successfully.");
    }

    static async Task SendSalesMessageAsync()
    {
        await using var client = new ServiceBusClient(connectionString);

        await using ServiceBusSender sender = client.CreateSender(queueName);
        try
        {
            string messageBody = $"$10,000 order for bicycle parts from retailer Adventure Works.";
            var message = new ServiceBusMessage(messageBody);
            Console.WriteLine($"Sending message: {messageBody}");
            await sender.SendMessageAsync(message);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
        }
        finally
        {
            // Calling DisposeAsync on client types is required to ensure that network
            // resources and other unmanaged objects are properly cleaned up.
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }


    public static async Task ReceiveSalesMessageAsync()
    {

        Console.WriteLine("======================================================");
        Console.WriteLine("Press ENTER key to exit after receiving all the messages.");
        Console.WriteLine("======================================================");
        connectionString = ReadFromAppSettings("ServiceBus:ConnectionString");

        var client = new ServiceBusClient(connectionString);

        var processorOptions = new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false
        };

        await using ServiceBusProcessor processor = client.CreateProcessor(queueName, processorOptions);

        processor.ProcessMessageAsync += MessageHandler;
        processor.ProcessErrorAsync += ErrorHandler;


        await processor.StartProcessingAsync();

        Console.Read();

        await processor.CloseAsync();

    }

    // handle received messages
    static async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        Console.WriteLine($"Received: {body}");

        // complete the message. messages is deleted from the queue. 
        await args.CompleteMessageAsync(args.Message);
    }

    // handle any errors when receiving messages
    static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

    public static string? ReadFromAppSettings(string key)
    {

        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory) // Required for appsettings.json in console apps
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string? keyValue = config[key];
        return keyValue;
    }
}

 
 
