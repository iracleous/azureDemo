using Microsoft.Identity.Client;
using System.Net.Http.Headers;
namespace azureDemo;

/*
//1. Install Required NuGet Packages
dotnet add package Microsoft.Identity.Client
dotnet add package Microsoft.Identity.Web
dotnet add package Microsoft.Graph

//2. Register an App in Entra ID 

App registrations
-> New Registration
.. Accounts in any organizational directory (Any Microsoft Entra ID tenant - Multitenant) and personal Microsoft accounts (e.g. Skype, Xbox)
-> name  athapp2025ab

Directory (tenant) ID:   xxxx
Application (client) ID: xxxx
secretId                 xxxxx
secretValue   xxxx

Redirect URI (for web apps):
https://localhost:5001/signin-oidc

*/
public class InteractWithEntraId
{
   private static readonly string tenantId = "b1732512-60e5-48fb-92e8-8d6902ac1349";
   private static readonly string clientId = "54a01f32-0366-4886-9c21-8c6613de772b";
    
    //secret to be hidden
 private static  readonly  string clientSecret = "";
  private static readonly  string authority = $"https://login.microsoftonline.com/{tenantId}";
  private static readonly  string scope = "https://graph.microsoft.com/.default";

    public static async Task<string> Main2()
    {
        var app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri(authority))
                .Build();

        var result = await app.AcquireTokenForClient([scope]).ExecuteAsync();
        string accessToken = result.AccessToken;

        Console.WriteLine($"Access Token: {accessToken}");
        return accessToken;
    }

    public static async Task MainConnect(string  accessToken)
    {

        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await client.GetAsync("https://graph.microsoft.com/v1.0/users");
        string responseBody = await response.Content.ReadAsStringAsync();

        Console.WriteLine(responseBody);
    }


    //Use Delegated Permissions
    public static async Task Main3()
    {
          string redirectUri = "http://localhost"; // Must match app registration

        var app = PublicClientApplicationBuilder.Create(clientId)
            .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
            .WithRedirectUri(redirectUri)
            .Build();

        string[] scopes = { "User.Read" }; // Delegated permissions

        var result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
        Console.WriteLine($"Access Token: {result.AccessToken}");
    }



}
