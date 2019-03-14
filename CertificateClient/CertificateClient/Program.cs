using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace CertificateClient
{
    class Program
    {
        private static string aadInstance = "https://login.microsoftonline.com/{0}";
        private static string tenant = "35d622d5-3f93-42b1-8984-3e2606dbe321";
        private static string clientId = "3ff68a77-2a93-4998-a91e-5c81e7e96893";
        private static string certName = "CN=CertificateClient";
        static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
        private static string resourceId = "c6b7b3ff-80c6-45f6-aa97-5f70ff8965d9";
        private static string apiBaseAddress = "https://localhost:44300";
        private static AuthenticationContext authContext = null;
        private static ClientAssertionCertificate certCred = null;

        static void Main(string[] args)
        {
            X509Certificate2 cert = ReadCertificateFromStore(certName);
            authContext = new AuthenticationContext(authority);
            certCred = new ClientAssertionCertificate(clientId, cert);
            var result = authContext.AcquireTokenAsync(resourceId, certCred).Result;
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
            HttpClient client = new HttpClient(handler);
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

        private static X509Certificate2 ReadCertificateFromStore(string certName)
        {
            X509Certificate2 cert = null;
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = store.Certificates;

            // Find unexpired certificates.
            X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);

            // From the collection of unexpired certificates, find the ones with the correct name.
            X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certName, false);

            // Return the first certificate in the collection, has the right name and is current.
            cert = signingCert.OfType<X509Certificate2>().OrderByDescending(c => c.NotBefore).FirstOrDefault();
            store.Close();
            return cert;
        }
    }
}
