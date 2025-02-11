
using azureDemo;
//00 ok
// await InteractWithStorageAccount.Main2();


//01 ok
// await InteractWithCosmosDb.Main2();

//02a
// string token = await InteractWithEntraId.Main2();
// await InteractWithEntraId.MainConnect(token);
// await InteractWithEntraId.Main3();

//02 ok
// await InteractWithVault.Main2();

//03 ok // 
//await InteractWithRedis.Main2();


//04 ok
// await InteractWithEventGridPublisher.PublisherDemo();
// function to read

/////
// await InteractWithEventHub.Main2();

//05 ok // 
// await InteractWithServiceBusQueue.WriteToQueue();
await InteractWithServiceBusQueue.ReceiveSalesMessageAsync();

//06 //
//InteractWithLogs.Main2();
/**/
//07

//08
