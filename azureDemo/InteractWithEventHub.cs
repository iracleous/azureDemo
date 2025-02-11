using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azureDemo;

public class InteractWithEventHub
{
    /*
     * 
     * 
    
Event Grid vs Event Hub


    //preparation
RESOURCE_GROUP="dath-tech-25"
LOCATION="northeurope"  # Change to your preferred region
EVENTHUB_NAMESPACE="myEventHubNamespace2025ab"
EVENTHUB_NAME="myEventHub2025ab"

az group create --name $RESOURCE_GROUP --location $LOCATION

az eventhubs namespace create --name $EVENTHUB_NAMESPACE --resource-group $RESOURCE_GROUP --location $LOCATION --sku Standard

az eventhubs eventhub create --name $EVENTHUB_NAME --namespace-name $EVENTHUB_NAMESPACE --resource-group $RESOURCE_GROUP --message-retention 1 --partition-count 2

az eventhubs eventhub consumer-group create --resource-group $RESOURCE_GROUP --namespace-name $EVENTHUB_NAMESPACE --eventhub-name $EVENTHUB_NAME --name myConsumerGroup

az eventhubs namespace authorization-rule keys list --resource-group $RESOURCE_GROUP --namespace-name $EVENTHUB_NAMESPACE --name RootManageSharedAccessKey --query primaryConnectionString --output tsv
   
     
nugget
    Azure.Messaging.EventHubs
     
     */

    public static async Task Main2()
    {
        //Inspect Event Hubs
        var connectionString = "";
        var eventHubName = "myEventHub2025ab";

        await using (var producer = new EventHubProducerClient(connectionString, eventHubName))
        {
            string[] partitionIds = await producer.GetPartitionIdsAsync();
        }

      /*
        //Publish events to Event Hubs
      
        await using (var producer = new EventHubProducerClient(connectionString, eventHubName))
        {
            using EventDataBatch eventBatch = await producer.CreateBatchAsync();
            eventBatch.TryAdd(new EventData(new BinaryData("First Another")));
            eventBatch.TryAdd(new EventData(new BinaryData("Second Another")));

            await producer.SendAsync(eventBatch);
        }

  */
        //Read events from an Event Hubs
        string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

        await using (var consumer = new EventHubConsumerClient(consumerGroup, connectionString, eventHubName))
        {
            using var cancellationSource = new CancellationTokenSource();
            cancellationSource.CancelAfter(TimeSpan.FromSeconds(45));

            await foreach (PartitionEvent receivedEvent in consumer.ReadEventsAsync(cancellationSource.Token))
            {
                // At this point, the loop will wait for events to be available in the partition. When an event
                // is available, the loop will iterate with the event that was received. Because we did not
                // specify a maximum wait time, the loop will wait forever unless cancellation is requested using
                // the cancellation token.
                Console.WriteLine(receivedEvent.Data.SequenceNumber);
            }
        }
/*

        //Read events from an Event Hubs partition

        await using (var consumer = new EventHubConsumerClient(consumerGroup, connectionString, eventHubName))
        {
            EventPosition startingPosition = EventPosition.Earliest;
            string partitionId = (await consumer.GetPartitionIdsAsync()).First();

            using var cancellationSource = new CancellationTokenSource();
            cancellationSource.CancelAfter(TimeSpan.FromSeconds(45));

            await foreach (PartitionEvent receivedEvent in consumer.ReadEventsFromPartitionAsync(partitionId, startingPosition, cancellationSource.Token))
            {
                // At this point, the loop will wait for events to be available in the partition. When an event
                // is available, the loop will iterate with the event that was received. Because we did not
                // specify a maximum wait time, the loop will wait forever unless cancellation is requested using
                // the cancellation token.
                Console.WriteLine(receivedEvent);
            }
        }
        */

    }
}