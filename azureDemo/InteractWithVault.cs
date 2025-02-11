// 02.
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azureDemo;

public class InteractWithVault
{
    /*

    preparation
RESOURCE_GROUP="dath-tech-25"
LOCATION="northeurope"  # Change to your preferred region
VAULT_NAME="myvault2025ab12"  # Must be globally unique

az group create --name $RESOURCE_GROUP --location $LOCATION

az keyvault create \
  --name $VAULT_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku standard

VAULT_URI=$(az keyvault show \
  --name $VAULT_NAME \
  --resource-group $RESOURCE_GROUP \
  --query properties.vaultUri \
  --output tsv)

echo "Vault URI: $VAULT_URI"


VAULT_ID=$(az keyvault show \
  --name $VAULT_NAME \
  --resource-group $RESOURCE_GROUP \
  --query id \
  --output tsv)

echo "Vault Resource ID: $VAULT_ID"

APP_ID="your-application-client-id"

az keyvault set-policy \
  --name $VAULT_NAME \
  --resource-group $RESOURCE_GROUP \
  --spn $APP_ID \
  --secret-permissions get list \
  --key-permissions get list



az account tenant list --query "[].{TenantID:tenantId}" --output table



dotnet add package Azure.Security.KeyVault.Secrets
dotnet add package Azure.Identity


App registrations
-> New Registration
.. Accounts in any organizational directory (Any Microsoft Entra ID tenant - Multitenant) and personal Microsoft accounts (e.g. Skype, Xbox)
-> name  athapp2025ab

Directory (tenant) ID:   xxxx
Application (client) ID: xxxx
secretId                 xxxxx
secretValue   xxxx

Assign Permissions in Key Vault
Go to Azure Key Vault → Access Control (IAM).

Click "Add Role Assignment".

Choose:

Role: "Key Vault Secrets Officer" (for full access)
Assign Access To: "athapp2025ab"

or
az keyvault set-policy --name <your-keyvault-name> --secret-permissions get list set --spn <your-client-id>



    */

    // Replace with your Key Vault URI
    private const string KeyVaultUri = "https://myvault2025ab12.vault.azure.net/";

public static async Task Main2()
{
    try
    {
            string clientId = "54a01f32-0366-4886-9c21-8c6613de772b";
            string tenantId = "b1732512-60e5-48fb-92e8-8d6902ac1349";
            //secret to be hidden
            string clientSecret = "";
            

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            var client = new SecretClient(new Uri(KeyVaultUri), credential);

        

        // Store a secret in Key Vault
       await SetSecretAsync(client, "r1", "MySecretValue");

        // Retrieve a secret from Key Vault
         var secretValue = await GetSecretAsync(client, "r1");
        Console.WriteLine($"Secret Value: {secretValue}");
       //  Delete the secret (optional)
     //   await DeleteSecretAsync(client, "other");     

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}


    public static async Task WriteAndReadFromVaultAsync()
    {
        var client = new SecretClient(new Uri(KeyVaultUri),
            new DefaultAzureCredential());

        await client.SetSecretAsync(new KeyVaultSecret("ff", "xxxxiiii"));

        KeyVaultSecret secret = await client.GetSecretAsync("ff");
        string secretValue = secret.Value;
        Console.WriteLine(secretValue);
    }




    private static async Task SetSecretAsync(SecretClient client, string secretName, string secretValue)
{
    var response = await client.SetSecretAsync(secretName, secretValue);
    Console.WriteLine($"Secret '{response.Value.Name}' set successfully.");
}

private static async Task<string> GetSecretAsync(SecretClient client, string secretName)
{
    KeyVaultSecret secret = await client.GetSecretAsync(secretName);
    Console.WriteLine($"Retrieved secret: {secret.Name}");
    return secret.Value;
}

private static async Task DeleteSecretAsync(SecretClient client, string secretName)
{
    var deleteOperation = await client.StartDeleteSecretAsync(secretName);
    Console.WriteLine($"Deleted secret: {secretName}");

    // Wait for the deletion to complete
    await deleteOperation.WaitForCompletionAsync();

    // Purge the deleted secret if needed (irreversible!)
    // await client.PurgeDeletedSecretAsync(secretName);
}
}
 

