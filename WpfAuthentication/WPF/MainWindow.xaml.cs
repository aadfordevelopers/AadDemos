using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string clientId = "d239ecaa-8411-4949-b7e4-fcf39be4f38b";
        const string authority = "https://login.microsoftonline.com/35d622d5-3f93-42b1-8984-3e2606dbe321";
        const string resourceId = "c6b7b3ff-80c6-45f6-aa97-5f70ff8965d9";
        const string baseAddress = "https://localhost:44300";
        private readonly Uri redirectURI = new Uri("http://wpf");
        private AuthenticationContext authContext = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            authContext = new AuthenticationContext(authority);
            txtData.Text = "Fetching Data......";
            GetData();
        }

        private async Task GetData()
        {
            AuthenticationResult result = null;
            string data = string.Empty;

            try
            {
                result = await authContext.AcquireTokenAsync(resourceId, clientId, redirectURI, new PlatformParameters(PromptBehavior.Auto));
            }
            catch (Exception ex)
            {
                txtData.Text = "An error occurred.";
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                HttpResponseMessage response = await httpClient.GetAsync(baseAddress + "/api/values");
                if (response.IsSuccessStatusCode)
                {
                    data = response.Content.ReadAsStringAsync().Result;
                }

                txtData.Text = $"Response from API is: {data}";
            }
        }
    }
}
