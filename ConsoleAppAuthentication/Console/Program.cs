using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        const string clientId = "d239ecaa-8411-4949-b7e4-fcf39be4f38b";
        const string authority = "https://login.microsoftonline.com/35d622d5-3f93-42b1-8984-3e2606dbe321";
        const string resourceId = "c6b7b3ff-80c6-45f6-aa97-5f70ff8965d9";
        const string baseAddress = "https://localhost:44300";
        private static readonly Uri redirectURI = new Uri("http://console");
        private static AuthenticationContext authContext = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Press Enter to Fetch Data from API.");
            Console.ReadLine();
            Task.WaitAll(GetData());
            Console.ReadLine();
        }

        private static async Task GetData()
        {
            authContext = new AuthenticationContext(authority);
            AuthenticationResult result = null;
            string data = string.Empty;

            try
            {
                result = await authContext.AcquireTokenAsync(resourceId, clientId, redirectURI, new PlatformParameters(PromptBehavior.Auto));
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                HttpResponseMessage response = await httpClient.GetAsync(baseAddress + "/api/values");
                if (response.IsSuccessStatusCode)
                {
                    data = response.Content.ReadAsStringAsync().Result;
                }

                Console.WriteLine($"Response from API is: {data}");
            }
        }
    }
}
