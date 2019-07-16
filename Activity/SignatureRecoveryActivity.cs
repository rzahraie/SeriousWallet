using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using SeriousWallet3.Database;
using System.Collections.Generic;

namespace SeriousWallet3
{
    [Activity(Label = "SignatureRecoveryActivity")]
    public class SignatureRecoveryActivity : AppCompatActivity
    {
        TextInputLayout nameLayout;
        TextInputEditText nameEdit;
        string nameError = "Name can't be blank!";

        TextInputLayout mobilePhoneNumberLayout;
        TextInputEditText mobilePhoneNumberEdit;
        string mobilePhoneError = "Mobile phone number can't be blank!";

        TextInputLayout emailAddressLayout;
        TextInputEditText emailAddressEdit;
        string emailError = "E-mail can't be blank!";

        TextInputLayout publicWalletKeyLayout;
        TextInputEditText publicWalletKeyEdit;
        string walletError = "Public wallet key can't be blank!";

        static List<Signatory> signatoryList = new List<Signatory>();
        static int currentSignatory = 1;
        static bool firstTimeUse = true;
        static bool initialized = false;
        int prevButtonResourceText;

        public static List<Signatory> SignatoryList { get => signatoryList; set => signatoryList = value; }
        public static string WalletName { get; set; }
        public static bool FirstTimeUse { get => firstTimeUse; set => firstTimeUse = value; }
        public static int NumberOfSignatories { get; set; }
        public static int CurrentSignatory { get => currentSignatory; set => currentSignatory = value; }

        Android.Support.V7.App.AlertDialog dialog;

        private bool UserInputsFilled()
        {
            bool a = false, b = false, c = false, d = false;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref nameLayout, ref nameEdit, nameError)) a = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref mobilePhoneNumberLayout, ref mobilePhoneNumberEdit, mobilePhoneError)) b = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref emailAddressLayout, ref emailAddressEdit, emailError)) c = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref publicWalletKeyLayout, ref publicWalletKeyEdit, walletError)) d = true;

            return (a && b && c && d);
        }

        private void WriteDataToDatabase()
        {
            DataLayer.WriteObject(AccountSetupActivity.User);
            DataLayer.WriteObject(WalletSetupActivity.Wallet);
            //DataLayer.WriteObject(PaperBackRecoveryActivity.Coin); //*
            DataLayer.SetCurrentWallet(WalletSetupActivity.Wallet.WalletName);

            foreach(Signatory s in signatoryList)
            {
                DataLayer.WriteObject(s);
            }
        }

        private void Init()
        {
            List<Signatory> signatories = null;

            if (!FirstTimeUse && !initialized)
            {
                signatories = DataLayer.GetSignatories(WalletName);
                NumberOfSignatories = signatories.Count;

                initialized = true;
            }

            if (!FirstTimeUse)
            {
                nameEdit.Text = signatories?[CurrentSignatory - 1].Name;
                mobilePhoneNumberEdit.Text = signatories?[CurrentSignatory - 1].MobilePhoneNumber;
                emailAddressEdit.Text = signatories?[CurrentSignatory - 1].EMailAddress;
                publicWalletKeyEdit.Text = signatories?[CurrentSignatory - 1].PublicAddress;

                prevButtonResourceText = Resource.String.cancel;
            }
            else prevButtonResourceText = Resource.String.prev;


        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.firsttimemain);

            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);

            Android.Views.LayoutInflater layoutInflater = this.LayoutInflater;

            Android.Views.View view = layoutInflater.Inflate(Resource.Layout.signaturerecovery, null, false);

            Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref nameLayout, ref nameEdit, Resource.Id.nameSignLayout, Resource.Id.nameSignEdit,
                nameError);
            Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref mobilePhoneNumberLayout, ref mobilePhoneNumberEdit, 
                Resource.Id.mobilePhoneNumberSignLayout,
                Resource.Id.mobilePhoneNumberSignEdit, mobilePhoneError);
            Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref emailAddressLayout, ref emailAddressEdit, Resource.Id.emailAddressSignLayout,
                Resource.Id.emailAddressSignEdit, emailError);
            Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref publicWalletKeyLayout, ref publicWalletKeyEdit, Resource.Id.pubWalletKeySignLayout,
                Resource.Id.pubWalletKeySignEdit, walletError);

            Init();

            builder.SetTitle(string.Format((NumberOfSignatories > 1 ? "Signatories" : "Signatory") + " {0} of {1}", 
                CurrentSignatory, NumberOfSignatories));

            builder.SetView(view);

            builder.SetNeutralButton(prevButtonResourceText, (senderAlert, args) =>
            {
                if (!firstTimeUse)
                {
                    dialog.Dismiss();

                    OnBackPressed();

                    return;
                }

                if (CurrentSignatory >= 2)
                {
                    CurrentSignatory--;
                    StartActivity(new Intent(Application.Context, typeof(SignatureRecoveryActivity)));
                }
                else StartActivity(new Intent(Application.Context, typeof(NumberOfSignatoriesActivity)));

            });

            if (CurrentSignatory < NumberOfSignatories)
            {
                builder.SetPositiveButton(Resource.String.next, (System.EventHandler<DialogClickEventArgs>)null);
                builder.SetNegativeButton(Resource.String.cancel, (senderAlert, args) => { Finish(); });

                dialog = builder.Create();

                dialog.Show();

                var nextBtn = dialog.GetButton((int)DialogButtonType.Positive);

                nextBtn.Click += (sender, args) =>
                {
                    if (!UserInputsFilled())
                    {

                    }
                    else
                    {
                        string swalletName = WalletSetupActivity.Wallet.WalletName;

                        signatoryList.Add(new Signatory()
                        {
                            Name = nameEdit.Text,
                            EMailAddress = emailAddressEdit.Text,
                            MobilePhoneNumber = mobilePhoneNumberEdit.Text,
                            PublicAddress = publicWalletKeyEdit.Text,
                            WalletName = swalletName
                        });

                        CurrentSignatory++;
                        StartActivity(new Intent(Application.Context, typeof(SignatureRecoveryActivity)));
                    }
                };
            }
            else
            {
                builder.SetPositiveButton(Resource.String.done, (System.EventHandler<DialogClickEventArgs>)null);

                dialog = builder.Create();

                dialog.Show();

                var doneBtn = dialog.GetButton((int)DialogButtonType.Positive);

                doneBtn.Click += (sender, args) =>
                {
                    if (!UserInputsFilled())
                    {
                        //do nothing
                    }
                    else
                    {
                        string swalletName = WalletSetupActivity.Wallet.WalletName;

                        signatoryList.Add(new Signatory()
                        {
                            Name = nameEdit.Text,
                            EMailAddress = emailAddressEdit.Text,
                            MobilePhoneNumber = mobilePhoneNumberEdit.Text,
                            PublicAddress = publicWalletKeyEdit.Text,
                            WalletName = swalletName
                        });

                        try
                        {
                            WriteDataToDatabase();
                        }
                        catch(System.Exception e)
                        {
                            MainActivity.setupError = true;
                            MainActivity.setupErrorText = "Setup Error : " + e.ToString();
                        }
                       

                        StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                    }
                };
            }
        }
    }
}