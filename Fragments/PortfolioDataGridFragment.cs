using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Support.V7.Widget;
using Syncfusion.SfDataGrid;
using SyncFusionUtilities;
using CoinDataModel;

namespace SeriousWallet3.Fragments
{
    public class PortfolioDataGridFragment : Fragment
    {
        SfDataGrid dataGrid;
        DataRepository dataRepository;
        private static PortfolioDataGridFragment portfolioDataGridFragment = null;

        public static PortfolioDataGridFragment NewInstance()
        {
            return ((portfolioDataGridFragment == null) ?
                (portfolioDataGridFragment = new PortfolioDataGridFragment { Arguments = new Bundle() }) :
                portfolioDataGridFragment);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        private void CreateQuoteGrid(CardView cv)
        {
            if (dataRepository == null) dataRepository = new DataRepository();

            dataGrid = new SfDataGrid(cv.Context)
            {
                RowHeight = 30,
                HeaderRowHeight = 30,
                GridStyle = new SyncFusionUtilities.CustomGridStyle(),
                NestedScrollingEnabled = true,
                AutoGenerateColumns = false
            };

            dataGrid.GridLoaded += DataGrid_GridLoaded;
            dataGrid.ItemsSource = dataRepository.CoinDataCollection;

            DataGridUtilities.context = Context;
           
            dataGrid.Columns.Add(DataGridUtilities.CreateTextColumn("Symbol", "Symbol", "", -1, 
                false, GravityFlags.Start | GravityFlags.CenterVertical));  //57
            dataGrid.Columns.Add(DataGridUtilities.CreateTextColumn("NumberOfCoins", "Coins", "", -1,
                false, GravityFlags.Start | GravityFlags.CenterVertical));  //65
            dataGrid.Columns.Add(DataGridUtilities.CreateTextColumn("Value", "Value", "C", -1,
                false, GravityFlags.Start | GravityFlags.CenterVertical));  //80

            GridTextColumn chgColumn = DataGridUtilities.CreateTextColumn("LastChange", "Last (Change)", "", -1);  //85
            chgColumn.TextAlignment = GravityFlags.Start | GravityFlags.CenterVertical;

            TextImageGridCell.arrowUp = Resource.Drawable.ic_arrow_drop_up_green_500_18dp;
            TextImageGridCell.arrowDown = Resource.Drawable.ic_arrow_drop_down_red_500_18dp;

            chgColumn.UserCellType = typeof(TextImageGridCell);

            dataGrid.Columns.Add(chgColumn);
        }

        private void DataGrid_GridLoaded(object sender, GridLoadedEventArgs e)
        {
            var per = (dataGrid.Width )/(Resources.DisplayMetrics.Density*100);

            dataGrid.Columns[0].Width = per * 18;
            dataGrid.Columns[1].Width = per * 18;
            dataGrid.Columns[2].Width = per * 20;
            dataGrid.Columns[3].Width = per * 44;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            var view =  inflater.Inflate(Resource.Layout.portfoliodatagridfragment, container, false);

            var cv = view.FindViewById<CardView>(Resource.Id.portfolioDataGridCardView);

            CreateQuoteGrid(cv);

            dataGrid.LayoutParameters = new CardView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

            cv.AddView(dataGrid);

            return view;
        }
    }
}