using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SeriousWallet3.Database;

namespace SeriousWallet3
{
    [Activity(Label = "WalletSetupActivity")]
    public class WalletSetupActivity : AppCompatActivity
    {
        TextInputLayout walletNameLayout;
        TextInputEditText walletNameEditText;
        string walleNameError = "Wallet name can't be blank!";

        TextInputLayout walletDescriptionLayout;
        TextInputEditText walletDescriptionEditText;
        string walletDescriptionError = "Wallet description can't be blank!";

        static CoinDataModel.Wallet wallet;

        public static CoinDataModel.Wallet Wallet { get => wallet; set => wallet = value; }

        private bool UserInputsFilled()
        {
            bool a = false, b = false;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref walletNameLayout, ref walletNameEditText, walleNameError)) a = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref walletDescriptionLayout, ref walletDescriptionEditText, walletDescriptionError)) b = true;

            return (a && b);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.firsttimemain);

            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);

            LayoutInflater layoutInflater = this.LayoutInflater;

            View view = layoutInflater.Inflate(Resource.Layout.walletSetup, null, false);

            Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref walletNameLayout, ref walletNameEditText, Resource.Id.walletNameLayout,
              Resource.Id.walletNameEdit, walleNameError);

            Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref walletDescriptionLayout, ref walletDescriptionEditText, 
                Resource.Id.walletDescriptionLayout,
              Resource.Id.walletDescriptionEdit, walletDescriptionError);

            builder.SetTitle("Wallet Information");

            builder.SetView(view);

            builder.SetPositiveButton(Resource.String.next, (System.EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton(Resource.String.prev, (senderAlert, args) => { StartActivity(new Intent(Application.Context, 
                typeof(FirstTimeLaunchActivity))); });

            var dialog = builder.Create();

            dialog.Show();

            var nextBtn = dialog.GetButton((int)DialogButtonType.Positive);

            nextBtn.Click += (sender, args) =>
            {
                if (!UserInputsFilled())
                {
                    //do nothing
                }
                else
                {
                    wallet = new CoinDataModel.Wallet(walletNameEditText.Text, walletDescriptionEditText.Text);

                    StartActivity(new Intent(Application.Context, typeof(PaperBackRecoveryActivity)));
                }
                
            };
        }
    }
}