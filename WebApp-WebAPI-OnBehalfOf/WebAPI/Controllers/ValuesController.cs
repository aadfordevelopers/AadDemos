using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            var userAccessToken = HttpContext.User.Identities.First().BootstrapContext.ToString();
            var claims = HttpContext.User.Claims;
            string userName = claims.Where(m => m.Type == ClaimTypes.Upn).Any() ? claims.First(m => m.Type == ClaimTypes.Upn).Value : claims.First(m => m.Type == ClaimTypes.Email).Value;
            UserAssertion userAssertion = new UserAssertion(userAccessToken, "urn:ietf:params:oauth:grant-type:jwt-bearer", userName);
            string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;
            AuthenticationContext authContext = new AuthenticationContext(AzureAdOptions.Settings.Authority);
            ClientCredential credential = new ClientCredential(AzureAdOptions.Settings.ClientId, AzureAdOptions.Settings.ClientSecret);
            var result = await authContext.AcquireTokenAsync(AzureAdOptions.Settings.ResourceId, credential, userAssertion);

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
                return $"response from API 2 using on behalf of flow is : {response.Content.ReadAsStringAsync().Result}";
            }

            return null;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
