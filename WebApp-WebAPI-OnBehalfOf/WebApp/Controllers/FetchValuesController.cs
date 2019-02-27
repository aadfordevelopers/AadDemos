using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    [Authorize]
    public class FetchValuesController : Controller
    {
        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            AuthenticationResult result = null;
            try
            {
                string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;
                AuthenticationContext authContext = new AuthenticationContext(AzureAdOptions.Settings.Authority, new TokenSessionCache(userObjectID, HttpContext.Session));
                ClientCredential credential = new ClientCredential(AzureAdOptions.Settings.ClientId, AzureAdOptions.Settings.ClientSecret);
                result = await authContext.AcquireTokenSilentAsync(AzureAdOptions.Settings.ResourceId, credential, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));

                // Retrieve data from API.
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
                HttpClient client = new HttpClient(handler);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, AzureAdOptions.Settings.APIBaseAddress + "/api/values");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Response = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Request.Query["reauth"] == "True")
                {
                    // If Reauth is required challenge for login again
                    return new ChallengeResult(OpenIdConnectDefaults.AuthenticationScheme);
                }
            }

            return View();
        }
    }
}
