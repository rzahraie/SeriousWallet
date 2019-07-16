using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using SeriousWallet3.Database;
using Android.Support.Design.Widget;

namespace SeriousWallet3.Dialogs
{
    public class UserAlertDialog
    {
        static TextInputLayout firstNameLayout, lastNameLayout, phoneNumberLayout, emailLayout;
        static TextInputEditText firstNameEditText, lastNameEditText, phoneNumberEditText, emailEditText;
        static string firstNameError = "First name can't be blank!",
                lastNameError = "Last name can't be blank!",
                phoneNumberError = "Phone number can't be blank!",
                emailError = "E-mail can't be blank!";

        static User user;

        public static User User { get => user; set => user = value; }

        private static bool UserInputsFilled()
        {
            bool a = false, b = false, c = false, d = false;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref firstNameLayout, ref firstNameEditText, firstNameError)) a = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref lastNameLayout, ref lastNameEditText, lastNameError)) b = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref phoneNumberLayout, ref phoneNumberEditText, phoneNumberError)) c = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref emailLayout, ref emailEditText, emailError)) d = true;

            return (a && b && c && d);
        }

        public static void ShowDialog(Context context, LayoutInflater layoutInflater)
        {
            var builder = new AlertDialog.Builder(context);

            View view = layoutInflater.Inflate(Resource.Layout.accountSetupLayout, null, false);

            Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref firstNameLayout, ref firstNameEditText, Resource.Id.accountFirstNameLayout,
                Resource.Id.accountFirstNameEdit, firstNameError);
            Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref lastNameLayout, ref lastNameEditText, Resource.Id.accountLastNameLayout,
                Resource.Id.accountLastNameEdit, lastNameError);
            Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref phoneNumberLayout, ref phoneNumberEditText, Resource.Id.accountPhoneNumberLayout,
                Resource.Id.accountPhoneNumberEdit, phoneNumberError);
            Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref emailLayout, ref emailEditText, Resource.Id.accountEMailLayout,
                Resource.Id.accountEMailEdit, emailError);

            user = DataLayer.GetUserObject();

            firstNameEditText.Text = user.FirstName;
            lastNameEditText.Text = user.LastName;
            phoneNumberEditText.Text = user.PhoneNumber;
            emailEditText.Text = user.EMail;

            builder.SetTitle("Edit user profile");

            builder.SetView(view);

            builder.SetPositiveButton("Ok", (System.EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton(Resource.String.cancel, (senderAlert, args) => { });

            var dialog = builder.Create();

            dialog.Show();

            var okButton = dialog.GetButton((int)DialogButtonType.Positive);

            okButton.Click += (sender, args) =>
            {
                if (!UserInputsFilled()) { }
                else
                {
                    user = new User();
                    user.FirstName = firstNameEditText.Text;
                    user.LastName = lastNameEditText.Text;
                    user.PhoneNumber = phoneNumberEditText.Text;
                    user.EMail = emailEditText.Text;

                    DataLayer.WriteObject(user);

                    dialog.Dismiss();
                }
            };
        }
    }
}