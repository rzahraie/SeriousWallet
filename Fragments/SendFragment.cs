using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views.InputMethods;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using CoinDataModel;
using SeriousWallet3.AddressBookInfo;
using SeriousWallet3.Database;
using SeriousWallet3.DataStructures;
using SeriousWallet3.CryptoManagers;
using Newtonsoft.Json;

namespace SeriousWallet3.Fragments
{
    public class SendFragment : Fragment
    {
        TextInputLayout addressLayout;
        TextInputLayout labelLayout;
        TextInputLayout amountLayout;

        TextInputEditText addressEdit;
        TextInputEditText labelEdit;
        TextInputEditText amountEdit;

        TextView dollarAmountTextView;

        

        string addressBlankError = "Address can't be blank!";
        string labelBlankError = "Label can't be blank!";
        string amountBlankError = "Amount can't be blank!";

        Android.Support.V7.Widget.RecyclerView recyclerView;
        AddressBookAdapter addressBookAdapter;

        LinearLayout buttonLayout;

        Button addressBookButton;
        Button qrCodeButton;
        Button clearButton;

        Button sendButton;
        RadioGroup radioGroup;
        View topView;
        TextView transactionFeeText;
        View bottomView;

        DataRepository dataRepository;

        List<ExpandedMenuModel> listDataHeader;
        Dictionary<ExpandedMenuModel, List<KeyValuePair<String,int>>> listDataChild;
        static SeriousWalletTransactionMessage seriousWalletTransactionMessage;
        static bool transactionByFileAttachment = false;
        static bool sendForm = true;
        static decimal coinDollarValue;
        static string symbol;
        static CoinManager.ReportResult reportResultDelegate;
        static string fromPrivateAddress;
        static string toName;
        static string walletName;

        enum TransactionEnum { SEND, RECEIVE };
        TransactionEnum transactionState = TransactionEnum.SEND;

        int index;

        private static SendFragment sendFragment = null;

        public void OnEmailAddress(string emailAddress)
        {
            if (transactionState == TransactionEnum.RECEIVE)
            {
                addressEdit.Text = emailAddress;
            }
        }

        public static void OnName(string name)
        {
            toName = name;
 
        }

        public void OnCryptoAddress(string cryptoAddress)
        {
            if (transactionState == TransactionEnum.SEND)
            {
                addressEdit.Text = cryptoAddress;
            }
        }

        private void CreateRecyclerView(View view)
        {
            recyclerView = view.FindViewById<Android.Support.V7.Widget.RecyclerView>(Resource.Id.addressRecyclerView);

            Android.Support.V7.Widget.DividerItemDecoration dividerItemDecoration = new
                Android.Support.V7.Widget.DividerItemDecoration(recyclerView.Context, Android.Support.V7.Widget.LinearLayoutManager.Vertical);

            List<AddressBook> addressBookList = new List<AddressBook>();

            addressBookAdapter = new AddressBookAdapter(addressBookList);

            recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(Context));
            recyclerView.SetAdapter(addressBookAdapter);
            recyclerView.AddItemDecoration(dividerItemDecoration);
            ClickListener clickListener = new ClickListener(Context, addressEdit);
            clickListener.recyclerView = recyclerView;
            //recyclerView.AddOnItemTouchListener(new RecyclerViewOnItemTouchListener(Context, recyclerView, clickListener));
            recyclerView.AddOnItemTouchListener(new RecyclerViewOnItemTouchListener(Context, 
                recyclerView, OnEmailAddress, OnName, OnCryptoAddress, clickListener));

            addressBookList.Add(new AddressBook("Steven Howchenhowser", "showchenhowdress@gmail.com", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"));
            addressBookList.Add(new AddressBook("Taun Olson", "taun@seriouscoin.io", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"));
            addressBookList.Add(new AddressBook("Todd Hoffman", "toddh@seriouscoin.io", "PKM90iag9jJgTHD1VXjvLCEnZuQ3rTCF0Q"));
            addressBookList.Add(new AddressBook("Todd Swasinger", "todds@seriouscoin.io", "20M90iag9jJgTHD1VXjvLCEnZuQ3rTCF0Q"));
            addressBookList.Add(new AddressBook("Ramin Zahraie", "ramin@seriouscoin.io", "58M90iag9jJgTHD1VXjvLCEnZuQ3rTCH1X"));
            addressBookList.Add(new AddressBook("Steve Smith", "ssmith@gmail.com", "58M90iag9jJgTHD1VXjvLCEnZuQ3rTCH2H"));
            addressBookList.Add(new AddressBook("William Johnson", "wjohnson@gmail.com", "58M90iag9jJgTHD1VXjvLCEnZuQ3rTCH27T"));

            addressBookAdapter.NotifyDataSetChanged();
        }

        private void CreateTextViewsAndButtons(View view)
        {
            amountLayout = view.FindViewById<TextInputLayout>(Resource.Id.amountLayoutFrag);
            addressLayout = view.FindViewById<TextInputLayout>(Resource.Id.userNameLayoutFrag);
            labelLayout = view.FindViewById<TextInputLayout>(Resource.Id.labelLayoutFrag);

            amountEdit = view.FindViewById<TextInputEditText>(Resource.Id.amountEditFrag);
            addressEdit = view.FindViewById<TextInputEditText>(Resource.Id.userNameTextEditFrag);
            labelEdit = view.FindViewById<TextInputEditText>(Resource.Id.labelEditFrag);
            dollarAmountTextView = view.FindViewById<TextView>(Resource.Id.dollarAmountTextViewFrag);
            
            amountEdit.ClearFocus();

            amountEdit.RequestFocus();

            //@@todo: convert to lambda functions
            amountEdit.FocusChange += AmountTextView_FocusChange;

            dollarAmountTextView.SetTextColor(Color.Red);

            amountEdit.TextChanged += (sender, e) =>
            {
                string amount = amountEdit.Text;

                if (!amount.Any(char.IsDigit)) return;

                decimal value = System.Convert.ToDecimal(amount) * coinDollarValue;
                dollarAmountTextView.Text = value.ToString("C");
            };

            addressEdit.FocusChange += AddressTextView_FocusChange;
            addressEdit.TextChanged += (sender, e) =>
            {
                addressBookAdapter.Filter(e.Text.ToString());
            };

            //@@todo: convert to lambda functions
            labelEdit.FocusChange += LabelTextView_FocusChange;

            buttonLayout = view.FindViewById<LinearLayout>(Resource.Id.buttonLayoutFrag);

            addressBookButton = view.FindViewById<Button>(Resource.Id.addressBookFrag);
            qrCodeButton = view.FindViewById<Button>(Resource.Id.qrCodeFrag);
            clearButton = view.FindViewById<Button>(Resource.Id.clearFrag);

            clearButton.Click += (sender, e) =>
            {
                addressEdit.Text = null;
            };

            //@@todo: convert to lambda functions
            addressBookButton.Click += AddressBookButton_Click;

            sendButton = view.FindViewById<Button>(Resource.Id.sendFrag);
            radioGroup = view.FindViewById<RadioGroup>(Resource.Id.feesRadioGroup);
            topView = view.FindViewById<View>(Resource.Id.topView);
            transactionFeeText = view.FindViewById<TextView>(Resource.Id.transactionFeeText);
            bottomView = view.FindViewById<View>(Resource.Id.bottomView);

            //@@todo: convert to lambda functions
            sendButton.Click += async (sender , e) =>
            {
                if (!UserInputsFilled()) return;

                switch (transactionState)
                {
                    case TransactionEnum.RECEIVE:
                        HandleReceiveTask();
                        break;

                    case TransactionEnum.SEND:
                        await HandleSendTaks();
                        break;

                    default:
                        break;
                }
            };
        }

        public static SendFragment NewInstance(string sym, bool sendFrm, bool transactByFileAttach, decimal coindollarValue,
            CoinManager.ReportResult reportresult, string fromPrivAddress, string walletNam,
            SeriousWalletTransactionMessage seriousWalletTransaction = null)
        {
            symbol = sym;

            sendForm = sendFrm;

            transactionByFileAttachment = transactByFileAttach;

            coinDollarValue = coindollarValue;

            reportResultDelegate = reportresult;

            fromPrivateAddress = fromPrivAddress;
            
            if (transactionByFileAttachment) seriousWalletTransactionMessage = seriousWalletTransaction;

            walletName = walletNam;

            return ((sendFragment == null) ?
                (sendFragment = new SendFragment { Arguments = new Bundle() }) :
                sendFragment);
        }

        //@@todo: remove
        //private void ShowAddressBook()
        //{
        //    var builder = new Android.Support.V7.App.AlertDialog.Builder(Context);

        //    builder.SetTitle("Address book");

        //    LayoutInflater layoutInflater = Activity.LayoutInflater;

        //    View view = layoutInflater.Inflate(Resource.Layout.addressbooklayoutex, null, false);

        //    ExpandableListView expandableListView = view.FindViewById<ExpandableListView>(Resource.Id.exapndableListView);

        //    PrepareAddressBook();

        //    ExpandableListAdapter expandableListAdapter = new ExpandableListAdapter(builder.Context, listDataHeader, listDataChild,
        //        expandableListView);

        //    expandableListView.SetAdapter(expandableListAdapter);

        //    expandableListView.ChildClick += delegate (object sender, ExpandableListView.ChildClickEventArgs e)
        //    {
              
        //        List<KeyValuePair<string,int>> ls = listDataChild[listDataHeader[e.GroupPosition]];

        //        if (ls[(int)e.Id].Key == "Portfolio")
        //        {
                    
        //        }

        //        //Toast.MakeText(this, "child clicked : " + ls[(int)e.Id], ToastLength.Short).Show();
        //    };

        //    builder.SetView(view);

        //    builder.SetPositiveButton("Ok", (senderAlert, args) =>
        //    {

        //    });

        //    builder.SetNegativeButton(Resource.String.cancel, (senderAlert, args) => { });

        //    builder.Show();

        //}

        private void AddToAddressBook(string name, string phoneNumber, string address, string emailAddress)
        {
            ExpandedMenuModel item1 = new ExpandedMenuModel();
            item1.Name = name;
            item1.Image = Resource.Drawable.ic_account_circle_black_18dp;
            // Adding data header
            listDataHeader.Add(item1);

            // Adding child data
            List<KeyValuePair<String, int>> heading1 = new List<KeyValuePair<String, int>>();
            heading1.Add(new KeyValuePair<string, int>(phoneNumber, Resource.Drawable.ic_phone_blue_light_18dp));
            heading1.Add(new KeyValuePair<string, int>(address, Resource.Drawable.ic_qr_code_scan_blue));
            heading1.Add(new KeyValuePair<string, int>(emailAddress, Resource.Drawable.ic_email_blue_light_18dp));
            listDataChild.Add(listDataHeader[index], heading1);// Header, Child data
            index++;
        }

        private void PrepareAddressBook()
        {
            index = 0;

            listDataHeader = new List<ExpandedMenuModel>();
            listDataChild = new Dictionary<ExpandedMenuModel, List<KeyValuePair<String, int>>>();

            AddToAddressBook("Joel Peterson", "480-555-1234", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L", "jpeterson@gmail.com");
            AddToAddressBook("Steve Patterson", "480-765-5321", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE8L", "spaterson@gmail.com");
            AddToAddressBook("Lisa Conners", "480-365-4221", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE8L", "lconnors@gmail.com");
            AddToAddressBook("Mike Jacobs", "480-737-1024", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE8L", "mjacobs@gmail.com");
            AddToAddressBook("Nancy Swartz", "480-767-4834", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE8L", "nswartz@gmail.com");
            AddToAddressBook("Steve Hurly", "480-576-6493", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE8L", "shurly@gmail.com");
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override void OnResume()
        {
            base.OnResume();

            amountEdit.Text = "";
            addressEdit.Text = "";
            labelEdit.Text = "";
            dollarAmountTextView.Text = "";

            addressLayout.ErrorEnabled = false;
            labelLayout.ErrorEnabled = false;
            amountLayout.ErrorEnabled = false;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment

            View view = inflater.Inflate(Resource.Layout.sendfragmentex, container, false);

            CreateTextViewsAndButtons(view);

            CreateRecyclerView(view);

            dataRepository = new DataRepository();

            //@@todo: change to transactionState
            if (!sendForm)
            {
                addressLayout.Hint = "E-mail address";
                addressBlankError = "E-mail address can't be blank!";
                amountEdit.SetTextColor(Color.Green);
                dollarAmountTextView.SetTextColor(Color.Green);
                radioGroup.Visibility = ViewStates.Gone;
                topView.Visibility = ViewStates.Gone;
                transactionFeeText.Visibility = ViewStates.Gone;
                bottomView.Visibility = ViewStates.Gone;
                transactionState = TransactionEnum.RECEIVE;

                sendButton.Text = "Receive payment";
            }

            if (transactionByFileAttachment)
            {
                addressLayout.Hint = "Bitcoin public address";
                addressBlankError = "Bitcoin public address can't be blank!";
                amountEdit.SetTextColor(Android.Graphics.Color.Red);
                amountEdit.Text = seriousWalletTransactionMessage.CoinAmount;
                addressEdit.Text = seriousWalletTransactionMessage.TransactionAddress;
                labelEdit.RequestFocus();
                InputMethodManager imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
                imm.ShowSoftInput(labelEdit, ShowFlags.Implicit);
            }

            return view;
        }

        private Android.Net.Uri WriteJSONFile(string fileName, string transctionTyp, string transactionID = null)
        {
            SendJSONMail sendJSONMail = new SendJSONMail()
            {
                transactionId = (transactionID != null) ? transactionID : Java.Util.UUID.RandomUUID().ToString(),
                senderEmail = DataLayer.GetSingleObject<User>()?.EMail,
                transactionType = transctionTyp,
                coinAmount = amountEdit.Text,
                senderName = DataLayer.GetSingleObject<User>()?.FirstName + " " + DataLayer.GetSingleObject<User>()?.LastName,
                authentication = Java.Util.UUID.RandomUUID().ToString(),
                coinType = "BTC", //@@todo: make variable
                transactionAddress = DataLayer.GetSingleObject<Coin>()?.PublicAddress,
                senderPhone = DataLayer.GetSingleObject<User>()?.PhoneNumber
            };

            var documents = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            var filename = System.IO.Path.Combine(documents.ToString(), fileName);

            using (System.IO.StreamWriter sw = System.IO.File.CreateText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(sw, sendJSONMail);
            }

            Java.IO.File dir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            Java.IO.File file = new Java.IO.File(dir, fileName);
            Android.Net.Uri path = Android.Net.Uri.FromFile(file);

            return path;
        }

        private void SendCoinRequestEmail()
        {
            Android.Net.Uri uri = WriteJSONFile("Request04122019.scw","0");

            var emailIntent = new Intent(Intent.ActionSend);

            emailIntent.PutExtra(Intent.ExtraEmail, new string[] { addressEdit.Text });
            emailIntent.PutExtra(Intent.ExtraSubject, "Bitcoin Request");
            emailIntent.PutExtra(Intent.ExtraStream, uri);
            emailIntent.SetType("message/rfc822");
            
            StartActivityForResult(emailIntent,0);
        }

        private bool UserInputsFilled()
        {
            bool a = false, b = false, c = false;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref addressLayout, ref addressEdit, addressBlankError)) a = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref labelLayout, ref labelEdit, labelBlankError)) b = true;

            if (!Utilities.AndroidUtility.IsTextEditBlank(ref amountLayout, ref amountEdit, amountBlankError)) c = true;

            return (a && b && c);
        }

        private void HandleReceiveTask()
        {
            SendCoinRequestEmail();

            //@@todo: change to database
            dataRepository.AddTransactionData(DateTime.Now, addressEdit.Text, labelEdit.Text, Convert.ToDecimal(amountEdit.Text),
                   GetValue(amountEdit.Text), "Receive");

            var activity = (MainActivity)this.Activity;

            activity.CloseReceiveFragment(addressEdit.Text);
        }

        private async Task HandleSendTaks()
        {
            var activity = (MainActivity)Activity;

            CoinManager coinManager = CryptoClasses.CoinManagerDict[symbol];

            await coinManager.SendCoinTransaction(fromPrivateAddress, toName, addressEdit.Text, Convert.ToDecimal(amountEdit.Text), 
                Convert.ToDecimal(dollarAmountTextView.Text), 
                "USD", walletName, reportResultDelegate);

            activity.CloseSendFragment(addressEdit.Text);
        }

        private async Task SendButton_Click(object sender, EventArgs e)
        {
            if (!UserInputsFilled()) return;

            switch(transactionState)
            {
                case TransactionEnum.RECEIVE:
                    HandleReceiveTask();
                    break;

                case TransactionEnum.SEND:
                    await HandleSendTaks();
                    break;

                default:
                    break;
            }
        }

        private void AddressBookButton_Click(object sender, EventArgs e)
        {
            ////ShowAddressBook();
            //AddressBookFragment addressBookFragment = AddressBookFragment.NewInstance();
            //FragmentManager.BeginTransaction()
            //    .Replace(Resource.Id.mainFrameLayout, addressBookFragment)
            //    .AddToBackStack(null)
            //    .Commit();

            recyclerView.Visibility = (recyclerView.Visibility != ViewStates.Visible ? ViewStates.Visible : ViewStates.Gone);

        }

        //@@todo
        private decimal GetValue(string coins)
        {
            return (Convert.ToDecimal(coins) * 7863m);
        }

        private void LabelTextView_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                if (string.IsNullOrWhiteSpace(labelEdit.Text))
                {
                    labelLayout.ErrorEnabled = true;
                    labelLayout.Error = "Label can't be blank!";
                }
            }
            else
            {
                labelLayout.Error = "";
                labelLayout.ErrorEnabled = false;
            }
        }

        private void AddressTextView_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                if (string.IsNullOrWhiteSpace(addressEdit.Text))
                {
                    addressLayout.ErrorEnabled = true;
                    addressLayout.Error = "Address can't be blank!";
                }

                buttonLayout.Visibility = ViewStates.Gone;
                recyclerView.Visibility = ViewStates.Gone;
            }
            else
            {
                addressLayout.Error = "";
                addressLayout.ErrorEnabled = false;

                buttonLayout.Visibility = ViewStates.Visible;
            }
        }

        private void AmountTextView_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            //@@todo - disable send button
            if (!e.HasFocus)
            {
                if (string.IsNullOrWhiteSpace(amountEdit.Text))
                {
                    amountLayout.ErrorEnabled = true;
                    amountLayout.Error = "Amount can't be blank!";
                }
            }
            else
            {
                amountLayout.Error = "";
                amountLayout.ErrorEnabled = false;
            }
        }
    }
}