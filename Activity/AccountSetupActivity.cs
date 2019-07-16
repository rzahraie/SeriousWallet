using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using SeriousWallet3.Database;

namespace SeriousWallet3
{
    [Activity(Label = "@string/app_name")]
    public class AccountSetupActivity : AppCompatActivity
    {
        
        TextInputLayout firstNameLayout, lastNameLayout, phoneNumberLayout, emailLayout;
        TextInputEditText firstNameEditText, lastNameEditText, phoneNumberEditText, emailEditText;
        string firstNameError = "First name can't be blank!",
            lastNameError = "Last name can't be blank!",
            phoneNumberError = "Phone number can't be blank!",
            emailError = "E-mail can't be blank!";

        static User user;

        public static User User { get => user; set => user = value; }

        private bool UserInputsFilled()
        {
            bool a = false, b= false, c = false, d = false;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref firstNameLayout, ref firstNameEditText, firstNameError)) a = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref lastNameLayout, ref lastNameEditText, lastNameError)) b = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref phoneNumberLayout, ref phoneNumberEditText, phoneNumberError)) c = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref emailLayout, ref emailEditText, emailError)) d = true;

            return (a && b && c && d);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.firsttimemain);

            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);

            LayoutInflater layoutInflater = this.LayoutInflater;

            View view = layoutInflater.Inflate(Resource.Layout.accountSetupLayout, null, false);

            Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref firstNameLayout, ref firstNameEditText, Resource.Id.accountFirstNameLayout,
                Resource.Id.accountFirstNameEdit, firstNameError);
            Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref lastNameLayout, ref lastNameEditText, Resource.Id.accountLastNameLayout,
                Resource.Id.accountLastNameEdit, lastNameError);
            Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref phoneNumberLayout, ref phoneNumberEditText, Resource.Id.accountPhoneNumberLayout,
                Resource.Id.accountPhoneNumberEdit, phoneNumberError);
            Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref emailLayout, ref emailEditText, Resource.Id.accountEMailLayout,
                Resource.Id.accountEMailEdit, emailError);

            builder.SetTitle("Account Setup");

            builder.SetView(view);

            builder.SetPositiveButton(Resource.String.next, (System.EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton(Resource.String.cancel, (senderAlert, args) => { Finish(); });

            var dialog = builder.Create();

            dialog.Show();

            var nextBtn = dialog.GetButton((int)DialogButtonType.Positive);

            nextBtn.Click += (sender, args) =>
            {
                if (!UserInputsFilled()) { }
                else
                {
                    user = new User();
                    user.FirstName = firstNameEditText.Text;
                    user.LastName = lastNameEditText.Text;
                    user.PhoneNumber = phoneNumberEditText.Text;
                    user.EMail = emailEditText.Text;

                    if (DataLayer.GetWalletsEx().Count > 0) StartActivity(new Intent(Application.Context, typeof(FirstTimeLaunchActivity)));
                    else StartActivity(new Intent(Application.Context, typeof(WalletSetupActivity)));
                }
            };
        }
    }
}