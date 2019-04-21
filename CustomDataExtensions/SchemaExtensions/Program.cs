using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SchemaExtensions
{
    class Program
    {
        private static string aadInstance = "https://login.microsoftonline.com/{0}";
        private static string tenant = "35d622d5-3f93-42b1-8984-3e2606dbe321";
        private static string clientId = "0ec6919e-9300-4428-a38b-ad18b4058ce8";
        private static string redirectUrl = "http://GraphClient";
        static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
        private static string resourceId = "https://graph.microsoft.com";
        private static string graphAPIUrl = "https://graph.microsoft.com";
        private static string schemaName = "extgufop49x_vehicle";

        static void Main(string[] args)
        {
            string token = GetAccessTokenForGraphAPI(clientId, redirectUrl, resourceId, authority);
            //CreateSchema("vehicle", token, clientId);
            //AddUpdateSchemaExtensionValue(schemaName, token, "Car", "Red", "Petrol", "d74a1140-119f-4106-a591-55242f7cdacb");
            //ReadSchemaExtension("d74a1140-119f-4106-a591-55242f7cdacb", schemaName, token);
            //RemoveSchemaExtensionValue(schemaName, token, "d74a1140-119f-4106-a591-55242f7cdacb");
        }

        public static string GetAccessTokenForGraphAPI(string clientId, string redirectUrl, string resourceId, string authority)
        {
            var autheContext = new AuthenticationContext(authority);
            var authResult = autheContext.AcquireTokenAsync(resourceId, clientId, new Uri(redirectUrl), new PlatformParameters(PromptBehavior.Auto)).Result;
            return authResult.AccessToken;
        }

        public static bool CreateSchema(string schemaName, string accessToken, string clientId)
        {
            string jsonString = $"{{ \"id\":\"{schemaName}\",\"description\":\"Vehicle Information owned by user\",\"targetTypes\":[\"User\"],\"status\":\"InDevelopment\",\"owner\":\"{clientId}\",\"properties\":[{{\"name\":\"vehicleType\",\"type\":\"String\"}},{{\"name\":\"color\",\"type\":\"String\"}},{{\"name\":\"fuelType\",\"type\":\"String\"}}]}}";
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, graphAPIUrl + "/v1.0/schemaExtensions");
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = client.SendAsync(request).Result;

            string responseResult = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                return true;
            }

            return false;
        }

        public static bool AddUpdateSchemaExtensionValue(string schemaName, string accessToken, string verhicleType, string color, string fuelType, string userId)
        {
            string jsonString = $"{{\"{schemaName}\":{{\"vehicleType\":\"{verhicleType}\",\"color\":\"{color}\",\"fuelType\":\"{fuelType}\"}}}}";
            HttpClient client = new HttpClient();
            var method = new HttpMethod("PATCH");
            HttpRequestMessage request = new HttpRequestMessage(method, graphAPIUrl + $"/v1.0/users/{userId}");
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = client.SendAsync(request).Result;

            string responseResult = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public static bool ReadSchemaExtension(string userId, string extensionName, string accessToken)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, graphAPIUrl + $"/v1.0/users/{userId}?$select=id,{schemaName}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = client.SendAsync(request).Result;

            string responseResult = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                return true;
            }

            return false;
        }

        public static bool RemoveSchemaExtensionValue(string schemaName, string accessToken, string userId)
        {
            string jsonString = $"{{\"{schemaName}\": null}}";
            HttpClient client = new HttpClient();
            var method = new HttpMethod("PATCH");
            HttpRequestMessage request = new HttpRequestMessage(method, graphAPIUrl + $"/v1.0/users/{userId}");
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = client.SendAsync(request).Result;

            string responseResult = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }
}
