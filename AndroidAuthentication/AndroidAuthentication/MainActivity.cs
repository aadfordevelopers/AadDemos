using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using APIClientLibrary;
using Android.Content;

namespace AndroidAuthentication
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            Button btnFetchData = FindViewById<Button>(Resource.Id.btnFetchData);
            TextView txtFromAPI = FindViewById<TextView>(Resource.Id.txtFromAPI);
            btnFetchData.Click += async delegate
            {
                string data = await FetchDataFromAPI.GetData(new PlatformParameters(this));
                txtFromAPI.Text = data;
            };
        }
    }
}