using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Syncfusion.Charts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoinDataModel;
using SeriousWallet3.DataStructures;

namespace SeriousWallet3.Fragments
{
    public class PortfolioChartsFragment : Fragment
    {
        SfChart pieChart;
        SfChart lineChart;
        SfChart columnChart;
        string pieChartTag = "PieChart";
        string columnChartTag = "ColumnChartTag";
        string lineChartTag = "lineChartTag";
        ColumnSeriesViewModel columnSeriesViewModel;
        bool pieChartAdded = false;
        bool columnChartAdded = false;
        bool lineChartAdded = false;
        FrameLayout chartLayout;
        DataRepository dataRepository;
        private static PortfolioChartsFragment portfolioChartsFragment = null;

        public static PortfolioChartsFragment NewInstance()
        {
            return ((portfolioChartsFragment == null) ?
                (portfolioChartsFragment = new PortfolioChartsFragment { Arguments = new Bundle() }) :
                portfolioChartsFragment);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            var view = inflater.Inflate(Resource.Layout.portfoliochartsfragment, container, false);

            chartLayout = view.FindViewById<FrameLayout>(Resource.Id.chartFrameLayout);

            chartLayout.ChildViewAdded += ChartLayout_ChildViewAdded;
            chartLayout.ChildViewRemoved += ChartLayout_ChildViewRemoved;

            InitializeLineChart();

            InitializePieChart();

            InitializeColumnChart();

            CreateChartControls(view);

            DrawLineChart();

            return view;
        }

        private void ChartLayout_ChildViewRemoved(object sender, ViewGroup.ChildViewRemovedEventArgs e)
        {
            if (e.Child.Tag.Equals(pieChartTag)) pieChartAdded = false;
            else if (e.Child.Tag.Equals(columnChartTag)) columnChartAdded = false;
            else if (e.Child.Tag.Equals(lineChartTag)) lineChartAdded = false;
        }

        private void ChartLayout_ChildViewAdded(object sender, ViewGroup.ChildViewAddedEventArgs e)
        {
            if (e.Child.Tag.Equals(pieChartTag)) pieChartAdded = true;
            else if (e.Child.Tag.Equals(columnChartTag)) columnChartAdded = true;
            else if (e.Child.Tag.Equals(lineChartTag)) lineChartAdded = true;
        }

        private void InitializeLineChart()
        {
            if (chartLayout.Context == null) return;

            lineChart = new SfChart(chartLayout.Context);
            lineChart.Title.Text = "Total Portfolio Value";

            dataRepository = new DataRepository();

            CategoryAxis categoryaxis = new CategoryAxis();
            categoryaxis.LabelPlacement = LabelPlacement.BetweenTicks;
            categoryaxis.EdgeLabelsDrawingMode = EdgeLabelsDrawingMode.Shift;
            categoryaxis.Title.Text = "Month";
            lineChart.PrimaryAxis = categoryaxis;

            NumericalAxis numericalaxis = new NumericalAxis();
            numericalaxis.Title.Text = "Thousands";
            numericalaxis.RangePadding = NumericalPadding.Normal;
            numericalaxis.LabelStyle.LabelFormat = "$##.##";
            lineChart.SecondaryAxis = numericalaxis;

            LineSeries lineSeries = new LineSeries()
            {
                ItemsSource = dataRepository.LineChartCollection,
                XBindingPath = "Name",
                YBindingPath = "Value"
            };

            lineChart.Tag = lineChartTag;

            lineChart.Series.Add(lineSeries);

            FrameLayout.LayoutParams flp = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MatchParent,
                FrameLayout.LayoutParams.MatchParent);

            lineChart.LayoutParameters = flp;
        }

        private List<Color> GetCustomColorsList(ObservableCollection<ChartDataModel> ocdm)
        {
            List<Color> colors = new List<Color>();

            foreach (ChartDataModel cdm in ocdm)
            {
                colors.Add(CryptoCoinColors.GetCryptoCoinColor(cdm.Name));
            }

            return colors;
        }

        private void InitializePieChart()
        {
            if (chartLayout.Context == null) return;

            pieChart = new SfChart(chartLayout.Context);
            pieChart.Title.Text = "Coin Distribution";
          
            pieChart.Legend.Visibility = Visibility.Visible;
            pieChart.Legend.DockPosition = ChartDock.Left;
            pieChart.Legend.OverflowMode = ChartLegendOverflowMode.Wrap;

            columnSeriesViewModel = new ColumnSeriesViewModel();

            DoughnutSeries doughnutSeries = new DoughnutSeries()
            {
                ItemsSource = columnSeriesViewModel.AssetsPerCoin,
                XBindingPath = "Name",
                YBindingPath = "Value",
                DoughnutCoefficient = 0.4f,
                CircularCoefficient = 0.9f
            };

            doughnutSeries.ColorModel.ColorPalette = ChartColorPalette.Custom;
            doughnutSeries.ColorModel.CustomColors = GetCustomColorsList(columnSeriesViewModel.AssetsPerCoin);
            doughnutSeries.DataMarker.LabelContent = LabelContent.Percentage;
            doughnutSeries.DataMarker.ShowLabel = false;

            pieChart.Series.Add(doughnutSeries);

            pieChart.Tag = pieChartTag;

            pieChart.LayoutParameters = new
              FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        }

        private void InitializeColumnChart()
        {
            if (chartLayout.Context == null) return;

            columnChart = new SfChart(chartLayout.Context);
            columnChart.PrimaryAxis = new CategoryAxis();
            columnChart.SecondaryAxis = new NumericalAxis();

            columnChart.Legend.ToggleSeriesVisibility = true;
            columnChart.Legend.DockPosition = ChartDock.Bottom;

            columnChart.Tag = columnChartTag;

            columnChart.LayoutParameters = new
                FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        }

        private void DrawLineChart()
        {
            if (columnChartAdded) chartLayout.RemoveView(columnChart);
            if (pieChartAdded) chartLayout.RemoveView(pieChart);
            if (!lineChartAdded) chartLayout.AddView(lineChart);
        }

        private void DrawDoughnutChart()
        {
            if (columnChartAdded) chartLayout.RemoveView(columnChart);
            if (lineChartAdded) chartLayout.RemoveView(lineChart);
            if (!pieChartAdded) chartLayout.AddView(pieChart);
        }

        private void DrawGainLossChart()
        {
            DrawColumnChart("Gain or Loss", "Symbol", "Dollars", columnSeriesViewModel.GainLossData);
        }

        private void DrawPricePercentGainChart()
        {
            DrawColumnChart("Price % Gain", "Symbol", "Percent", columnSeriesViewModel.PricePercentGain);
        }

        private void DrawNumberOfCoinsChart()
        {
            DrawColumnChart("Number of Coins", "Symbol", "#", columnSeriesViewModel.NumberOfCoins);
        }

        private void DrawPercentOwnedChart()
        {
            DrawColumnChart("% Ownded", "Symbol", "Percent", columnSeriesViewModel.PercentOwned);
        }

        private void DrawCurrentValueChart()
        {
            DrawColumnChart("Current Value", "Symbol", "Dollars", columnSeriesViewModel.CurrentValue);
        }

        private void DrawColumnChart(string title, string primaryAxisTitle, string secondaryAxisTitle,
            ObservableCollection<ChartDataModel> dataSeries)
        {
            if (pieChartAdded) chartLayout.RemoveView(pieChart);
            if (lineChartAdded) chartLayout.RemoveView(lineChart);

            columnChart.Series.Clear();

            columnChart.Title.Text = title;

            columnChart.PrimaryAxis.Title.Text = primaryAxisTitle;

            columnChart.SecondaryAxis.Title.Text = secondaryAxisTitle;

            ColumnSeries columnSeries = new ColumnSeries()
            {
                ItemsSource = dataSeries,
                XBindingPath = "Name",
                YBindingPath = "Value",
                EnableAnimation = true
            };

            //var colors = new List<Color>()
            //{
            //    Color.Red,
            //    Color.Gray,
            //    Color.Blue,
            //    Color.Blue,
            //    Color.Blue,
            //    Color.Maroon,
            //    Color.Maroon,
            //    Color.Pink,
            //    Color.Pink,
            //    Color.Gold,
            //    Color.Gray,
            //    Color.Green,
            //    Color.LightBlue,
            //    Color.LightCoral,
            //    Color.Lime,
            //    Color.Orange,
            //    Color.Cyan
            //};

            columnSeries.ColorModel.ColorPalette = ChartColorPalette.Custom;
            columnSeries.ColorModel.CustomColors = GetCustomColorsList(dataSeries);
            columnChart.Series.Add(columnSeries);

            if (!columnChartAdded) chartLayout.AddView(columnChart);
        }

        private void CreateChartControls(View view)
        {
            ImageButton ibLeft = view.FindViewById<ImageButton>(Resource.Id.leftButton);
            ImageButton ibRight = view.FindViewById<ImageButton>(Resource.Id.rightButton);

            RadioGroup rg = view.FindViewById<RadioGroup>(Resource.Id.radioGroup1);

            rg.CheckedChange += (s, e) =>
            {
                switch (e.CheckedId)
                {
                    case (Resource.Id.radioButton1):
                        DrawLineChart();
                        break;
                    case (Resource.Id.radioButton2):
                        DrawDoughnutChart();
                        break;
                    case (Resource.Id.radioButton3):
                        DrawGainLossChart();
                        break;
                    case (Resource.Id.radioButton4):
                        DrawPricePercentGainChart();
                        break;
                    case (Resource.Id.radioButton5):
                        DrawNumberOfCoinsChart();
                        break;
                    case (Resource.Id.radioButton6):
                        DrawPercentOwnedChart();
                        break;
                    case (Resource.Id.radioButton7):
                        DrawCurrentValueChart();
                        break;
                    default:
                        break;
                }
            };

            ibRight.Click += (s, e) =>
            {
                if (rg.CheckedRadioButtonId != Resource.Id.radioButton7)
                    rg.Check(rg.CheckedRadioButtonId + 1);
            };

            ibLeft.Click += (s, e) =>
            {
                if (rg.CheckedRadioButtonId != Resource.Id.radioButton1)
                    rg.Check(rg.CheckedRadioButtonId - 1);
            };
        }
    }
}