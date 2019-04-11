using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPAuthentication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string clientId = "662fa2a1-7b56-49c3-b392-5fa2beeac8b1";
        const string authority = "https://login.microsoftonline.com/35d622d5-3f93-42b1-8984-3e2606dbe321";
        const string resourceId = "b68f72a1-f591-4512-b22b-03718b8eea41";
        const string baseAddress = "https://azurefunctionauth.azurewebsites.net/api/HttpTrigger1";
        
        private AuthenticationContext authContext = null;
        private Uri redirectURI = null;

        public MainPage()
        {
            this.InitializeComponent();
            redirectURI = Windows.Security.Authentication.Web.WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
            authContext = new AuthenticationContext(authority);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }

        private async Task GetData()
        {
            AuthenticationResult result = null;
            string data = string.Empty;

            try
            {
                result = await authContext.AcquireTokenAsync(resourceId, clientId, redirectURI, new PlatformParameters(PromptBehavior.Auto, false));
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog("An Error has occurred. Please contact admin.");
                await dialog.ShowAsync();
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                HttpResponseMessage response = await httpClient.GetAsync(baseAddress + "?name=mohit");
                if (response.IsSuccessStatusCode)
                {
                    data = response.Content.ReadAsStringAsync().Result;
                }

                txtValue.Text = data;
            }
        }
    }
}
