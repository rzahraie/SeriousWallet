using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using ZXing.Mobile;
using SeriousWallet3.Database;

namespace SeriousWallet3
{
    [Activity(Label = "PaperBackRecoveryActivity")]
    public class PaperBackRecoveryActivity : AppCompatActivity
    {
        TextInputLayout walletPrivateKeyLayout;
        TextInputEditText walletPrivateKeyEdit;
        Button qrCodePrivateButton;
        string walletPrivateKeyError = "Wallet private key can't be blank!";

        TextInputLayout walletPublicKeyLayout;
        TextInputEditText walletPublicKeyEdit;
        Button qrCodePublicButton;
        string walletPublicKeyError = "Wallet public key can't be blank!";

        TextInputLayout bitcoinPrivateKeyLayout;
        TextInputEditText bitcoinPrivateKeyEdit;
        Button qrCodeBitcoinPrivateButton;
        string bitcoinPrivateKeyError = "Bitcoin public key can't be blank!";

        TextInputLayout bitcoinPublicKeyLayout;
        TextInputEditText bitcoinPublicKeyEdit;
        Button qrCodeBitcoinPublicButton;
        string bitCoinPublicKeyError = "Bitcoin public key can't be blank!";

        MobileBarcodeScanner scanner;

        static Coin coin;

        public static Coin Coin { get => coin; set => coin = value; }

        private bool UserInputsFilled()
        {
            bool a = false, b = false, c = false, d = false;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref walletPrivateKeyLayout, ref walletPrivateKeyEdit, walletPrivateKeyError)) a = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref walletPublicKeyLayout, ref walletPublicKeyEdit, walletPublicKeyError)) b = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref bitcoinPrivateKeyLayout, ref bitcoinPrivateKeyEdit, bitcoinPrivateKeyError)) c = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref bitcoinPublicKeyLayout, ref bitcoinPublicKeyEdit, bitCoinPublicKeyError)) d = true;

            return (a && b && c && d);
        }

        private async void ReadQRCode(TextInputEditText editText)
        {
            scanner.UseCustomOverlay = false;
            scanner.TopText = "Hold the camera up to the barcode\nAbout 6 inches away";
            scanner.BottomText = "Wait for the barcode to automatically scan!";

            var result = await scanner.Scan();

            if (result != null) editText.Text = result.Text;
        }

        private void CreateTextLayoutEditAndQrButton(View view, ref TextInputLayout textInputLayout, ref TextInputEditText textInputEditText, 
            int resourceLayoutId,
            int resourceEditId, string errorText, ref Button button, int resourceButtonId)
        {
            textInputLayout = view.FindViewById<TextInputLayout>(resourceLayoutId);
            textInputEditText = view.FindViewById<TextInputEditText>(resourceEditId);

            button = view.FindViewById<Button>(resourceButtonId);

            TextInputEditText textInput = textInputEditText;
            TextInputLayout inputLayout = textInputLayout;
            Button tbutton = button;

            button.Click += delegate
            {
                ReadQRCode(textInput);
            };

            textInputEditText.FocusChange += (sender, e) =>
            {
                if (!e.HasFocus)
                {
                    tbutton.Visibility = ViewStates.Gone;

                    if (string.IsNullOrWhiteSpace(textInput.Text))
                    {
                        inputLayout.ErrorEnabled = true;
                        inputLayout.Error = errorText;
                    }
                }
                else
                {
                    tbutton.Visibility = ViewStates.Visible;

                    inputLayout.Error = "";
                    inputLayout.ErrorEnabled = false;
                }
            };
        }
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            MobileBarcodeScanner.Initialize(Application);

            SetContentView(Resource.Layout.firsttimemain);

            scanner = new MobileBarcodeScanner();

            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);

            LayoutInflater layoutInflater = this.LayoutInflater;

            View view = layoutInflater.Inflate(Resource.Layout.paperbackuprecovery, null, false);

            CreateTextLayoutEditAndQrButton(view, ref walletPrivateKeyLayout, ref walletPrivateKeyEdit, Resource.Id.walletPrivKeyLayout,
                Resource.Id.walletPrivKeyEdit, walletPrivateKeyError, ref qrCodePrivateButton, Resource.Id.qrWalletCodePrivate);

            CreateTextLayoutEditAndQrButton(view, ref walletPublicKeyLayout, ref walletPublicKeyEdit, Resource.Id.walletPubKeyLayout,
                Resource.Id.walletPubKeyEdit, walletPublicKeyError, ref qrCodePublicButton, Resource.Id.qrWalletCodePrivate);

            CreateTextLayoutEditAndQrButton(view, ref bitcoinPrivateKeyLayout, ref bitcoinPrivateKeyEdit, Resource.Id.bitcoinPrivKeyLayout,
                Resource.Id.bitcoinPrivKeyEdit, bitcoinPrivateKeyError, ref qrCodeBitcoinPrivateButton, Resource.Id.qrCodeBitcoinPrivate);

            CreateTextLayoutEditAndQrButton(view, ref bitcoinPublicKeyLayout, ref bitcoinPublicKeyEdit, Resource.Id.bitcoinPubKeyLayout,
               Resource.Id.bitcoinPubKeyEdit, bitCoinPublicKeyError, ref qrCodeBitcoinPublicButton, Resource.Id.qrCodeBitcoinPublic);

            builder.SetTitle("Paper Backup Recovery");

            builder.SetView(view);

            builder.SetPositiveButton(Resource.String.next, (System.EventHandler<DialogClickEventArgs>)null);

            builder.SetNegativeButton(Resource.String.cancel, (senderAlert, args) => { Finish(); });

            builder.SetNeutralButton(Resource.String.prev, (senderAlert, args) => 
            {
                StartActivity(new Intent(Application.Context, typeof(WalletSetupActivity)));
            });

            var dialog = builder.Create();

            dialog.Show();

            var nextBtn = dialog.GetButton((int)DialogButtonType.Positive);

            nextBtn.Click += (sender, args) =>
            {
                if (!UserInputsFilled()) { }
                else
                {
                    coin = new Coin();

                    coin.Symbol = "BTC";
                    coin.PrivateKey = bitcoinPrivateKeyEdit.Text;
                    coin.PublicAddress = bitcoinPublicKeyEdit.Text;
                    coin.WalletName = WalletSetupActivity.Wallet.WalletName;

                    StartActivity(new Intent(Application.Context, typeof(NumberOfSignatoriesActivity)));
                }
            };
        }
    }
}