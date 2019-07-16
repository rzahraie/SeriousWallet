using Android.App;
using Android.Views;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Widget;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Mobile;
using System.Threading.Tasks;

namespace SeriousWallet3.Utilities
{
    class AndroidUtility
    {
        private static MobileBarcodeScanner scanner;
        private static bool scannerInitialized = false;

        private static void InitializeScanner(Application application)
        {
            if (scannerInitialized) return;

            MobileBarcodeScanner.Initialize(application);

            scanner = new MobileBarcodeScanner();

            scannerInitialized = true;
        }

        public static Bitmap CreateQRCode(string address)
        {
            QRCodeWriter write = new QRCodeWriter();

            int size = 120;

            BitMatrix matrix = write.encode(address, ZXing.BarcodeFormat.QR_CODE, size, size, null);
            BitmapRenderer bit = new BitmapRenderer();
            Bitmap bitmap = bit.Render(matrix, BarcodeFormat.QR_CODE, address);

            return bitmap;
        }

        public static async Task<string> ReadQRCode(Application application)
        {
            InitializeScanner(application);

            scanner.UseCustomOverlay = false;
            scanner.TopText = "Hold the camera up to the barcode\nAbout 6 inches away";
            scanner.BottomText = "Wait for the barcode to automatically scan!";

            var result = await scanner.Scan();

            if (result != null) return result.Text;
            else return "";
        }

        public static void CreateTextLayoutEdit(View view, ref TextInputLayout textInputLayout, 
            ref TextInputEditText textInputEditText, int resourceLayoutId,
            int resourceEditId, string errorText, LinearLayout linearLayout = null)
        {
            textInputLayout = view.FindViewById<TextInputLayout>(resourceLayoutId);
            textInputEditText = view.FindViewById<TextInputEditText>(resourceEditId);

            TextInputEditText textInput = textInputEditText;
            TextInputLayout inputLayout = textInputLayout;

            textInputEditText.FocusChange += (sender, e) =>
            {
                if (!e.HasFocus)
                {
                    if (linearLayout != null) linearLayout.Visibility = ViewStates.Gone;

                    if (string.IsNullOrWhiteSpace(textInput.Text))
                    {
                        inputLayout.ErrorEnabled = true;
                        inputLayout.Error = errorText;
                    }
                    else
                    {
                        inputLayout.Error = "";
                        inputLayout.ErrorEnabled = false;
                    }
                }
                else if (linearLayout != null) linearLayout.Visibility = ViewStates.Visible;
            };
        }

        public static bool IsTextEditBlank(ref TextInputLayout textInputLayout, ref TextInputEditText textInputEditText,
            string errorText)
        {
            if (textInputEditText.Text.Length <= 0)
            {
                textInputLayout.ErrorEnabled = true;
                textInputLayout.Error = errorText;

                return true;
            }
            else
            {
                textInputLayout.ErrorEnabled = false;
                textInputLayout.Error = "";

                return false;
            }
        }
    }
}