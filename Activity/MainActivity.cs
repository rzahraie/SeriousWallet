using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using SeriousWallet3.Fragments;
using SeriousWallet3.Database;
using SeriousWallet3.DataStructures;
using Syncfusion.SfDataGrid;
using CoinDataModel;
using SyncFusionUtilities;
using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using ZXing.Mobile;

namespace SeriousWallet3
{
    enum ViewSelect { GRID_VIEW, CARD_VIEW };

    [Activity(Label = "@string/app_name", Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        Android.Support.V7.Widget.Toolbar mainToolBar;
        QuotePanelFragment quotePanelFragment;
        ViewSelect viewSelect = ViewSelect.CARD_VIEW;
        bool init = true;
        TextView totalValue;
        SfDataGrid dataGrid;
        DataRepository dataRepository;
        int selectedRowIndex = -1;
        string walletSelected;

        ExpandableListAdapter menuAdapter;
        List<ExpandedMenuModel> listDataHeader;
        Dictionary<ExpandedMenuModel, List<String>> listDataChild;
        Android.Support.V4.Widget.DrawerLayout drawerLayout;
        List<string> symbolDescriptionListCopy;

        ActionBarDrawerToggle actionBarDrawerToggle;

        //@@todo:accessors
        public static bool startUpByFile = false;
        public static SeriousWalletTransactionMessage seriousWalletTransactionMessage;
        public static bool setupError = false;
        public static string setupErrorText = "";
        static View rootView;

        FloatingActionButton floatingAddButton;

        public static void ReportTransactionSendReceiveResult(string msg)
        {
            Snackbar s = Snackbar.Make(rootView, msg,
                Snackbar.LengthLong).SetAction("Ok", (v) =>
                {
                    
                });

            s.Show();
        }

        private void PrintWalletInfo()
        {

            QRCodeWriter write = new QRCodeWriter();

            int size = 120;

            BitMatrix matrix = write.encode("", ZXing.BarcodeFormat.QR_CODE, size, size, null);
            BitmapRenderer bit = new BitmapRenderer();
            Android.Graphics.Bitmap bitmap = bit.Render(matrix, BarcodeFormat.QR_CODE, "");
        }

        public void SetTotalValue(decimal total)
        {
            totalValue.Text = total.ToString("C");
        }

        public void ShowCryptoCurrencyAddressFragment(string symbol)
        {

            Coin coin = DataLayer.GetCoinObject(symbol, walletSelected);

            CryptoCurrencyAddressPager cryptoCurrencyAddressPager = new
                CryptoCurrencyAddressPager(coin.PrivateKey, coin.PublicAddress);

            ShowSupportFragment(cryptoCurrencyAddressPager, symbol);
        }

        public void ShowCandleFragment(string symbol)
        {
            CandleChartFragment candleChartFragment = CandleChartFragment.NewInstance(symbol, "Bitcoin");

            ShowFragment(candleChartFragment, "Chart");
        }

        public void ShowSendFragment(string symbol)
        {
            Coin coin = DataLayer.GetCoinObject(symbol, walletSelected);

            //@@todo: update to date coin value
            SendFragment sendFragment = SendFragment.NewInstance(symbol, true, startUpByFile,
                7920.4m, ReportTransactionSendReceiveResult, coin.PrivateKey, walletSelected, 
                seriousWalletTransactionMessage);

            ShowFragment(sendFragment, "Send:" + symbol);
        }

        public void ShowReceiveFragment(string symbol)
        {
            Coin coin = DataLayer.GetCoinObject(symbol, walletSelected);

            //@@todo: update to date coin value
            SendFragment sendFragment = SendFragment.NewInstance(symbol, false, startUpByFile, 
                7920.4m, ReportTransactionSendReceiveResult,
                coin.PrivateKey, walletSelected,
                seriousWalletTransactionMessage);

            ShowFragment(sendFragment, "Receive:" + symbol);
        }

        public void ClearMenuBar()
        {
            mainToolBar.Menu.Clear();
        }

        public void ShowTransactionFragment(string symbol)
        {
            TransactionFragment transactionFragment = TransactionFragment.NewInstance(symbol);

            ShowFragment(transactionFragment, "Transaction");
        }

        public void CloseSendFragment(string addressToAdd)
        {
            HandleHomeButton();

            if (addressToAdd.Length == 0) return;

            Snackbar s = Snackbar.Make(Window.DecorView.RootView, "Request sent. Add " + addressToAdd + " to address book?",
                Snackbar.LengthLong).SetAction("Yes", (v) =>
            {
                //ShowAddressBookAddEntry();
            });

            s.Show();
        }

        public void CloseReceiveFragment(string addressToAdd)
        {
            HandleHomeButton();

            Snackbar s = Snackbar.Make(Window.DecorView.RootView, "Request sent. Add " + addressToAdd + " to address book?",
                Snackbar.LengthLong).SetAction("Yes", (v) =>
                {
                    //ShowAddressBookAddEntry();
                });

            s.Show();
        }

        public void ShowErrorFromFragment(string errorMessage)
        {
            Snackbar s = Snackbar.Make(Window.DecorView.RootView, errorMessage, Snackbar.LengthIndefinite).SetAction("Ok", (v) =>
            {
            });
            s.Show();
        }

        private void ShowHamurgerIcon()
        {
            if (actionBarDrawerToggle != null) actionBarDrawerToggle.DrawerIndicatorEnabled = true;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }

        private void ShowArrowIcon()
        {
            actionBarDrawerToggle.DrawerIndicatorEnabled = false;
        }

        private void HandleHomeButton()
        {
            if (actionBarDrawerToggle.DrawerIndicatorEnabled)
            {
                drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                return;
            }

            actionBarDrawerToggle.DrawerIndicatorEnabled = true;

            if (FragmentManager.BackStackEntryCount >= 1)
            {
                FragmentManager.PopBackStackImmediate();
                mainToolBar.Menu.RemoveItem(Resource.Id.menu_card_view);
                mainToolBar.Menu.RemoveItem(Resource.Id.menu_grid_view);
                mainToolBar.Menu.Clear();
                mainToolBar.InflateMenu(Resource.Menu.MainToolBarMenu);

                ShowHamurgerIcon();
                SupportActionBar.Title = Resources.GetString(Resource.String.app_name);

                floatingAddButton.Visibility = ViewStates.Visible;
            }
            else if (SupportFragmentManager.BackStackEntryCount >= 1)
            {
                SupportFragmentManager.PopBackStackImmediate();
                mainToolBar.Menu.RemoveItem(Resource.Id.menu_card_view);
                mainToolBar.Menu.RemoveItem(Resource.Id.menu_grid_view);
                mainToolBar.Menu.Clear();
                mainToolBar.InflateMenu(Resource.Menu.MainToolBarMenu);

                ShowHamurgerIcon();
                SupportActionBar.Title = Resources.GetString(Resource.String.app_name);

                floatingAddButton.Visibility = ViewStates.Visible;
            }
        }

        private void ShowSettingsFragment()
        {
            PrefFragment prefFragment = PrefFragment.newInstance();

            ShowSupportFragment(prefFragment, "Settings");
        }

        private void ShowSupportFragment(Android.Support.V4.App.Fragment fragment, string title)
        {
            if (fragment != null)
            {
                SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.mainFrameLayout, fragment)
                .AddToBackStack(null)
                .Commit();

                mainToolBar.Menu.Clear();
                
                ShowArrowIcon();
                SupportActionBar.Title = title;

                floatingAddButton.Visibility = ViewStates.Gone;
            }
        }

        private void ShowFragment(Fragment fragment, string title)
        {
            if (fragment != null)
            {
                FragmentManager.BeginTransaction()
                .Replace(Resource.Id.mainFrameLayout, fragment)
                .AddToBackStack(null)
                .Commit();

                mainToolBar.Menu.Clear();
                ShowArrowIcon();
                SupportActionBar.Title = title;

                floatingAddButton.Visibility = ViewStates.Gone;
            }
        }

        private void ShowMainViewFragment(Fragment fragment, int frameId)
        {
            if (fragment != null)
            {
                FragmentManager.BeginTransaction()
                .Replace(frameId, fragment)
                .Commit();
            }
        }

        //@@todo: Duplicate???
        private void CreateWalletTable(string currentWallet)
        {
            if (dataRepository == null) dataRepository = new DataRepository();
            DataGridUtilities.context = BaseContext;
            
            dataRepository.WalletDataCollection = DataLayer.GetWalletsEx();

            dataGrid = new SfDataGrid(BaseContext)
            {
                RowHeight = 30,
                HeaderRowHeight = 30,
                GridStyle = new SyncFusionUtilities.CustomGridStyle(),
                NestedScrollingEnabled = true,
                AutoGenerateColumns = false,
                ItemsSource = dataRepository.WalletDataCollection,
                SelectionMode = SelectionMode.Single
            };

            GridTextColumn walletColumn = DataGridUtilities.CreateTextColumn("WalletName", "Wallet", "", -1,
               false, GravityFlags.Start | GravityFlags.CenterVertical);

            WalletImageGridCell.checkMark = Resource.Drawable.ic_check_green_dark_18dp;
            WalletImageGridCell.currentWallet = currentWallet;

            walletColumn.UserCellType = typeof(WalletImageGridCell);
            walletColumn.LoadUIView = true;
            walletColumn.AllowSorting = false;

            dataGrid.Columns.Add(walletColumn);

            dataGrid.Columns.Add(DataGridUtilities.CreateTextColumn("WalletDescription", "Description", "", -1,
                false, GravityFlags.Start | GravityFlags.CenterVertical));  //65

            dataGrid.GridLoaded += DataGrid_GridLoaded;
            dataGrid.GridTapped += DataGrid_GridTapped;
        }

        private void DataGrid_GridTapped(object sender, GridTappedEventArgs e)
        {
            selectedRowIndex = e.RowColumnIndex.RowIndex;
            var rowData = dataGrid.GetRecordAtRowIndex(selectedRowIndex);
            walletSelected = dataGrid.GetCellValue(rowData, "WalletName").ToString();
        }

        private void DataGrid_GridLoaded(object sender, GridLoadedEventArgs e)
        {
            var per = (dataGrid.Width) / (Resources.DisplayMetrics.Density * 100);

            dataGrid.Columns[0].Width = per * 30;
            dataGrid.Columns[1].Width = per * 70;
        }

        private void SetWallet(string walletName = null)
        {
            if (walletName != null)
            {
                DataLayer.SetCurrentWallet(walletName);
                SupportActionBar.Subtitle = walletName;
            }
            else
            {
                walletSelected = DataLayer.GetCurrentWallet();
                SupportActionBar.Subtitle = walletSelected;
            }
            //@@todo - asset change
        }

        private void ShowWalletDialog()
        {
            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);

            LayoutInflater layoutInflater = LayoutInflater;

            View view = layoutInflater.Inflate(Resource.Layout.firsttimelaunch, null, false);
            RadioGroup rg = view.FindViewById<RadioGroup>(Resource.Id.firstTimeRadioGroup);
            rg.Visibility = ViewStates.Gone;

            FrameLayout frameLayout = view.FindViewById<FrameLayout>(Resource.Id.firstTimeFrameLayout);

            frameLayout.Visibility = ViewStates.Visible;

            TextView errorText = view.FindViewById<TextView>(Resource.Id.errorTextView);

            builder.SetTitle("Select a wallet");

            builder.SetView(view);

            CreateWalletTable(SupportActionBar.Subtitle);

            frameLayout.AddView(dataGrid);

            builder.SetPositiveButton("Ok", (System.EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton(Resource.String.cancel, (senderAlert, args) => {  });

            var dialog = builder.Create();

            dialog.Show();

            var nextBtn = dialog.GetButton((int)DialogButtonType.Positive);

            nextBtn.Click += (sender, args) =>
            {
                if (selectedRowIndex == -1)
                {
                    errorText.Visibility = ViewStates.Visible;
                    errorText.Text = "A wallet must be selected!";
                    return;
                }
                else
                {
                    SetWallet(walletSelected);

                    dialog.Dismiss();
                }
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
           MenuInflater.Inflate(Resource.Menu.MainToolBarMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        private void SetView(ViewSelect vs)
        {
            if ((viewSelect == vs) && (!init)) return;

            viewSelect = vs;

            init = false;

            switch (vs)
            {
                case ViewSelect.CARD_VIEW:
                    ShowMainViewFragment(quotePanelFragment = QuotePanelFragment.NewInstance(), Resource.Id.bottomFrameLayout);
                    break;

                case ViewSelect.GRID_VIEW:
                    ShowMainViewFragment(PortfolioDataGridFragment.NewInstance(), Resource.Id.bottomFrameLayout);
                    break;

                default:
                    break;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case (Resource.Id.menu_wallet):
                    ShowWalletDialog();
                    break;

                case (Resource.Id.menu_card_view):
                    SetView(ViewSelect.CARD_VIEW);
                    break;

                case (Resource.Id.menu_grid_view):
                    SetView(ViewSelect.GRID_VIEW);
                    break;

                case (Android.Resource.Id.Home):
                    HandleHomeButton();
                    break;

                default:
                    break;
            }

            return false;
        }

        private void PrepareListData()
        {
            listDataHeader = new List<ExpandedMenuModel>();
            listDataChild = new Dictionary<ExpandedMenuModel, List<String>>();

            ExpandedMenuModel item1 = new ExpandedMenuModel();
            item1.Name = "Settings";
            item1.Image = Resource.Drawable.ic_settings_grey_700_18dp;
            // Adding data header
            listDataHeader.Add(item1);

            // Adding child data
            List<String> heading1 = new List<String>();
            heading1.Add("Settings");
            heading1.Add("Timezone");
            heading1.Add("Currency");
            heading1.Add("Edit Profile");
            heading1.Add("Beneficiary(ies)");
            heading1.Add("Setup PIN");
            listDataChild.Add(listDataHeader[0], heading1);// Header, Child data

            ExpandedMenuModel item2 = new ExpandedMenuModel();
            item2.Name = "Tools";
            item2.Image = Resource.Drawable.ic_devices_grey_700_18dp;
            listDataHeader.Add(item2);

            List<String> heading2 = new List<String>();
            heading2.Add("Portfolio");
            heading2.Add("Share App with friends");
            heading2.Add("Invite friends");
            listDataChild.Add(listDataHeader[1], heading2);

            ExpandedMenuModel item3 = new ExpandedMenuModel();
            item3.Name = "Backup";
            item3.Image = Resource.Drawable.ic_backup_grey_700_18dp;
            listDataHeader.Add(item3);

            List<String> heading3 = new List<String>();
            heading3.Add("View Pockets");
            heading3.Add("View Wallets");
            heading3.Add("Send/Receive");
            heading3.Add("View private keys");
            heading3.Add("View QR Code");
            listDataChild.Add(listDataHeader[2], heading3);

            ExpandedMenuModel item4 = new ExpandedMenuModel();
            item4.Name = "Pockets/Wallets";
            item4.Image = Resource.Drawable.ic_shop_two_grey_700_18dp;
            listDataHeader.Add(item4);

            List<String> heading4 = new List<String>();
            heading4.Add("Paper Wallet Printout");
            heading4.Add("Email Wallet");
            heading4.Add("Backing Phrase");
            heading4.Add("Trusted 3rd Partys");
            heading4.Add("Serious Help");
            listDataChild.Add(listDataHeader[3], heading4);

            ExpandedMenuModel item5 = new ExpandedMenuModel();
            item5.Name = "Help";
            item5.Image = Resource.Drawable.ic_help_grey_700_18dp;
            listDataHeader.Add(item5);

            List<string> heading5 = new List<string>();
            heading5.Add("Support");
            heading5.Add("FAQ");
            listDataChild.Add(listDataHeader[4], heading5);

            ExpandedMenuModel item6 = new ExpandedMenuModel();
            item6.Name = "About";
            item6.Image = Resource.Drawable.ic_info_grey_700_18dp;
            listDataHeader.Add(item6);

            List<string> heading6 = new List<string>();
            heading6.Add("Website");
            heading6.Add("Version");
            heading6.Add("Support");
            listDataChild.Add(listDataHeader[5], heading6);
        }

        private void CreateHamburgerMenu()
        {
            drawerLayout = FindViewById<Android.Support.V4.Widget.DrawerLayout>(Resource.Id.drawer_layout);
            actionBarDrawerToggle = new ActionBarDrawerToggle(this, drawerLayout,
               Resource.String.NavigationDrawerOpen, Resource.String.NavigationDrawerClose);

            actionBarDrawerToggle.SyncState();

            ExpandableListView expandableList = FindViewById<ExpandableListView>(Resource.Id.navigationmenu);
            //setup navigation view
            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            navigationView.NavigationItemSelected += (sender, e) =>
            {
                var menuItem = e.MenuItem;

                int id = menuItem.ItemId;

                menuItem.SetChecked(!menuItem.IsChecked);
                drawerLayout.CloseDrawers();
            };

            PrepareListData();

            menuAdapter = new ExpandableListAdapter(this, listDataHeader, listDataChild, expandableList);

            expandableList.SetAdapter(menuAdapter);

            expandableList.ChildClick += delegate (object sender, ExpandableListView.ChildClickEventArgs e)
            {
                drawerLayout.CloseDrawers();

                List<string> ls = listDataChild[listDataHeader[e.GroupPosition]];

                string menuClicked = ls[(int)e.Id];

                if (menuClicked == "Settings")
                {
                    ShowSettingsFragment();
                }
                else if (menuClicked == "Edit Profile")
                {
                    Dialogs.UserAlertDialog.ShowDialog(this, LayoutInflater);
                }
                else if (menuClicked == "Beneficiary(ies)")
                {
                    SignatureRecoveryActivity.FirstTimeUse = false;
                    SignatureRecoveryActivity.WalletName = walletSelected;

                    StartActivity(new Intent(Application.Context, typeof(SignatureRecoveryActivity)));
                }
                else if (menuClicked == "Support")
                {
                    ShowFragment(CryptoCurrencyAddressFragment.NewInstance(), "Test");
                }
                else
                {
                    Toast.MakeText(this, "child clicked : " + ls[(int)e.Id], ToastLength.Short).Show();
                }
            };
        }

        private void CreateAddCardFloatingButton()
        {
            floatingAddButton = FindViewById<FloatingActionButton>(Resource.Id.floatingAddButton);

            floatingAddButton.Click += (sender, e) =>
            {
                var builder = new Android.Support.V7.App.AlertDialog.Builder(this);

                LayoutInflater layoutInflater = this.LayoutInflater;

                View view = layoutInflater.Inflate(Resource.Layout.addassetlayout, null, false);

                Spinner sp = view.FindViewById<Spinner>(Resource.Id.currencySpinner);

                List<Coin> coins = DataLayer.GetCoinObjects(walletSelected);

                foreach(Coin c in coins)
                {
                    int i = symbolDescriptionListCopy.RemoveAll((x) => x.Contains(c.Symbol));
                }

                var adapter = new ArrayAdapter(builder.Context, Android.Resource.Layout.SimpleSpinnerItem,
                    symbolDescriptionListCopy);

                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                sp.Adapter = adapter;

                sp.SetSelection(0);

                TextInputLayout privateAddressInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.privateAddressLayout);
                TextInputEditText privateAddressInputEdit = view.FindViewById<TextInputEditText>(Resource.Id.privateAddressEdit);

                TextInputLayout publicAddressInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.publicAddressLayout);
                TextInputEditText publicAddressInputEdit = view.FindViewById<TextInputEditText>(Resource.Id.publicAddressEdit);

                LinearLayout privateAddressLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.privateAddressButtonLayout);
                LinearLayout publicAddressLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.publicAddressButtonLayout);

                //Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref privateAddressInputLayout, ref privateAddressInputEdit, 
                //    Resource.Id.privateAddressLayout, Resource.Id.privateAddressEdit, "Private address can't be blank!", privateAddressLinearLayout);

                //Utilities.AndroidUtility.CreateTextLayoutEdit(view, ref publicAddressInputLayout, ref publicAddressInputEdit,
                //    Resource.Id.publicAddressLayout, Resource.Id.publicAddressEdit, "Public address can't be blank!", publicAddressLinearLayout);

                privateAddressInputEdit.FocusChange += (s, ee) =>
                {
                    if (!ee.HasFocus)
                    {
                        privateAddressLinearLayout.Visibility = ViewStates.Gone;

                        if (string.IsNullOrWhiteSpace(privateAddressInputEdit.Text))
                        {
                            privateAddressInputLayout.ErrorEnabled = true;
                            privateAddressInputLayout.Error = "Private address can't be blank!";
                        }
                    }
                    else
                    {
                        privateAddressLinearLayout.Visibility = ViewStates.Visible;

                        privateAddressInputLayout.Error = "";
                        privateAddressInputLayout.ErrorEnabled = false;
                    }
                };

                publicAddressInputEdit.FocusChange += (s, ee) =>
                {
                    if (!ee.HasFocus)
                    {
                        publicAddressLinearLayout.Visibility = ViewStates.Gone;

                        if (string.IsNullOrWhiteSpace(publicAddressInputEdit.Text))
                        {
                            publicAddressInputLayout.ErrorEnabled = true;
                            publicAddressInputLayout.Error = "Public address can't be blank!";
                        }
                    }
                    else
                    {
                        publicAddressLinearLayout.Visibility = ViewStates.Visible;

                        publicAddressInputLayout.Error = "";
                        publicAddressInputLayout.ErrorEnabled = false;
                    }
                };

                Button privateAddressQrButton = view.FindViewById<Button>(Resource.Id.qrCodePrivateAddressButton);
                Button privateAddressClearButton = view.FindViewById<Button>(Resource.Id.clearPrivateAddressButton);

                Button publicAddressQrButton = view.FindViewById<Button>(Resource.Id.qrCodePublicAddressButton);
                Button publicAddressClearButton = view.FindViewById<Button>(Resource.Id.clearPublicAddressButton);

                privateAddressQrButton.Click += async (s, ee) =>
                {
                    privateAddressInputEdit.Text = await Utilities.AndroidUtility.ReadQRCode(Application);
                };

                privateAddressClearButton.Click += (s, ee) =>
                {
                    privateAddressInputEdit.Text = "";
                };

                publicAddressQrButton.Click += async (s, ee) =>
                {
                    publicAddressInputEdit.Text = await Utilities.AndroidUtility.ReadQRCode(Application);
                };

                publicAddressClearButton.Click += (s, ee) =>
                {
                    publicAddressInputEdit.Text = "";
                };

                builder.SetTitle("Add coin tile");

                builder.SetView(view);

                builder.SetPositiveButton("Ok", (System.EventHandler<DialogClickEventArgs>)null);
                builder.SetNegativeButton(Resource.String.cancel, (senderAlert, args) => { });

                var dialog = builder.Create();

                dialog.Show();

                var okBtn = dialog.GetButton((int)DialogButtonType.Positive);

                okBtn.Click += (s, args) =>
                {
                    if ((string.IsNullOrWhiteSpace(privateAddressInputEdit.Text)) || (string.IsNullOrWhiteSpace(publicAddressInputEdit.Text)))
                    {

                    }
                    else
                    {
                        int i = sp.SelectedItemPosition;
                        string text = sp.SelectedItem.ToString();
                        string[] strings = text.Split('-');
                        string symbol = strings[0].Trim();
                        string description = strings[1].Trim();

                        quotePanelFragment.AddCard(symbol, description, privateAddressInputEdit.Text,
                            publicAddressInputEdit.Text, walletSelected);

                        if (i >= 0) symbolDescriptionListCopy.RemoveAt(i);

                        dialog.Dismiss();
                    }
                };
            };
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Set your main view here
            SetContentView(Resource.Layout.main);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                ShowHamurgerIcon();
                //Set hamburger items menu
                SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white_18dp);
                SupportActionBar.Subtitle = walletSelected;
            }

            CreateHamburgerMenu();

            totalValue = FindViewById<TextView>(Resource.Id.textView7);

            mainToolBar = toolbar;

            PortfolioChartsFragment chartsFragment = PortfolioChartsFragment.NewInstance();

            ShowMainViewFragment(chartsFragment, Resource.Id.middleFrameLayout);

            SetView(viewSelect);

            SetWallet();

            symbolDescriptionListCopy = CryptoClasses.SymbolDescriptionList;

            CreateAddCardFloatingButton();

            rootView = Window.DecorView.RootView;

            if (startUpByFile)
            {
                if (seriousWalletTransactionMessage == null)
                {
                    ShowAlertDialog("Corrupt send message!");
                    return;
                }

                ShowSendFragment(seriousWalletTransactionMessage.CoinType);
            }
        }

        private void FloatingAddButton_Click(object sender, EventArgs e)
        {
            ShowAddAssetOrPanelDialog();
        }

        private void ShowAlertDialog(string message)
        {
            Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this);
            alertDialog.SetPositiveButton("Ok", (senderAlert, args) => { });
            alertDialog.SetMessage(message);

            alertDialog.Show();   
        }

        private void ShowAddAssetOrPanelDialogEx()
        {
            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);

            LayoutInflater layoutInflater = LayoutInflater;

            View view = layoutInflater.Inflate(Resource.Layout.addassetlayout, null, false);

            builder.SetTitle("Add coin tile");

            builder.SetView(view);

            builder.SetPositiveButton("Ok", (System.EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton(Resource.String.cancel, (senderAlert, args) => { });

            var dialog = builder.Create();

            dialog.Show();

            var okBtn = dialog.GetButton((int)DialogButtonType.Positive);

            okBtn.Click += (sender, args) =>
            {

            };
        }

        private void ShowAddAssetOrPanelDialog()
        {
            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);

            LayoutInflater layoutInflater = this.LayoutInflater;

            View view = layoutInflater.Inflate(Resource.Layout.addassetlayout, null, false);

            Spinner sp = view.FindViewById<Spinner>(Resource.Id.currencySpinner);

            var adapter = new ArrayAdapter(builder.Context, Android.Resource.Layout.SimpleSpinnerItem,
                symbolDescriptionListCopy);

            //var adapter = ArrayAdapter.CreateFromResource(
            //      builder.Context, Resource.Array.Currency_Array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            sp.Adapter = adapter;

            sp.SetSelection(0);

            TextInputLayout privateAddressInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.privateAddressLayout);
            TextInputEditText privateAddressInputEdit = view.FindViewById<TextInputEditText>(Resource.Id.privateAddressEdit);

            TextInputLayout publicAddressInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.publicAddressLayout);
            TextInputEditText publicAddressInputEdit = view.FindViewById<TextInputEditText>(Resource.Id.publicAddressEdit);

            Button privateAddressQrButton = view.FindViewById<Button>(Resource.Id.qrCodePrivateAddressButton);
            Button privateAddressClearButton = view.FindViewById<Button>(Resource.Id.clearPrivateAddressButton);

            Button publicAddressQrButton = view.FindViewById<Button>(Resource.Id.qrCodePublicAddressButton);
            Button publicAddressClearButton = view.FindViewById<Button>(Resource.Id.clearPublicAddressButton);

            LinearLayout privateAddressLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.privateAddressButtonLayout);
            LinearLayout publicAddressLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.publicAddressButtonLayout);

            privateAddressQrButton.Click += async (sender, e) =>
            {
                privateAddressInputEdit.Text = await Utilities.AndroidUtility.ReadQRCode(Application);
            };

            privateAddressClearButton.Click += (sender, e) =>
            {
                privateAddressInputEdit.Text = "";
            };

            publicAddressQrButton.Click += async (sender, e) =>
            {
                publicAddressInputEdit.Text = await Utilities.AndroidUtility.ReadQRCode(Application);
            };

            publicAddressClearButton.Click += (sender, e) =>
            {
                publicAddressInputEdit.Text = "";
            };

            privateAddressInputEdit.FocusChange += (sender, e) =>
            {
                if (!e.HasFocus)
                {
                    privateAddressLinearLayout.Visibility = ViewStates.Gone;

                    if (string.IsNullOrWhiteSpace(privateAddressInputEdit.Text))
                    {
                        privateAddressInputLayout.ErrorEnabled = true;
                        privateAddressInputLayout.Error = "Private address can't be blank!";
                    }
                }
                else
                {
                    privateAddressLinearLayout.Visibility = ViewStates.Visible;

                    privateAddressInputLayout.Error = "";
                    privateAddressInputLayout.ErrorEnabled = false;
                }
            };

            publicAddressInputEdit.FocusChange += (sender, e) =>
            {
                if (!e.HasFocus)
                {
                    publicAddressLinearLayout.Visibility = ViewStates.Gone;

                    if (string.IsNullOrWhiteSpace(publicAddressInputEdit.Text))
                    {
                        publicAddressInputLayout.ErrorEnabled = true;
                        publicAddressInputLayout.Error = "Public address can't be blank!";
                    }
                }
                else
                {
                    publicAddressLinearLayout.Visibility = ViewStates.Visible;

                    publicAddressInputLayout.Error = "";
                    publicAddressInputLayout.ErrorEnabled = false;
                }
            };

            builder.SetTitle("Add coin tile");

            builder.SetView(view);

            builder.SetPositiveButton("Ok", (System.EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton(Resource.String.cancel, (senderAlert, args) => { });

            var dialog = builder.Create();

            dialog.Show();

            var okBtn = dialog.GetButton((int)DialogButtonType.Positive);

            okBtn.Click += (sender, args) =>
            {
                if ((string.IsNullOrWhiteSpace(privateAddressInputEdit.Text)) || (string.IsNullOrWhiteSpace(publicAddressInputEdit.Text)))
                {

                }
                else
                {
                    //Coin coin = new Coin();

                    int i = sp.SelectedItemPosition;
                    string text = sp.SelectedItem.ToString();
                    string[] strings = text.Split('-');
                    string symbol = strings[0].Trim();
                    string description = strings[1].Trim();

                    //@@todo add cardview
                    quotePanelFragment.AddCard(symbol, description, privateAddressInputEdit.Text, 
                        publicAddressInputEdit.Text, walletSelected);

                    symbolDescriptionListCopy.RemoveAt(i);

                    dialog.Dismiss();
                }
            };
          }
    }
}

