using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using CoinDataModel;
using Syncfusion.SfDataGrid;
using Com.Syncfusion.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SeriousWallet3.AddressBookInfo;

namespace SeriousWallet3.Fragments
{
    public class TransactionFragment : Fragment
    {
        TextInputLayout btcAddressLayout;
        TextInputLayout labelLayout;
        TextInputLayout amountLayout;

        //@@todo change names for consistency
        TextInputEditText labelReceiveEditText;
        TextInputEditText amountReceiveEdit;
        TextInputEditText messageReceiveEdit;

        AutoCompleteTextView bitCoinAddressView;
        TextInputEditText labelTextView;
        TextInputEditText amountTextView;

        AddressBookAdapter addressBookAdapter;

        LinearLayout buttonLayout;

        Button addressBookButton;
        Button qrCodeButton;
        Button clearButton;

        SearchView filterText;
        SfDataGrid dataGrid;
        DataRepository dataRepository;
        int selectedRowIndex;
        string symbol;
        int prevSelectedRecyclerItem;

        Android.Support.V7.Widget.RecyclerView recyclerView;

        LinearLayout mainLayout;

        View mainView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            SetHasOptionsMenu(true);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            var activity = (MainActivity)this.Activity;
            activity.ClearMenuBar();
            inflater.Inflate(Resource.Menu.TransactionToolBarMenu, menu);
        }

        private void ShowReceiveDilaog()
        {
            var builder = new Android.Support.V7.App.AlertDialog.Builder(Context);

            builder.SetTitle("Receive");

            LayoutInflater layoutInflater = Activity.LayoutInflater;

            View view = layoutInflater.Inflate(Resource.Layout.receivelayout, null, false);

            labelReceiveEditText = view.FindViewById<TextInputEditText>(Resource.Id.labelReceiveEdit);
            amountReceiveEdit = view.FindViewById<TextInputEditText>(Resource.Id.amountReceiveEdit);
            messageReceiveEdit = view.FindViewById<TextInputEditText>(Resource.Id.messageReceiveEdit);
            

            builder.SetView(view);

            //@@todo - blank error hints
            builder.SetPositiveButton(Resource.String.receive, (senderAlert, args) =>
            {
                if ((labelReceiveEditText.Text.Length != 0) && (messageReceiveEdit.Text.Length != 0)
                   && (amountReceiveEdit.Text.Length != 0))
                {
                    dataRepository.AddTransactionData(DateTime.Now, labelReceiveEditText.Text, labelReceiveEditText.Text, Convert.ToDecimal(amountReceiveEdit.Text),
                        GetValue(amountReceiveEdit.Text), "Received");
                }
            });

            builder.SetNegativeButton(Resource.String.cancel, (senderAlert, args) => { });

            builder.Show();
        }

        private void ShowSendDialog()
        {
            var builder = new Android.Support.V7.App.AlertDialog.Builder(Context);

            builder.SetTitle("Send");

            LayoutInflater layoutInflater = Activity.LayoutInflater;

            View view = layoutInflater.Inflate(Resource.Layout.sendlayout, null,false);

            mainLayout = view.FindViewById<LinearLayout>(Resource.Id.mainSendLayout);

            List<AddressBook> addressBookList = new List<AddressBook>();

            btcAddressLayout = view.FindViewById<Android.Support.Design.Widget.TextInputLayout>(
                 Resource.Id.username_layout);
            labelLayout = view.FindViewById<Android.Support.Design.Widget.TextInputLayout>(
                Resource.Id.labelLayout);
            amountLayout = view.FindViewById<Android.Support.Design.Widget.TextInputLayout>(
                Resource.Id.amountLayout);
            recyclerView = view.FindViewById<Android.Support.V7.Widget.RecyclerView>(Resource.Id.recyclerView);

            addressBookAdapter = new AddressBookAdapter(addressBookList);

            bitCoinAddressView = view.FindViewById<AutoCompleteTextView>(Resource.Id.autoComplete);
            labelTextView = view.FindViewById<TextInputEditText>(Resource.Id.labelEdit);
            amountTextView = view.FindViewById<TextInputEditText>(Resource.Id.amountEdit);

            buttonLayout = view.FindViewById<LinearLayout>(Resource.Id.buttonLayout);

            addressBookButton = view.FindViewById<Button>(Resource.Id.addressBook);
            qrCodeButton = view.FindViewById<Button>(Resource.Id.qrCode);
            clearButton = view.FindViewById<Button>(Resource.Id.clear);

            Android.Support.V7.Widget.DividerItemDecoration dividerItemDecoration = new
                Android.Support.V7.Widget.DividerItemDecoration(recyclerView.Context, Android.Support.V7.Widget.LinearLayoutManager.Vertical);

            recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(builder.Context));
            recyclerView.SetAdapter(addressBookAdapter);
            recyclerView.AddItemDecoration(dividerItemDecoration);
            //recyclerView.AddOnItemTouchListener(new RecyclerViewOnItemTouchListener(Context, recyclerView, new ClickListener(Context)));
            
            addressBookList.Add(new AddressBook("Steven Howchenhowser", "showchenhowdress@gmail.com", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"));
            addressBookList.Add(new AddressBook("Taun Olson", "taun@seriouscoin.io", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"));
            addressBookList.Add(new AddressBook("Todd Hoffman", "toddh@seriouscoin.io", "PKM90iag9jJgTHD1VXjvLCEnZuQ3rTCF0Q"));
            addressBookList.Add(new AddressBook("Todd Swasinger", "todds@seriouscoin.io", "20M90iag9jJgTHD1VXjvLCEnZuQ3rTCF0Q"));
            addressBookList.Add(new AddressBook("Ramin Zahraie", "ramin@seriouscoin.io", "58M90iag9jJgTHD1VXjvLCEnZuQ3rTCH1X"));
            addressBookList.Add(new AddressBook("Steve Smith", "ssmith@gmail.com", "58M90iag9jJgTHD1VXjvLCEnZuQ3rTCH2H"));
            addressBookList.Add(new AddressBook("William Johnson", "wjohnson@gmail.com", "58M90iag9jJgTHD1VXjvLCEnZuQ3rTCH27T"));


            addressBookAdapter.NotifyDataSetChanged();

            //@@address book
           

            addressBookButton.Click += AddressBookButton_Click;
            clearButton.Click += ClearButton_Click;

            List<string> addressList = new List<string>()
            {
                "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L",
                "1NU19iag9jJgTHD1VXjvLCEnZuQ3rKDE9L",
                "PKM90iag9jJgTHD1VXjvLCEnZuQ3rTCF0Q",
                "20M90iag9jJgTHD1VXjvLCEnZuQ3rTCF0Q",
                "58M90iag9jJgTHD1VXjvLCEnZuQ3rTCH1X"
            };

            ArrayAdapter<String> addressListDataAdapters = new ArrayAdapter<String>(Context,
               Android.Resource.Layout.SimpleListItem1, addressList);

            //bitCoinAddressView.Adapter = addressListDataAdapters;
            

            bitCoinAddressView.FocusChange += BitCoinAddressView_FocusChange;
            //bitCoinAddressView.AddTextChangedListener(new TextWatcher(ref addressBookAdapter));
            bitCoinAddressView.AfterTextChanged += BitCoinAddressView_AfterTextChanged;
            bitCoinAddressView.TextChanged += BitCoinAddressView_TextChanged;
            labelTextView.FocusChange += LabelTextView_FocusChange;
            amountTextView.FocusChange += AmountTextView_FocusChange;

            builder.SetView(view);

            builder.SetPositiveButton(Resource.String.send, (senderAlert, args) => 
            {
                if ((bitCoinAddressView.Text.Length != 0) && (labelTextView.Text.Length != 0) 
                    && (amountTextView.Text.Length != 0))
                {
                    dataRepository.AddTransactionData(DateTime.Now, bitCoinAddressView.Text, labelTextView.Text, Convert.ToDecimal(amountTextView.Text),
                        GetValue(amountTextView.Text), "Sent to");

                    Snackbar s = Snackbar.Make(mainView, "Request sent. Add " + bitCoinAddressView.Text + " to address book?", Snackbar.LengthLong).SetAction("Yes", (v) =>
                    {
                        ShowAddressBookAddEntry();
                    });

                    s.Show();
                }
            });

            builder.SetNegativeButton(Resource.String.cancel, (senderAlert, args) => { });

            builder.Show();
        }

        //@@todo
        private void ShowAddressBookAddEntry()
        {
            var builder = new Android.Support.V7.App.AlertDialog.Builder(Context);

            builder.SetTitle("Add address");

            builder.SetPositiveButton(Resource.String.add, (senderAlert, args) =>
            {
            });

            builder.SetNegativeButton(Resource.String.cancel, (senderAlert, args) => { });

            builder.Show();

        }

        private void AddressBookButton_Click(object sender, EventArgs e)
        {
            recyclerView.Visibility = (recyclerView.Visibility != ViewStates.Visible ? ViewStates.Visible : ViewStates.Gone);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            bitCoinAddressView.Text = "";
        }

        private void BitCoinAddressView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            addressBookAdapter.Filter(e.Text.ToString());
        }

        private void BitCoinAddressView_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
           
        }

        private void RecyclerView_InterceptTouchEvent(object sender, Android.Support.V7.Widget.RecyclerView.InterceptTouchEventEventArgs e)
        {
            if (e.Event.Action != MotionEventActions.Down) return;

            (e.Rv.FindViewHolderForAdapterPosition(prevSelectedRecyclerItem) as 
                AdapterBookViewHolder).ItemLayout.SetBackgroundColor(Android.Graphics.Color.Transparent);

            View view = e.Rv.FindChildViewUnder(e.Event.GetX(), e.Event.GetY());
            int position = e.Rv.GetChildAdapterPosition(view);
            var adapter = e.Rv.GetAdapter();
            var holder = e.Rv.FindViewHolderForAdapterPosition(position);

            string txt = (holder as AdapterBookViewHolder).CryptoAddressView.Text;
            (holder as AdapterBookViewHolder).ItemLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#d5d5d5"));

            Toast.MakeText(Context, "Address selected: " + txt, ToastLength.Short).Show();

            prevSelectedRecyclerItem = position;
        }

        //@@todo
        private decimal GetValue(string coins)
        {
            return (Convert.ToDecimal(coins) * 11153m);
        }
        //@@todo
        private void CheckInput(string text)
        {

        }

        //@@todo -> make lambda
        private void AmountTextView_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            //@@todo - disable send button
            if (!e.HasFocus)
            {
                if (string.IsNullOrWhiteSpace(amountTextView.Text))
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

        //@@todo -> make lambda
        private void LabelTextView_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                if (string.IsNullOrWhiteSpace(labelTextView.Text))
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

        //@@todo -> make lambda
        private void BitCoinAddressView_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                if (string.IsNullOrWhiteSpace(bitCoinAddressView.Text))
                {
                    btcAddressLayout.ErrorEnabled = true;
                    btcAddressLayout.Error = "Address can't be blank!";
                }

                buttonLayout.Visibility = ViewStates.Gone;
                recyclerView.Visibility = ViewStates.Gone;
            }
            else
            {
                btcAddressLayout.Error = "";
                btcAddressLayout.ErrorEnabled = false;

                buttonLayout.Visibility = ViewStates.Visible;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case (Resource.Id.menu_send):
                    ShowSendDialog();
                    break;

                case (Resource.Id.menu_receive):
                    ShowReceiveDilaog();
                    break;

                default:
                    break;
            }

            return true;
        }

        public static TransactionFragment NewInstance(string symbol)
        {
            var transFrag = new TransactionFragment { Arguments = new Bundle() };
            transFrag.symbol = symbol;
            return transFrag;
        }

        public List<Color> GetCustomColors(ObservableCollection<ChartDataModel> cd)
        {
            List<Color> colors = new List<Color>();

            foreach(ChartDataModel cdm in cd)
            {
                colors.Add(cdm.Value > 0 ? Color.Green : Color.Red);
            }

            return colors;
        }

        private void CreateTransactionChart(View view)
        {
            ImageButton chartButton = view.FindViewById<ImageButton>(Resource.Id.transactionGraphButton);

            FrameLayout chartFrameLayout = view.FindViewById<FrameLayout>(Resource.Id.transactionsChartFrameLayout);

            chartButton.Tag = new Identifier("Down");
            chartFrameLayout.Visibility = ViewStates.Gone;

            chartButton.Click += (sender, e) =>
            {
                string tag = ((Identifier)chartButton.Tag).Name;

                chartFrameLayout.Visibility = ViewStates.Gone;

                if (tag == "Down")
                {
                    ((Identifier)chartButton.Tag).Name = "Up";
                    chartFrameLayout.Visibility = ViewStates.Visible;
                    chartButton.SetBackgroundResource(Resource.Drawable.ic_keyboard_arrow_up_black_18dp);
                }
                else
                {
                    ((Identifier)chartButton.Tag).Name = "Down";
                    chartFrameLayout.Visibility = ViewStates.Gone;
                    chartButton.SetBackgroundResource(Resource.Drawable.ic_keyboard_arrow_down_black_18dp);
                }
            };

            SfChart columnChart = new SfChart(chartFrameLayout.Context);
            columnChart.PrimaryAxis = new CategoryAxis();
            columnChart.SecondaryAxis = new NumericalAxis();

            columnChart.Legend.ToggleSeriesVisibility = true;
            columnChart.Legend.DockPosition = ChartDock.Bottom;

            columnChart.LayoutParameters = new
                FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

            columnChart.Series.Clear();

            columnChart.Title.Text = "Transactions";
            columnChart.SecondaryAxis.Title.Text = "Coins";

            ObservableCollection<ChartDataModel>  cd = Database.DataLayer.GetTransactionChartData();

            ColumnSeries columnSeries = new ColumnSeries()
            {
                ItemsSource = cd,
                XBindingPath = "Name",
                YBindingPath = "Value",
                EnableAnimation = true
            };

            columnSeries.ColorModel.ColorPalette = ChartColorPalette.Custom;
            columnSeries.ColorModel.CustomColors = GetCustomColors(cd);
            columnChart.Series.Add(columnSeries);

            chartFrameLayout.AddView(columnChart);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            mainView = inflater.Inflate(Resource.Layout.transactionfragment, container, false);

            SetHasOptionsMenu(true);

            FrameLayout fl = mainView.FindViewById<FrameLayout>(Resource.Id.transDataGridFrame);
            filterText = mainView.FindViewById<SearchView>(Resource.Id.searchView1);

            filterText.SetPadding(0, 20, 0, 0);
            filterText.SetQueryHint("Enter To, Amount, etc. to filter");
            filterText.QueryTextChange += FilterText_QueryTextChange;

            dataRepository = new DataRepository();
            dataRepository.filtertextchanged = OnFilterChanged;

            dataGrid = new SfDataGrid(Context)
            {
                RowHeight = 30,
                HeaderRowHeight = 30,
                AllowResizingColumn = true,
                GridStyle = new SyncFusionUtilities.CustomGridStyle(),
                ResizingMode = ResizingMode.OnMoved,
                ItemsSource = dataRepository.TransactionDataCollection,
                FrozenColumnsCount = 2,
                AutoGenerateColumns = false,
                ColumnSizer = ColumnSizer.None,
                AllowSorting = true,
                SelectionMode = SelectionMode.Single,
                EnableDataVirtualization = true
            };

            dataGrid.GridTapped += DataGrid_GridTapped;

            SyncFusionUtilities.DataGridUtilities.context = Context;

            GridTextColumn dateColumn = SyncFusionUtilities.DataGridUtilities.CreateTextColumn("Date", "Date", "", 120);

            SyncFusionUtilities.StatusCell.checkIcon = Resource.Drawable.ic_check_green_dark_18dp;
            SyncFusionUtilities.StatusCell.circleIcon = Resource.Drawable.ic_rotate_left_blue_dark_18dp;
            
            dateColumn.UserCellType = typeof(SyncFusionUtilities.StatusCell);
            dateColumn.LoadUIView = true;
            dateColumn.AllowSorting = false;

            dataGrid.Columns.Add(dateColumn);

            dataGrid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("Type", "Type", "", 120, true));
            dataGrid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("To", "To", "", 180, true));
            dataGrid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("Status", "Status", "", 180, true));
            dataGrid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("Address", "Address", "", -1, true));
            dataGrid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("Currency", "Currency", "", -1, true));
            dataGrid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("Coins", "Coins", "", 180));
            dataGrid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("Amount", "Amount", "",180));
            dataGrid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("Debit", "Debit","", -1, true));
            dataGrid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("TransactionFee", "TransactionFee","", -1, true));
            dataGrid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("TransactionId", "TransactionId", "", -1, true));
            dataGrid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("TransactionSizeUnit", "TransactionSizeUnit", "", -1, true));
            dataGrid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("TransactionSize", "TransactionSize","", -1, true));
            dataGrid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("OutputSize", "OutputSize","", -1, true));

            fl.AddView(dataGrid);

            CreateTransactionChart(mainView);

            return mainView;
        }

        private void ShowTransactionDetailDialog()
        {
            ObservableCollection<TransactionDisplayData> data =
               new ObservableCollection<TransactionDisplayData>();
            string currency = "";
            string transSizeUnit = "";
            string cellValue;

            foreach (var column in dataGrid.Columns)
            {
                if ((column.MappingName == "StatusIcon") || (column.MappingName == "LabelStatusIcon")) continue;

                var rowData = dataGrid.GetRecordAtRowIndex(selectedRowIndex);

                if (column.MappingName == "Currency")
                {
                    currency = dataGrid.GetCellValue(rowData, column.MappingName).ToString();
                    continue;
                }

                if (column.MappingName == "TransactionSizeUnit")
                {
                    transSizeUnit = dataGrid.GetCellValue(rowData, column.MappingName).ToString();
                    continue;
                }

                cellValue = dataGrid.GetCellValue(rowData, column.MappingName).ToString();

                if ((column.MappingName == "Debit") || (column.MappingName == "TransactionFee") ||
                        (column.MappingName == "Amount")) cellValue += " " + currency;
                else if (column.MappingName == "TransactionSize") cellValue += " " + transSizeUnit;

                data.Add(new TransactionDisplayData(column.MappingName, cellValue));
            }

            SfDataGrid grid = new SfDataGrid(Context)
            {
                RowHeight = 30,
                HeaderRowHeight = 0,
                GridStyle = new SyncFusionUtilities.CustomGridStyle(),
                AutoGenerateColumns = false,
                ColumnSizer = ColumnSizer.None,
                FrozenColumnsCount = 1,
                ItemsSource = data
            };

            grid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("ColumnOneData", "One", "", 125));
            grid.Columns.Add(SyncFusionUtilities.DataGridUtilities.CreateTextColumn("ColumnTwoData", "Two", "", 500));

            var builder = new Android.Support.V7.App.AlertDialog.Builder(Context);

            builder.SetTitle("Transaction Details");

            LayoutInflater layoutInflater = Activity.LayoutInflater;

            View dialogView = layoutInflater.Inflate(Resource.Layout.customdialoglayout, null);

            LinearLayout lay = dialogView.FindViewById<LinearLayout>(Resource.Id.customLinearLayout);

            lay.AddView(grid);

            builder.SetView(dialogView);

            builder.SetNegativeButton(Android.Resource.String.Ok, (senderAlert, args) => { });

            builder.Show();
        }

        private void DataGrid_GridTapped(object sender, GridTappedEventArgs e)
        {
            View view = dataGrid.GetChildAt(e.RowColumnIndex.ColumnIndex);
            if (view == null) return;

            var menu = new PopupMenu(Context, view);
            menu.Inflate(Resource.Menu.DataGridPopupMenu);

            menu.MenuItemClick += Menu_MenuItemClick;

            selectedRowIndex = e.RowColumnIndex.RowIndex;

            menu.Show();
        }

        private void Menu_MenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs e)
        {
            switch (e.Item.ItemId)
            {
                case (Resource.Id.showTransactionDetails):
                    ShowTransactionDetailDialog();
                    break;
                default:
                    break;
            }
        }

        void OnFilterChanged()
        {
            if (dataGrid.View != null)
            {
                this.dataGrid.View.Filter = dataRepository.FilterRecords;
                this.dataGrid.View.RefreshFilter();
            }
        }

        private void FilterText_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            dataRepository.FilterText = (sender as SearchView).Query;
        }
    }
}