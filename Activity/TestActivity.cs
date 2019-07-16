using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using SeriousWallet3.Fragments;
//using SeriousWallet3.Database;
//using SeriousWallet3.DataStructures;
using Syncfusion.SfDataGrid;
using CoinDataModel;
using SyncFusionUtilities;
//using ZXing;
//using ZXing.QrCode;
//using ZXing.Common;
//using ZXing.Mobile;

namespace SeriousWallet3
{
    [Activity(Label = "TestActivity")]
    public class TestActivity : AppCompatActivity
    {
        ExpandableListAdapter menuAdapter;
        ActionBarDrawerToggle actionBarDrawerToggle;
        Android.Support.V4.Widget.DrawerLayout drawerLayout;
        List<ExpandedMenuModel> listDataHeader;
        Dictionary<ExpandedMenuModel, List<String>> listDataChild;
        Android.Support.V7.Widget.Toolbar mainToolBar;
        TextView totalValue;
        FloatingActionButton floatingAddButton;

        private void ShowArrowIcon()
        {
            actionBarDrawerToggle.DrawerIndicatorEnabled = false;
        }

        public void SetTotalValue(decimal total)
        {
            totalValue.Text = total.ToString("C");
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

        private void ShowHamurgerIcon()
        {
            if (actionBarDrawerToggle != null) actionBarDrawerToggle.DrawerIndicatorEnabled = true;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
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
                    
                }
                else if (menuClicked == "Edit Profile")
                {
                    Dialogs.UserAlertDialog.ShowDialog(this, LayoutInflater);
                }
                else if (menuClicked == "Beneficiary(ies)")
                {
                    SignatureRecoveryActivity.FirstTimeUse = false;
                    SignatureRecoveryActivity.WalletName = "";

                    StartActivity(new Intent(Application.Context, typeof(SignatureRecoveryActivity)));
                }
                else
                {
                    Toast.MakeText(this, "child clicked : " + ls[(int)e.Id], ToastLength.Short).Show();
                }
            };
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.main);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                ShowHamurgerIcon();
                //Set hamburger items menu
                SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white_18dp);
                SupportActionBar.Subtitle = "TEST";
            }

            totalValue = FindViewById<TextView>(Resource.Id.textView7);

            mainToolBar = toolbar;

            // Create your application here
            floatingAddButton = FindViewById<FloatingActionButton>(Resource.Id.floatingAddButton);

            floatingAddButton.Click += (sender, e) =>
            {
                Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this);
                alertDialog.SetPositiveButton("Ok", (senderAlert, args) => { });
                alertDialog.SetMessage("test");

                alertDialog.Show();
            };

            ShowMainViewFragment(QuotePanelFragment.NewInstance(), Resource.Id.bottomFrameLayout);

            //PortfolioChartsFragment chartsFragment = PortfolioChartsFragment.NewInstance();

            //ShowMainViewFragment(chartsFragment, Resource.Id.middleFrameLayout);
        }
    }
}