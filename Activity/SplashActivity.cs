using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using SeriousWallet3.DataStructures;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace SeriousWallet3
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true,
        LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataSchemes = new string[] { "content", "file" },
        DataMimeTypes = new string[] { "*/*", "application/stream-octet", "application/octet-stream", "application/scw" },
        DataHost = "*",
        DataPathPatterns = new string[] { ".*", ".*\\.scw", ".*\\..*\\.scw", ".*\\..*\\..*\\.scw", ".*\\..*\\..*\\..*\\.scw" })]
    public class SplashActivity : AppCompatActivity
    {
        bool startupByAttachment = false;
        SeriousWalletTransactionMessage message;

        private void ProcessAttachment()
        {
            if (Intent.Action.Equals(Intent.ActionView))
            {
                try
                {
                    Android.Net.Uri uri = Intent.Data;
                    string scheme = uri.Scheme;
                    string mimetype = ContentResolver.GetType(uri);

                    if ((ContentResolver.SchemeFile == "file") && (mimetype == "application/octet-stream"))
                    {
                        startupByAttachment = true;

                        System.IO.Stream stream = ContentResolver.OpenInputStream(uri);
                        System.IO.StreamReader sr = new System.IO.StreamReader(stream);

                        var serializer = new JsonSerializer();

                        using (var jst = new JsonTextReader(sr))
                        {
                            message = serializer.Deserialize<SeriousWalletTransactionMessage>(jst);
                        }
                    }
                    else
                    {
                        Finish();
                    }
                }
                catch (System.Exception e)
                {
                    //@@todo write exception to log
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }

        protected override void OnResume()
        {
            base.OnResume();

            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();

            ProcessAttachment();
        }

        public override void OnBackPressed() { }

        async void SimulateStartup()
        {
            await Task.Delay(4000); // Simulate a bit of startup work.

            if (startupByAttachment)
            {
                MainActivity.startUpByFile = true;
                MainActivity.seriousWalletTransactionMessage = message;

                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }
            else
            {
                if (Database.DataLayer.FirstTimeUse) StartActivity(new Intent(Application.Context, typeof(AccountSetupActivity)));
                else StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }
        }
    }
}