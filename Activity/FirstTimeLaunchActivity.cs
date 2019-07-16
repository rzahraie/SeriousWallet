using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Syncfusion.SfDataGrid;
using SyncFusionUtilities;
using CoinDataModel;

namespace SeriousWallet3
{
    //class WalletSelectDialog
    //{
    //    SfDataGrid dataGrid;
    //    DataRepository dataRepository;
    //    int selectedRowIndex = -1;
    //    float density;

    //    public SfDataGrid CreateDataGrid(Context context, float density)
    //    {
    //        this.density = density;

    //        dataGrid = new SfDataGrid(context)
    //        {
    //            RowHeight = 30,
    //            HeaderRowHeight = 30,
    //            GridStyle = new SyncFusionUtilities.CustomGridStyle(),
    //            NestedScrollingEnabled = true,
    //            AutoGenerateColumns = false,
    //            ItemsSource = dataRepository.WalletDataCollection,
    //            SelectionMode = SelectionMode.Single
    //        };

    //        dataGrid.GridLoaded += DataGrid_GridLoaded;

    //        dataGrid.Columns.Add(DataGridUtilities.CreateTextColumn("WalletName", "Wallet", "", -1,
    //           false, GravityFlags.Start | GravityFlags.CenterVertical));  //57
    //        dataGrid.Columns.Add(DataGridUtilities.CreateTextColumn("WalletAddress", "Address", "", -1,
    //            false, GravityFlags.Start | GravityFlags.CenterVertical));  //65

    //        return dataGrid;
    //    }

    //    private void DataGrid_GridLoaded(object sender, GridLoadedEventArgs e)
    //    {
    //        var per = (dataGrid.Width) / (density * 100);

    //        dataGrid.Columns[0].Width = per * 30;
    //        dataGrid.Columns[1].Width = per * 70;
    //    }
    //}

    [Activity(Label = "@string/app_name")]
    public class FirstTimeLaunchActivity : AppCompatActivity
    {
        SfDataGrid dataGrid;
        DataRepository dataRepository;
        int selectedRowIndex = -1;
        FrameLayout frameLayout;
        TextView errorText;
        string walletSelected;

        //@@todo: Duplicate
        private void CreateDataTable()
        {
            if (dataRepository == null) dataRepository = new DataRepository();
            DataGridUtilities.context = BaseContext;

            dataRepository.WalletDataCollection = Database.DataLayer.GetWalletsEx();

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

            dataGrid.Columns.Add(DataGridUtilities.CreateTextColumn("WalletName", "Wallet", "", -1,
               false, GravityFlags.Start | GravityFlags.CenterVertical));  //57
            dataGrid.Columns.Add(DataGridUtilities.CreateTextColumn("WalletAddress", "Address", "", -1,
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

        private void ShowFrameLayout(ViewStates vs)
        {
            frameLayout.Visibility = vs;
            errorText.Visibility = vs;
            errorText.Text = "";
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.firsttimemain);

            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);

            Android.Views.LayoutInflater layoutInflater = this.LayoutInflater;

            Android.Views.View view = layoutInflater.Inflate(Resource.Layout.firsttimelaunch, null, false);

            RadioGroup rg = view.FindViewById<RadioGroup>(Resource.Id.firstTimeRadioGroup);
            frameLayout = view.FindViewById<FrameLayout>(Resource.Id.firstTimeFrameLayout);
            errorText = view.FindViewById<TextView>(Resource.Id.errorTextView);

            rg.CheckedChange += (s, e) =>
            {
                switch (e.CheckedId)
                {
                    case (Resource.Id.newWalletButton):
                        ShowFrameLayout(ViewStates.Gone);
                        break;

                    case (Resource.Id.recoverWalletButton):
                        ShowFrameLayout(ViewStates.Visible);
                        break;

                    default:
                        break;
                }
            };

            builder.SetTitle("No Wallet Found");

            builder.SetView(view);

            CreateDataTable();

            frameLayout.AddView(dataGrid);

            builder.SetPositiveButton(Resource.String.next, (System.EventHandler <DialogClickEventArgs>) null);
            builder.SetNegativeButton(Resource.String.cancel, (senderAlert, args) => { Finish(); });
            builder.SetNeutralButton(Resource.String.prev, (senderAlert, args) => { StartActivity(new Intent(Application.Context, 
                typeof(AccountSetupActivity))); });

            var dialog = builder.Create();

            dialog.Show();

            var nextBtn = dialog.GetButton((int)DialogButtonType.Positive);

            nextBtn.Click += (sender, args) =>
            {
                if ((selectedRowIndex == -1) && (frameLayout.Visibility == ViewStates.Visible))
                {
                    errorText.Visibility = ViewStates.Visible;
                    errorText.Text = "A wallet must be selected!";
                    return;
                }
                else if ((selectedRowIndex != -1) && (frameLayout.Visibility == ViewStates.Visible))
                {
                    //@@todo: write current wallet to database
                    StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                    return;
                }

                StartActivity(new Intent(Application.Context, typeof(WalletSetupActivity)));
            };
        }
    }
}