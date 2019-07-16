using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Syncfusion.Charts;
using CoinDataModel;
using System;
using System.Collections.Generic;

namespace SeriousWallet3.Fragments
{
    public class CandleChartFragment : Fragment
    {
        SfChart chart;
        SfChart volumeChart;
        CandleSeriesModel candleSeriesModel;
        private static CandleChartFragment candleChartFragment = null;
        static string symbol;
        static string description;

        public static CandleChartFragment NewInstance(string sym, string desc)
        {
            description = desc;
            symbol = sym;

            return ((candleChartFragment == null) ?
                (candleChartFragment = new CandleChartFragment { Arguments = new Bundle() }) :
                candleChartFragment);
        }

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
            inflater.Inflate(Resource.Menu.ChartToolBarMenu, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case (Resource.Id.menu_configuration):
                    Toast.MakeText(Context, "To be implemented", ToastLength.Short).Show();
                    break;

                default:
                    break;
            }

            return true;
        }

        private void CreateVolumeChart()
        {
            volumeChart = new SfChart(Context);

            DateTimeAxis dateTimeAxis = new DateTimeAxis();
            dateTimeAxis.LabelStyle.LabelFormat = "MM/dd/yyyy";
            dateTimeAxis.LabelRotationAngle = -45;
            volumeChart.PrimaryAxis = dateTimeAxis;

            System.Globalization.CultureInfo uiCulture1 = System.Globalization.CultureInfo.CurrentUICulture;
            NumericalAxis na = new NumericalAxis();
            na.OpposedPosition = true;
            na.LabelCreated += Na_LabelCreated;

            volumeChart.SecondaryAxis = na;

            ColumnSeries columnSeries = new ColumnSeries()
            {
                ItemsSource = candleSeriesModel.CandleDataSeries,
                XBindingPath = "Date",
                YBindingPath = "Volume",
                EnableAnimation = true
            };

            volumeChart.Series.Add(columnSeries);

            volumeChart.Series[0].ColorModel.ColorPalette = ChartColorPalette.Custom;
            volumeChart.Series[0].ColorModel.CustomColors = GetBarColors();

            volumeChart.LayoutParameters = new
                FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        }

        List<Color> GetBarColors()
        {
            List<Color> clr = new List<Color>();

            foreach (OHLCV oh in candleSeriesModel.CandleDataSeries)
            {
                if (oh.Close > oh.Open) clr.Add(Color.Black);
                else if (oh.Close < oh.Open) clr.Add(Color.Red);
                else clr.Add(Color.Gray);
            }

            return clr;
        }

        private void Na_LabelCreated(object sender, ChartAxis.LabelCreatedEventArgs e)
        {
            decimal d = Convert.ToDecimal(e.AxisLabel.LabelContent);
            e.AxisLabel.LabelContent = d.ToString("#,##0,,M", System.Globalization.CultureInfo.InvariantCulture);
        }

        private void CreateCandleChart()
        {
            chart = new SfChart(Context);
            chart.SetBackgroundColor(Color.White);
            chart.Title.Text = "BCT";
            chart.Title.TextSize = 15;
            DateTimeAxis dateTimeAxis = new DateTimeAxis();
            chart.PrimaryAxis = dateTimeAxis;
            chart.PrimaryAxis.Visibility = Visibility.Gone;

            NumericalAxis numericalAxis = new NumericalAxis();
            numericalAxis.OpposedPosition = true;
            numericalAxis.LabelCreated += NumericalAxis_LabelCreated;
            chart.SecondaryAxis = numericalAxis;

            CandleSeries candleSeries = new CandleSeries();
            candleSeries.ItemsSource = candleSeriesModel.CandleDataSeries;
            candleSeries.StrokeColor = Color.Black;
            candleSeries.StrokeWidth = 0.75f;
            candleSeries.XBindingPath = "Date";
            candleSeries.Open = "Open";
            candleSeries.Close = "Close";
            candleSeries.High = "High";
            candleSeries.Low = "Low";
            candleSeries.TooltipEnabled = true;
            candleSeries.EnableAnimation = true;
            candleSeries.EnableSolidCandles = true;
            candleSeries.BullFillColor = Color.LawnGreen;
            candleSeries.BearFillColor = Color.Red;

            chart.Series.Add(candleSeries);

            chart.LayoutParameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        }

        private void NumericalAxis_LabelCreated(object sender, ChartAxis.LabelCreatedEventArgs e)
        {
            decimal d = Convert.ToDecimal(e.AxisLabel.LabelContent);
            e.AxisLabel.LabelContent = d.ToString("#,#", System.Globalization.CultureInfo.InvariantCulture);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view = inflater.Inflate(Resource.Layout.candlechartfragment, container, false);

            candleSeriesModel = new CandleSeriesModel();

            FrameLayout fl1 = view.FindViewById<FrameLayout>(Resource.Id.frameLayout1);
            FrameLayout fl2 = view.FindViewById<FrameLayout>(Resource.Id.frameLayout2);

            CreateCandleChart();
            CreateVolumeChart();

            fl1.AddView(chart);
            fl2.AddView(volumeChart);

            return view;
        }
    }
}