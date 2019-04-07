using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace APIClientLibrary
{
    public static class FetchDataFromAPI
    {
        const string clientId = "83872a6b-1fba-44a2-92e5-8bcc0da7d5e3";
        const string authority = "https://login.microsoftonline.com/common/";
        const string resourceId = "b68f72a1-f591-4512-b22b-03718b8eea41";
        const string baseAddress = "https://azurefunctionauth.azurewebsites.net/api/HttpTrigger1";
        static Uri redirectURI = new Uri("https://login.microsoftonline.com/common/oauth2/nativeclient");


        public static async Task<string> GetData(IPlatformParameters platformParameters)
        {
            AuthenticationContext authContext = new AuthenticationContext(authority);
            AuthenticationResult result = null;
            string data = string.Empty;

            try
            {
                result = await authContext.AcquireTokenAsync(resourceId, clientId, redirectURI, platformParameters);

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    HttpResponseMessage response = await httpClient.GetAsync(baseAddress + "?name=mohit");
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsStringAsync().Result;
                    }

                    return "Failed";
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
