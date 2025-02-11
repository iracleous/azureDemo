using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace azureDemo;

/*
 
Preparation using CLI (bash shell)

az account list  --output tsv


RESOURCE_GROUP="dath-tech-25"
LOCATION="northeurope"  # Change to your preferred Azure region
STORAGE_ACCOUNT_NAME="youruniquestorageacct25"  # Must be globally unique
CONTAINER_NAME="imagedata"

az group create --name $RESOURCE_GROUP --location $LOCATION
az storage account create \
  --name $STORAGE_ACCOUNT_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku Standard_LRS \
  --kind StorageV2

CONNECTION_STRING=$(az storage account show-connection-string \
  --name $STORAGE_ACCOUNT_NAME \
  --resource-group $RESOURCE_GROUP \
  --query connectionString \
  --output tsv)

echo "Connection String: $CONNECTION_STRING"


az storage container create \
  --name $CONTAINER_NAME \
  --account-name $STORAGE_ACCOUNT_NAME \
  --connection-string "$CONNECTION_STRING"

***
install nugget  Azure.Storage.Blobs
 
 */




public class InteractWithStorageAccount
{
    //secret to be hidden
    private static string connectionString = @"";
    private static string containerName = "imagedata";

    public static async Task Main2()
    {
        string localFilePath = @"c:/data/sample.txt";
        string blobFileName = "uploaded-sample.txt";

        // Create BlobServiceClient
        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        // Ensure the container exists
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        // Upload a file
        await UploadFile(containerClient, localFilePath, blobFileName);

        // List blobs
        await ListBlobs(containerClient);

       // Download the file
   //     await DownloadFile(containerClient, blobFileName, "downloaded-sample.txt");

        // Delete the file
   //     await DeleteBlob(containerClient, blobFileName);
    }

    static async Task UploadFile(BlobContainerClient containerClient, string localFilePath, string blobName)
    {
        BlobClient blobClient = containerClient.GetBlobClient(blobName);
        await using FileStream uploadFileStream = File.OpenRead(localFilePath);
        await blobClient.UploadAsync(uploadFileStream, true);
        Console.WriteLine($"Uploaded {blobName}");
    }

    static async Task ListBlobs(BlobContainerClient containerClient)
    {
        Console.WriteLine("Listing blobs...");
        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            Console.WriteLine($" - {blobItem.Name}");
        }
    }

    static async Task DownloadFile(BlobContainerClient containerClient, string blobName, string downloadPath)
    {
        BlobClient blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DownloadToAsync(downloadPath);
        Console.WriteLine($"Downloaded {blobName} to {downloadPath}");
    }

    static async Task DeleteBlob(BlobContainerClient containerClient, string blobName)
    {
        BlobClient blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
        Console.WriteLine($"Deleted {blobName}");
    }
}
