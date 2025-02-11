using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


string? serviceBusConnectionString = builder.Configuration.GetValue<string>("ServiceBus:ConnectionString");
builder.Services.AddSingleton<ServiceBusClient>(new ServiceBusClient(serviceBusConnectionString));


builder.Services.AddSingleton<ServiceBusProcessor>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<ServiceBusClient>();
    var queueName = builder.Configuration["ServiceBus:QueueName"];
    var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
    return processor;
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
