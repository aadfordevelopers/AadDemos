using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static string aadInstance = "https://login.microsoftonline.com/{0}";
        private static string tenant = "35d622d5-3f93-42b1-8984-3e2606dbe321";
        private static string clientId = "3ff68a77-2a93-4998-a91e-5c81e7e96893";
        private static string clientSecret = "dJvpuqHuSkHTnnG2CVszhmY00j/D7gkp49WaRwnf5pw=";
        static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
        private static string resourceId = "c6b7b3ff-80c6-45f6-aa97-5f70ff8965d9";
        private static string apiBaseAddress = "https://localhost:44300";
        private static AuthenticationContext authContext = null;
        private static ClientCredential certCred = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Press Enter to fetch respone from API");
            Console.ReadLine();
            authContext = new AuthenticationContext(authority);
            certCred = new ClientCredential(clientId, clientSecret);
            var result = authContext.AcquireTokenAsync(resourceId, certCred).Result;
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiBaseAddress + "/api/values");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            HttpResponseMessage response = client.SendAsync(request).Result;

            string responseResult = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                responseResult = response.Content.ReadAsStringAsync().Result;
            }

            Console.WriteLine(responseResult);
            Console.ReadLine();
        }
    }
}
