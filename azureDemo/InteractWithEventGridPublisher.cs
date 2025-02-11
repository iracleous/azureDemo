// 04.

using Azure;
using Azure.Messaging.EventGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azureDemo;

public class InteractWithEventGridPublisher
{
    /*
     * 
     Preparation
RESOURCE_GROUP="dath-tech-25"
LOCATION="northeurope"  # Change to your preferred region
TOPIC_NAME="eventgridtopic2025ab"  # Must be unique

az group create --name $RESOURCE_GROUP --location $LOCATION

az eventgrid topic create \
  --name $TOPIC_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION

TOPIC_ENDPOINT=$(az eventgrid topic show \
  --name $TOPIC_NAME \
  --resource-group $RESOURCE_GROUP \
  --query endpoint \
  --output tsv)

echo "Topic Endpoint: $TOPIC_ENDPOINT"

ACCESS_KEY=$(az eventgrid topic key list \
  --name $TOPIC_NAME \
  --resource-group $RESOURCE_GROUP \
  --query key1 \
  --output tsv)

echo "Access Key: $ACCESS_KEY"

     * 
     * 
 
Authenticate using Topic Access Key  
Authenticate using Shared Access Signature
Authenticate using Microsoft Entra ID


o	Add the Azure.Messaging.EventGrid package

     * 
     */

    private const string EventGridTopicEndpoint = @"https://eventgridtopic2025ab.northeurope-1.eventgrid.azure.net/api/events";
    //secret to be hidden
    private const string EventGridAccessKey = "";

    public static async Task PublisherDemo()
    {
        try
        {
            // Create an Event Grid publisher client
            var credential = new AzureKeyCredential(EventGridAccessKey);
            var client = new EventGridPublisherClient(new Uri(EventGridTopicEndpoint), credential);

            // Create a list of events
            var events = new List<EventGridEvent>
            {
                new EventGridEvent(
                    subject: "NewFileUploaded",
                    eventType: "FileUploadEvent",
                    dataVersion: "1.0",
                    data: new { id=45, FileName = "example.txt", FileSize = "15MB" }  
                ),
                new EventGridEvent(
                    subject: "UserRegistered",
                    eventType: "UserRegistrationEvent",
                    dataVersion: "1.0",
                    data: new {  id=47,UserName = "JohnDoe", Email = "john.doe@example.com" }
                )
            };

            // Publish the events to Event Grid
            await client.SendEventsAsync(events);
            Console.WriteLine("Events successfully published to Event Grid.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }






}
