using AzServiceBusProducer.Models;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace AzServiceBusProducer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SbusProducerController : ControllerBase
    {
        private readonly ServiceBusClient _client;
        private readonly string _queueName;
        private readonly ServiceBusProcessor _processor;

        public SbusProducerController(ServiceBusClient client, ServiceBusProcessor processor)
        {
            _client = client;
            _queueName = "az204-queue";
            _processor = processor;
        }

        [HttpPost]
        public async Task PostOrder(Order order)
        {
            await using ServiceBusSender sender = _client.CreateSender(_queueName);
            var message = new ServiceBusMessage(JsonConvert.SerializeObject(order));
            await sender.SendMessageAsync(message);
        }



        [HttpPost("consume")]
        public async Task<IActionResult> StartConsuming()
        {
            _processor.ProcessMessageAsync += ProcessMessageHandler;
            _processor.ProcessErrorAsync += ProcessErrorHandler;

            // Start processing messages
            await _processor.StartProcessingAsync();

            return Ok("Service Bus message consumption started.");
        }



       // This handler processes incoming messages
        private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received message: {body}");

            // Here, you can perform any processing or actions you need
            // For example, store the message to a database, trigger events, etc.

            // Complete the message to remove it from the queue
            await args.CompleteMessageAsync(args.Message);
        }

        // This handler handles errors during message processing
        private Task ProcessErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error occurred: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        // Endpoint to stop consuming messages
        [HttpPost("stop")]
        public async Task<IActionResult> StopConsuming()
        {
            await _processor.StopProcessingAsync();
            return Ok("Service Bus message consumption stopped.");
        }
    }
}
 

 