using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Syncfusion.SfDataGrid;
using Syncfusion.Data;


namespace SyncFusionUtilities
{
    public class WalletImageGridCell : GridCell
    {
        private TextView textview;
        public static string currentWallet;
        public static int checkMark;

        public WalletImageGridCell(Context ctxt)
            : base(ctxt)
        {
            textview = new TextView(ctxt);
            textview.LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            textview.SetTextColor(Color.Black);
            CanRendererUnload = false;
            AddView(textview);
        }

        protected override void UnLoad()
        {
            if (Parent != null)
                (Parent as VirtualizingCellsControl).RemoveView(this);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            //if (this.DataColumn != null && DataColumn.CellValue != null && textview.Text != DataColumn.CellValue.ToString())
            //{
            textview.Text = DataColumn.CellValue.ToString();

            if (textview.Text.Contains(currentWallet))
                textview.SetCompoundDrawablesWithIntrinsicBounds(checkMark, 0, 0, 0);
            //}
        }

        protected override void Dispose(bool disposing)
        {
            textview.Dispose();
            textview = null;
            base.Dispose(disposing);
        }
    }

    public class TextImageGridCell : GridCell
    {
        private TextView textview;
        public static int arrowUp;
        public static int arrowDown;

        public TextImageGridCell(Context ctxt)
            : base(ctxt)
        {
            textview = new TextView(ctxt);
            textview.LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            textview.SetTextColor(Color.Black);
            CanRendererUnload = false;
            AddView(textview);
        }

        protected override void UnLoad()
        {
            if (Parent != null)
                (Parent as VirtualizingCellsControl).RemoveView(this);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            //if (this.DataColumn != null && DataColumn.CellValue != null && textview.Text != DataColumn.CellValue.ToString())
            //{
                textview.Text = DataColumn.CellValue.ToString();
               
            if (textview.Text.Contains("-"))
                textview.SetCompoundDrawablesWithIntrinsicBounds(arrowDown, 0, 0, 0);
            else
                textview.SetCompoundDrawablesWithIntrinsicBounds(arrowUp, 0, 0, 0);
            //}
        }

        protected override void Dispose(bool disposing)
        {
            textview.Dispose();
            textview = null;
            base.Dispose(disposing);
        }
    }

    public class ChangeCell : GridCell
    {
        TextView stockText;
        ImageView stockImage;
        public static int arrowUp;
        public static int arrowDown;

        public ChangeCell(Context context) : base(context)
        {
            stockText = new TextView(context);
            stockImage = new ImageView(context);
            stockText.Gravity = GravityFlags.Right;
            stockText.SetTextColor(Color.Rgb(51, 51, 51));
            CanRendererUnload = false;
            AddView(stockText);
            AddView(stockImage);
        }

        protected override void UnLoad()
        {
            if (Parent != null)
                (Parent as VirtualizingCellsControl).RemoveView(this);
        }

        protected override void OnDraw(Canvas canvas)
        {
            stockText.Text = DataColumn.CellValue.ToString();
            if (Convert.ToDouble(DataColumn.CellValue) > 0.0)
                stockImage.SetImageResource(arrowUp);
            else
                stockImage.SetImageResource(arrowDown);
            base.OnDraw(canvas);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            //stockText.Layout((int)(35 * Resources.DisplayMetrics.Density), (int)(10 * Resources.DisplayMetrics.Density), Width - 20, Height);
            //stockImage.Layout((int)(15 * Resources.DisplayMetrics.Density), 0, (int)(35 * Resources.DisplayMetrics.Density), Height);

            stockText.Layout((int)(40 * Resources.DisplayMetrics.Density), (int)(10 * Resources.DisplayMetrics.Density), Width - 20, Height);
            stockImage.Layout((int)(10 * Resources.DisplayMetrics.Density), 0, (int)(40 * Resources.DisplayMetrics.Density), Height);
        }
    }

    public class StatusCell : GridCell
    {
        TextView statusText;
        ImageView statusImage;
        public static int checkIcon;
        public static int circleIcon;
        static int row = 0;

        public StatusCell(Context context) : base(context)
        {
            statusText = new TextView(context);
            statusImage = new ImageView(context);
            statusText.Gravity = GravityFlags.Right;
            statusText.SetTextColor(Color.Rgb(51, 51, 51));
            CanRendererUnload = false;
            AddView(statusText);
            AddView(statusImage);
        }

        protected override void UnLoad()
        {
            if (Parent != null)
                (Parent as VirtualizingCellsControl).RemoveView(this);
        }

        protected override void OnDraw(Canvas canvas)
        {
            statusText.Text = DataColumn.CellValue.ToString();
            //if (++row % 2 != 0)
            //    statusImage.SetImageResource(checkIcon);
            //else
                statusImage.SetImageResource(circleIcon);
            base.OnDraw(canvas);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            statusImage.Layout((int)(15 * Resources.DisplayMetrics.Density), 0, (int)(35 * Resources.DisplayMetrics.Density), Height);
            statusText.Layout((int)(35 * Resources.DisplayMetrics.Density), (int)(10 * Resources.DisplayMetrics.Density), Width - 20, Height);
        }
    }

    public class CustomGridStyle : DataGridStyle
    {
        private int collapseIcon;
        private int expandIcon;

        public CustomGridStyle()
        {
        }

        public CustomGridStyle(int collapseIcon, int expandIcon)
        {
            this.collapseIcon = collapseIcon;
            this.expandIcon = expandIcon;
        }
        public override Color GetRecordBackgroundColor()
        {
            return Color.ParseColor("#FFE5E6FA");
        }

        public override Color GetAlternatingRowBackgroundColor()
        {
            return Color.White;
        }

        public override GridLinesVisibility GetGridLinesVisibility()
        {
            return GridLinesVisibility.None;
        }

        public override Color GetHeaderBackgroundColor()
        {
            return Color.ParseColor("#FF778898");
        }

        public override Color GetSelectionBackgroundColor()
        {
            return Color.ParseColor("#FFFAD127");
        }

        public override Color GetCaptionSummaryRowBackgroundColor()
        {
            return Color.ParseColor("#696969");
        }

        public override Color GetCaptionSummaryRowForegroundColor()
        {
            return Color.White;
        }

        public override Color GetTableSummaryBackgroundColor()
        {
            return Color.ParseColor("#bdbdbd");
        }

        public override Color GetTableSummaryForegroundColor()
        {
            return Color.White;
        }

        public override int GetGroupCollapseIcon()
        {
            return collapseIcon;
        }
        public override int GetGroupExpanderIcon()
        {
            return expandIcon;
        }
    }

    public class DataGridUtilities
    {
        public static Android.Content.Context context;

        public static TextView CreateTextView(string columnName)
        {
            if (context == null) throw new ArgumentNullException("DataGridUtilities.context cannot be null!"); ;
            TextView tv = new TextView(context);
            tv.Text = columnName;
            tv.SetTextColor(Color.White);
            tv.SetBackgroundColor(Color.ParseColor("#FF778898"));
            tv.TextAlignment = TextAlignment.Center;
            tv.SetTypeface(tv.Typeface, TypefaceStyle.Bold);

            return tv;
        }
        public static GridTextColumn CreateTextColumn(string columnName, string headerText, string format, int width, bool hidden = false, 
            GravityFlags gv = GravityFlags.Left)
        {
            GridTextColumn column = new GridTextColumn()
            {
                MappingName = columnName,
                HeaderTemplate = CreateTextView(headerText),
                TextAlignment = gv,
                Format = format,
                IsHidden = hidden,
                LoadUIView = true
            };

            if (width != -1) column.Width = width;

            return column;
        }

        public static GridDateTimeColumn CreateDateColumn(string columnName, string headerText, string format, int width, bool hidden = false)
        {
            GridDateTimeColumn column = new GridDateTimeColumn()
            {
                MappingName = columnName,
                HeaderTemplate = CreateTextView(headerText),
                TextAlignment = GravityFlags.Left,
                Format = format,
                IsHidden = hidden,
                LoadUIView = true
            };

            if (width != -1) column.Width = width;

            return column;
        }

        public static GridImageColumn CreateImageColumn(string columnName, string headerText, int width, bool hidden = false)
        {
            GridImageColumn column = new GridImageColumn()
            {
                MappingName = columnName,
                HeaderTemplate = CreateTextView(headerText),
                IsHidden = hidden,
                TextAlignment = GravityFlags.Left
            };

            if (width != -1) column.Width = width;

            return column;
        }

        public static GridNumericColumn CreateNumericColumn(string columnName, string headerText, int width, bool hidden = false)
        {
            GridNumericColumn column = new GridNumericColumn()
            {
                MappingName = columnName,
                HeaderTemplate = CreateTextView(headerText),
                TextAlignment = GravityFlags.Left,
                IsHidden = hidden,
                NumberDecimalDigits = 10,
                Format = "0.#####"
            };

            if (width != -1) column.Width = width;

            return column;
        }
    }

    public class CustomRenderer2 : GridCaptionSummaryCellRenderer
    {
        public CustomRenderer2()
        {

        }
        public override void OnInitializeDisplayView(DataColumnBase dataColumn, TextView view)
        {
            base.OnInitializeDisplayView(dataColumn, view);
            Group group = dataColumn.RowData as Group;

            if (dataColumn.GridColumn.MappingName == "Date")
            {
                var value = SummaryCreator.GetSummaryDisplayText(group.SummaryDetails, "Symbol", this.DataGrid.View);
                view.Text = group.Key + " - " + value + " Items ";
            }
        }
    }
}

namespace CoinDataModel
{
    public class Notify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void InvokePropertyChanged<T>(string name, T property, T value)
        {
            if (!property.Equals(value))
            {
                property = value;
                this.OnPropertyChanged(name);
            }
        }
    }

    public class ChartDataModel
    {
        public string Name { get; set; }

        public DateTime date { get; set; }

        public double Value { get; set; }

        public double Value1 { get; set; }

        public double Size { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Open { get; set; }

        public double Close { get; set; }

        public double Volume { get; set; }

        public ChartDataModel(string name, double value)
        {
            Name = name;
            Value = value;
        }

        public ChartDataModel(string name, decimal value)
        {
            Name = name;
            Value = System.Convert.ToDouble(value);
        }

        public ChartDataModel(string name, double value, double size)
        {
            Name = name;
            Value = value;
            Size = size;
        }

        public ChartDataModel(double value, double value1, double size)
        {
            Value1 = value;
            Value = value1;
            Size = size;
        }

        public ChartDataModel(string name, double high, double low, double open, double close)
        {
            Name = name;
            High = high;
            Low = low;
            Value = open;
            Size = close;
        }
        public ChartDataModel(DateTime Date, double high, double low, double open, double close)
        {
            date = Date;
            High = high;
            Low = low;
            Value = open;
            Size = close;
        }

        public ChartDataModel(double value, double size)
        {
            Value = value;
            Size = size;
        }

        public ChartDataModel(DateTime dateTime, double value)
        {
            date = dateTime;
            Value = value;
        }
    }

    public class OHLCV
    {
        public DateTime Date { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Open { get; set; }

        public double Close { get; set; }

        public double Volume { get; set; }

        public OHLCV(string dateString, decimal open, decimal high, decimal low, decimal close, decimal volume)
        {
            string[] d = dateString.Split('/');
            //12/1/2017
            //2017,12,1
            Date = new DateTime(System.Convert.ToInt16(d[2]), System.Convert.ToInt16(d[0]), System.Convert.ToInt16(d[1]));

            Open = System.Convert.ToDouble(open);
            High = System.Convert.ToDouble(high);
            Low = System.Convert.ToDouble(low);
            Close = System.Convert.ToDouble(close);
            Volume = System.Convert.ToDouble(volume);
        }
    }

    public class CandleSeriesModel
    {
        public ObservableCollection<OHLCV> CandleDataSeries { get; set; }

        public CandleSeriesModel()
        {
            CandleDataSeries = new ObservableCollection<OHLCV>
            {
                new OHLCV("12/19/2017",18971.18945m,19021.9707m,16812.80078m,17523.69922m,3136709262m),
                new OHLCV("12/20/2017", 17521.73047m, 17813.59961m, 15642.69043m, 16461.9707m, 3791752510m),
                new OHLCV("12/21/2017", 16461.08984m, 17301.83008m, 14952.98047m, 15632.12012m, 2619295475m),
                new OHLCV("12/22/2017", 15632.12012m, 15823.71973m, 10875.70996m, 13664.96973m, 6245731508m),
                new OHLCV("12/23/2017", 13664.96973m, 15493.23047m, 13356.07031m, 14396.45996m, 2491903154m),
                new OHLCV("12/24/2017", 14396.62988m, 14413.71973m, 12166.4502m, 13789.9502m, 2428437693m),
                new OHLCV("12/25/2017", 13789.9502m, 14467.42969m, 13010.70996m, 13833.49023m, 1487888106m),
                new OHLCV("12/26/2017", 13830.19043m, 16094.66992m, 13748.49023m, 15756.55957m, 2198577125m),
                new OHLCV("12/27/2017", 15757.01953m, 16514.58984m, 14534.66016m, 15416.63965m, 2162831128m),
                new OHLCV("12/28/2017", 15416.33984m, 15505.50977m, 13466.07031m, 14398.7002m, 2425912717m),
                new OHLCV("12/29/2017", 14398.4502m, 15109.80957m, 13951.08008m, 14392.57031m, 1733583750m),
                new OHLCV("12/30/2017", 14392.13965m, 14461.45996m, 11962.08984m, 12531.51953m, 2387311023m),
                new OHLCV("12/31/2017", 12532.37988m, 14241.82031m, 12359.42969m, 13850.40039m, 1492142483m),
                new OHLCV("1/1/2018", 13850.49023m, 13921.53027m, 12877.66992m, 13444.87988m, 1057521524m),
                new OHLCV("1/2/2018", 13444.87988m, 15306.12988m, 12934.16016m, 14754.12988m, 1956783037m),
                new OHLCV("1/3/2018", 14754.08984m, 15435.00977m, 14579.70996m, 15156.62012m, 1604206990m),
                new OHLCV("1/4/2018", 15156.49023m, 15408.66016m, 14244.66992m, 15180.08008m, 1656714736m),
                new OHLCV("1/5/2018", 15180.08008m, 17126.94922m, 14832.36035m, 16954.7793m, 2283988962m),
                new OHLCV("1/6/2018", 16954.75977m, 17252.84961m, 16286.57031m, 17172.30078m, 1412703790m),
                new OHLCV("1/7/2018", 17174.5m, 17184.81055m, 15791.12988m, 16228.16016m, 1309532650m),
                new OHLCV("1/8/2018", 16228.25977m, 16302.91992m, 13902.30957m, 14976.16992m, 2166366561m),
                new OHLCV("1/9/2018", 14976.16992m, 15390.28027m, 14221.54981m, 14468.5m, 1486802326m),
                new OHLCV("1/10/2018", 14468.08984m, 14919.49023m, 13450.54004m, 14919.49023m, 2025083791m),
                new OHLCV("1/11/2018", 14920.36035m, 14979.95996m, 12825.9502m, 13308.05957m, 2373494121m),
                new OHLCV("1/12/2018", 13308.05957m, 14129.08008m, 12851.91016m, 13841.19043m, 1402292716m),
                new OHLCV("1/13/2018", 13841.19043m, 14595.04004m, 13830.28027m, 14243.12012m, 1021352776m),
                new OHLCV("1/14/2018", 14244.12012m, 14415.66992m, 13031.91016m, 13638.62988m, 1112590573m),
                new OHLCV("1/15/2018", 13638.62988m, 14355.82031m, 13416.70996m, 13631.98047m, 1170462750m),
                new OHLCV("1/16/2018", 13634.59961m, 13648.83984m, 10032.69043m, 11282.49023m, 3842651741m),
                new OHLCV("1/17/2018", 11282.49023m, 11736.29981m, 9205.379883m, 11162.7002m, 3666978316m),
                new OHLCV("1/18/2018", 11162.7002m, 12018.42969m, 10642.33008m, 11175.51953m, 2357251805m),
                new OHLCV("1/19/2018", 11175.51953m, 11941.74023m, 10867.17969m, 11674.79004m, 1044731072m)
            };
        }
    }

    public class ColumnSeriesViewModel
    {
        public ObservableCollection<ChartDataModel> AssetsPerCoin { get; set; }
        public ObservableCollection<ChartDataModel> GainLossData { get; set; }
        public ObservableCollection<ChartDataModel> PricePercentGain { get; set; }
        public ObservableCollection<ChartDataModel> NumberOfCoins { get; set; }
        public ObservableCollection<ChartDataModel> PercentOwned { get; set; }
        public ObservableCollection<ChartDataModel> CurrentValue { get; set; }

        public ColumnSeriesViewModel()
        {
            AssetsPerCoin = new ObservableCollection<ChartDataModel>
            {
                new ChartDataModel("Bitcoin", 9343.86),
                new ChartDataModel("Ripple", 2363.81),
                new ChartDataModel("Dash", 8133.06),
                new ChartDataModel("EOS", 5515.23),
                new ChartDataModel("Ethereum", 5142.05),
                new ChartDataModel("Litecoin", 6053.58)
            };

            GainLossData = new ObservableCollection<ChartDataModel>
            {
                new ChartDataModel("BTC",6500m),
                new ChartDataModel("XRP",200m),
                new ChartDataModel("ETH",-670m),
                new ChartDataModel("DASH",150m),
                new ChartDataModel("LTC",40m),
                new ChartDataModel("OMG",-10m),
                new ChartDataModel("XRP",700m),
                new ChartDataModel("XEM",-10m),
                new ChartDataModel("XMR",350m),
                new ChartDataModel("EOS",250m),
                new ChartDataModel("XLM",100m),
                new ChartDataModel("NEO",-150m)
            };

            PricePercentGain = new ObservableCollection<ChartDataModel>
            {
                new ChartDataModel("BTC",0.1),
                new ChartDataModel("XRP",0.01),
                new ChartDataModel("ETH",-25.0),
                new ChartDataModel("DASH",-11.0),
                new ChartDataModel("LTC",11.5),
                new ChartDataModel("OMG",-2.0),
                new ChartDataModel("XRP",28.0),
                new ChartDataModel("XEM",-0.2),
                new ChartDataModel("XMR",27.0),
                new ChartDataModel("EOS",42.0),
                new ChartDataModel("XLM",8.0),
                new ChartDataModel("NEO",-20.0)
            };

            NumberOfCoins = new ObservableCollection<ChartDataModel>
            {
                new ChartDataModel("BTC",10.3),
                new ChartDataModel("XRP",12.4),
                new ChartDataModel("ETH",85.0),
                new ChartDataModel("DASH",55.0),
                new ChartDataModel("LTC",217.0),
                new ChartDataModel("OMG",120.0),
                new ChartDataModel("XRP",10000.0),
                new ChartDataModel("XEM",35.0),
                new ChartDataModel("XMR",5.0),
                new ChartDataModel("EOS",355.0),
                new ChartDataModel("XLM",250.0),
                new ChartDataModel("NEO",8.0)
            };

            PercentOwned = new ObservableCollection<ChartDataModel>
            {
                new ChartDataModel("BTC", 0.01),
                new ChartDataModel("XRP", 0.03),
                new ChartDataModel("ETH", 13.5),
                new ChartDataModel("DASH", 11.5),
                new ChartDataModel("LTC", 27.0),
                new ChartDataModel("OMG", 5.5),
                new ChartDataModel("XRP", 6.3),
                new ChartDataModel("XEM", 0.3),
                new ChartDataModel("XMR", 4.0),
                new ChartDataModel("EOS", 2.7),
                new ChartDataModel("XLM", 1.3),
                new ChartDataModel("NEO", 1.7)
            };

            CurrentValue = new ObservableCollection<ChartDataModel>
            {
                new ChartDataModel("BTC",12500.0),
                new ChartDataModel("XRP",200.0),
                new ChartDataModel("ETH",2700.0),
                new ChartDataModel("DASH",3200.0),
                new ChartDataModel("LTC",3200.0),
                new ChartDataModel("OMG",1700.0),
                new ChartDataModel("XRP",2100.0),
                new ChartDataModel("XEM",25.0),
                new ChartDataModel("XMR",450.0),
                new ChartDataModel("EOS",267.0),
                new ChartDataModel("XLM",300.0),
                new ChartDataModel("NEO",500.0)
            };
        }
    }

    public class CoinInformation
    {
        //@@todo: change name to symbol
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal PriceChange { get; set; }
        public DateTime LastUpdate { get; set; }
        public double PricePercentageChange { get; set; }
        public decimal NumberOfCoins { get; set; }
        public decimal ValueChange { get; set; }
        public double ValueChangePercentage { get; set; }

        public decimal TotalValue
        {
            get
            {
                return (NumberOfCoins * Price);
            }
        }

        public Color PriceChangeColor
        {
            get
            {
                return (PriceChange > 0 ? Color.ParseColor("#4CAF50") : Color.Red);
            }
        }

        public Color ValueChangeColor
        {
            get
            {
                return (ValueChange > 0 ? Color.ParseColor("#4CAF50") : Color.Red);
            }
        }

        public int PriceChangeIcon
        {
            get
            {
                //@@change
                return (PriceChange > 0 ? SeriousWallet3.Resource.Drawable.ic_arrow_drop_up_green_500_18dp : 
                    SeriousWallet3.Resource.Drawable.ic_arrow_drop_down_red_500_18dp);
            }
        }

        public int ValueChangeIcon
        {
            get
            {
                //@@change
                return (ValueChange > 0 ? SeriousWallet3.Resource.Drawable.ic_arrow_drop_up_green_500_18dp : 
                    SeriousWallet3.Resource.Drawable.ic_arrow_drop_down_red_500_18dp);
            }
        }
    };

    public class CoinData : Notify
    {
        private string symbol;
        private decimal numberOfCoins;
        private decimal last;
        private decimal change;
        private decimal totalValue;

        public string Symbol { get { return symbol; } set { InvokePropertyChanged("Symbol", symbol, value); } }
        public decimal NumberOfCoins { get { return numberOfCoins; } set { InvokePropertyChanged("NumberOfCoins", numberOfCoins, value); } }
        public decimal Last
        {
            get
            {
                return last;
            }
            set
            {
                totalValue = numberOfCoins * last;
                InvokePropertyChanged("Last", last, value);
            }
        }

        public string LastChange
        {
            //@@todo: format for dollar ranges: i.e.- > $1000 => $1K
            get
            {
                return (last.ToString("C") + " (" + change.ToString() + ")");
            }
        }

        public decimal Value { get { return totalValue; } set { InvokePropertyChanged("Value", totalValue, value); }  }
        public decimal Change { get { return change; } set { InvokePropertyChanged("Change", change, value); } }

        public CoinData(string symbol, decimal numberOfCoins, decimal last, decimal change)
        {
            this.symbol = symbol;
            this.numberOfCoins = numberOfCoins;
            this.last = last;
            this.change = change;
            totalValue = numberOfCoins * last;
        }
    }

    public class TransactionDisplayData : Notify
    {
        private string columnOneData;
        private string columnTwoData;

        public string ColumnOneData { get { return columnOneData; } set { InvokePropertyChanged<string>("ColumnOneData", columnOneData, value); } }
        public string ColumnTwoData { get { return columnTwoData; } set { InvokePropertyChanged<string>("ColumnTwoData", columnTwoData, value); } }

        public TransactionDisplayData(string columOneData, string columnTwoData)
        {
            this.columnOneData = columOneData;
            this.columnTwoData = columnTwoData;
        }
    }

    public class ReceiveData : Notify
    {
        private DateTime date;
        private string label;
        private string message;
        private decimal requested;

        public DateTime Date { get { return date; } set { InvokePropertyChanged<DateTime>("Date", date, value); } }

        public string Label { get { return label; } set { InvokePropertyChanged<string>("Label", label, value); } }

        public string Message { get { return message; } set { InvokePropertyChanged<string>("Message", message, value); } }

        public decimal Requested { get { return requested; } set { InvokePropertyChanged<decimal>("Requested", requested, value); } }

        public ReceiveData(DateTime date, string label, string message, decimal requested)
        {
            this.date = date;
            this.label = label;
            this.message = message;
            this.requested = requested;
        }
    }

    public class SendData : Notify
    {
        private Bitmap statusIcon;
        private DateTime date;
        private string to;
        private string address;
        private decimal amount;
        private string currency;

        public Bitmap StatusIcon { get { return statusIcon; } set { InvokePropertyChanged("StatusIcon", statusIcon, value); } }
        public DateTime Date { get { return date; } set { InvokePropertyChanged("Date", date, value); } }
        public string To { get { return to; } set { InvokePropertyChanged("To", to, value); } }
        public string Address { get { return address; } set { InvokePropertyChanged("Address", address, value); } }
        public decimal Amount { get { return amount; } set { InvokePropertyChanged("Amount", amount, value); } }
        public string Currency
        {
            get { return currency; }
            set { InvokePropertyChanged("Currency", currency, value); }
        }

        public string AmountCurrency
        {
            get
            {
                return (Amount.ToString("G29") + " " + Currency);
            }
        }

        public SendData(Bitmap statusIcon, DateTime date, string to, string address, decimal amount,
             string currency)
        {
            this.statusIcon = statusIcon;
            this.date = date;
            this.to = to;
            this.amount = amount;
            this.address = address;
            this.currency = currency;
        }
    }

    public class CoinExtendedData : Notify
    {
        string symbol = "NONE";
        DateTime entryDate = DateTime.Today;
        double entryPrice;
        double prevLast = 0;
        double last = 0;
        double change = 0;
        double position = 0;
        double pAndL = 0;

        public CoinExtendedData(string symbol, DateTime entryDate, double entryPrice, double last, double position)
        {
            this.symbol = symbol;
            this.entryDate = entryDate;
            this.entryPrice = entryPrice;
            this.last = last;
            this.change = last - prevLast;
            this.position = position;
            this.pAndL = position * (last - entryPrice);
        }

        public string Symbol
        {
            get { return symbol; }
        }

        public DateTime Date
        {
            get { return entryDate; }
        }

        public double Price
        {
            get { return entryPrice; }
        }

        public double Change
        {
            get { return change; }
            set { InvokePropertyChanged<double>("Change", change, value); }
        }

        public double PL
        {
            get { return (Coins * Price); }
            set { InvokePropertyChanged<double>("PL", (Coins * Price), value); }
        }

        public double Coins
        {
            get { return position; }
        }

        public double Last
        {
            get { return last; }
            set
            {
                if (last != value)
                {
                    last = value;
                    this.OnPropertyChanged("Last");

                    Change = last - value;

                    PL = position * (last - entryPrice);

                    prevLast = value;
                }
            }
        }
    }

    public class TransactionData : Notify
    {
        private Bitmap statusIcon;
        private DateTime date;
        private string type;
        private Bitmap labelStatusIcon;
        private string to;
        private string coins;
        private string amount;
        private string address;
        private string status;
        private string currency;
        private string debit;
        private string transactionFee;
        private string transactionId;
        private string transactionSize;
        private string transactionSizeUnit;
        private string outputSize;


        public Bitmap StatusIcon { get { return statusIcon; } set { InvokePropertyChanged("StatusIcon", statusIcon, value); } }
        public DateTime Date { get { return date; } set { InvokePropertyChanged("Date", date, value); } }
        public string Type { get { return type; } set { InvokePropertyChanged("Type", type, value); } }
        public Bitmap LabelStatusIcon { get { return labelStatusIcon; } set { InvokePropertyChanged("LabelStatusIcon", labelStatusIcon, value); } }
        public string To { get { return to; } set { InvokePropertyChanged("To", to, value); } }
        public string Coins { get { return coins; } set { InvokePropertyChanged("Coins", coins, value); } }
        public string Amount { get { return amount; } set { InvokePropertyChanged("Amount", amount, value); } }
        public string Address { get { return address; } set { InvokePropertyChanged("Address", address, value); } }
        public string Currency { get { return currency; } set { InvokePropertyChanged("Currency", currency, value); } }
        public string Debit { get { return debit; } set { InvokePropertyChanged("Debit", debit, value); } }
        public string TransactionFee { get { return transactionFee; } set { InvokePropertyChanged("TransactionFee", transactionFee, value); } }
        public string Status { get { return status; } set { InvokePropertyChanged("Status", status, value); } }
        public string TransactionId { get { return transactionId; } set { InvokePropertyChanged("TransactionId", transactionId, value); } }
        public string TransactionSize { get { return transactionSize; } set { InvokePropertyChanged("TransactionSize", transactionSize, value); } }
        public string TransactionSizeUnit
        {
            get { return transactionSizeUnit; }
            set { InvokePropertyChanged("TransactionSizeUnit", transactionSizeUnit, value); }
        }

        public string OutputSize { get { return outputSize; } set { InvokePropertyChanged("OutputSize", outputSize, value); } }

        public TransactionData(Bitmap statusIcon, DateTime date, string type, Bitmap labelStatusIcon, string to, decimal amount,
            string address, string currency, decimal debit, decimal transactionFee, string transactionId, int transactionSize,
            string transactionSizeUnit, int outputSize, string status, decimal coins)
        {
            this.statusIcon = statusIcon;
            this.date = date;
            this.type = type;
            this.labelStatusIcon = labelStatusIcon;
            this.to = to;
            this.amount = (amount *7595m).ToString("0.#####");
            this.address = address;
            this.currency = currency;
            this.debit = debit.ToString("0.#####");
            this.transactionFee = transactionFee.ToString("0.#####");
            this.transactionId = transactionId;
            this.transactionSize = System.Convert.ToString(transactionSize);
            this.transactionSizeUnit = System.Convert.ToString(transactionSizeUnit);
            this.outputSize = System.Convert.ToString(outputSize);
            this.status = status;
            this.coins = coins.ToString("0.#####");
        }
    }

    public class WalletData : Notify
    {
        private string walletName;
        private string walletAddress;

        public string WalletName { get => walletName; set => walletName = value; }
        public string WalletAddress { get => walletAddress; set => walletAddress = value; }

        public WalletData(string walletName, string walletAddress)
        {
            this.walletName = walletName;
            this.walletAddress = walletAddress;
        }
    }

    public class Wallet : Notify
    {
        private string walletName;
        private string walletDescription;

        public string WalletName { get => walletName; set => InvokePropertyChanged("WalletName", walletName, value); }
        public string WalletDescription { get => walletDescription; set => InvokePropertyChanged("WalletDescription", walletDescription, value); }

        public Wallet(string walletName, string walletDescription)
        {
            this.walletName = walletName;
            this.walletDescription = walletDescription;
        }
    }

    public class DataRepository : Notify
    {
        private ObservableCollection<ChartDataModel> coinBarChartCollection;
        private ObservableCollection<CoinData> coinDataCollection;
        private ObservableCollection<ChartDataModel> lineChartCollection;
        private ObservableCollection<ReceiveData> receiveDataCollection;
        private ObservableCollection<CoinExtendedData> coinExtendedDataCollection;
        private ObservableCollection<TransactionData> transactionDataCollection;
        private ObservableCollection<CoinInformation> coinInformationCollection;
        private ObservableCollection<SendData> sendDataCollection;
        private ObservableCollection<Wallet> walletDataCollection;

        #region SyncfusionSearch
        private string filtertext = "";

        internal delegate void FilterChanged();
        internal FilterChanged filtertextchanged;

        public string FilterText
        {
            get { return filtertext; }
            set
            {
                filtertext = value;
                OnFilterTextChanged();
                RaisePropertyChanged("FilterText");
            }
        }

        private void OnFilterTextChanged()
        {
            filtertextchanged();
        }

        public bool FilterRecords(object o)
        {
            var item = o as TransactionData;
            string lowerFiltertext = filtertext.ToLower();

            if (item.Date.ToString().ToLower().Contains(filtertext.ToLower()) ||
                item.To.ToLower().Contains(lowerFiltertext) ||
                item.Type.ToLower().Contains(lowerFiltertext) ||
                item.Amount.ToString().ToLower().Contains(lowerFiltertext))
                return true;
            else return false;
        }
        #endregion

        public ObservableCollection<Wallet> WalletDataCollection
        {
            get
            {
                if (walletDataCollection == null)
                {
                    InitiateWalletData();
                }

                return walletDataCollection;
            }
            set
            {
                walletDataCollection = value;
                RaisePropertyChanged("WalletDataCollection");
            }
        }

        public ObservableCollection<SendData> SendDataCollection
        {
            get
            {
                if (sendDataCollection == null)
                {
                    InitiateSendData();
                }

                return sendDataCollection;
            }
        }

        public ObservableCollection<CoinInformation> CoinInformationCollection
        {
            get
            {
                if (coinInformationCollection == null)
                {
                    InitiateCoinInformation();
                }

                return coinInformationCollection;
            }
        }

        public ObservableCollection<ReceiveData> ReceiveDataCollection
        {
            get
            {
                if (receiveDataCollection == null)
                {
                    InitiateReceiveDataCollection();
                }

                return receiveDataCollection;
            }
            set
            {
                this.receiveDataCollection = value;
                RaisePropertyChanged("ReceiveDataCollection");
            }
        }

        public ObservableCollection<ChartDataModel> LineChartCollection
        {
            get
            {
                if (lineChartCollection == null)
                {
                    InitLineChartData();
                }

                return lineChartCollection;
            }
        }

        public ObservableCollection<ChartDataModel> CoinBarChartCollection
        {
            get
            {
                if (coinBarChartCollection == null)
                {
                    InitiateCoinBarChart();
                }

                return coinBarChartCollection;
            }
        }

        public ObservableCollection<CoinData> CoinDataCollection
        {
            get
            {
                if (coinDataCollection == null)
                {
                    InitiateCoinData();
                }

                return coinDataCollection;
            }
            set
            {
                this.CoinDataCollection = value;
                RaisePropertyChanged("CoinDataCollection");
            }
        }

        public ObservableCollection<CoinExtendedData> CoinExtendedDataCollection
        {
            get
            {
                if (coinExtendedDataCollection == null)
                {
                    InitiateCoinExtendedDataCollection();
                }

                return coinExtendedDataCollection;
            }
        }

        public ObservableCollection<TransactionData> TransactionDataCollection
        {
            get
            {
                if (transactionDataCollection == null)
                {
                    InitiateTransactionData();
                }

                return transactionDataCollection;
            }
            set
            {
                this.transactionDataCollection = value;
                RaisePropertyChanged("TransactionDataCollection");
            }
        }

        private void InitiateWalletData()
        {
            walletDataCollection = new ObservableCollection<Wallet>();

            //walletDataCollection.Add(new WalletData("RZWallet", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"));
            //walletDataCollection.Add(new WalletData("Wallet2", "2NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"));
            //walletDataCollection.Add(new WalletData("Wallet3", "3NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"));
            //walletDataCollection.Add(new WalletData("Wallet4", "4NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"));
            //walletDataCollection.Add(new WalletData("Wallet5", "5NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"));
            //walletDataCollection.Add(new WalletData("Wallet6", "6NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"));
            //walletDataCollection.Add(new WalletData("Wallet7", "7NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"));
        }

        private void InitiateCoinExtendedDataCollection()
        {
            coinExtendedDataCollection = new ObservableCollection<CoinExtendedData>();

            coinExtendedDataCollection.Add(new CoinExtendedData("BTC", new DateTime(2017, 1, 1), 788.8, 7124.3, 2.567));
            coinExtendedDataCollection.Add(new CoinExtendedData("BTC", new DateTime(2017, 2, 1), 808.96, 7124.3, 1.3547));
            coinExtendedDataCollection.Add(new CoinExtendedData("BTC", new DateTime(2017, 3, 1), 898.45, 7124.3, 0.7536));

            coinExtendedDataCollection.Add(new CoinExtendedData("ETH", new DateTime(2017, 5, 12), 15.19, 329.97, 1012.45));
            coinExtendedDataCollection.Add(new CoinExtendedData("ETH", new DateTime(2017, 6, 12), 17.34, 329.97, 565.345));
            coinExtendedDataCollection.Add(new CoinExtendedData("ETH", new DateTime(2017, 7, 15), 18.26, 329.97, 345.67));

            coinExtendedDataCollection.Add(new CoinExtendedData("LTC", new DateTime(2017, 1, 12), 45.67, 83.45, 200.045));
            coinExtendedDataCollection.Add(new CoinExtendedData("LTC", new DateTime(2017, 2, 12), 44.34, 83.45, 125.42));
            coinExtendedDataCollection.Add(new CoinExtendedData("LTC", new DateTime(2017, 3, 15), 55.26, 83.45, 67.89));

            coinExtendedDataCollection.Add(new CoinExtendedData("ZEC", new DateTime(2017, 4, 12), 200.56, 354.22, 98.67));
            coinExtendedDataCollection.Add(new CoinExtendedData("ZEC", new DateTime(2017, 5, 12), 189.42, 354.22, 120.92));
            coinExtendedDataCollection.Add(new CoinExtendedData("ZEC", new DateTime(2017, 6, 15), 123.67, 354.22, 134.89));
        }

        private void InitLineChartData()
        {
            lineChartCollection = new ObservableCollection<ChartDataModel>();

            lineChartCollection.Add(new ChartDataModel("Aug", 440m));
            lineChartCollection.Add(new ChartDataModel("Sep", 443m));
            lineChartCollection.Add(new ChartDataModel("Oct", 447m));
            lineChartCollection.Add(new ChartDataModel("Nov", 450m));
            lineChartCollection.Add(new ChartDataModel("Dec", 451m));
            lineChartCollection.Add(new ChartDataModel("Jan", 442m));
        }

        private void InitiateCoinBarChart()
        {
            coinBarChartCollection = new ObservableCollection<ChartDataModel>();

            if (coinDataCollection == null) InitiateCoinData();

            foreach (CoinData cd in coinDataCollection)
            {
                coinBarChartCollection.Add(new ChartDataModel(cd.Symbol, cd.Value));
            }
        }

        private void InitiateCoinData()
        {
            coinDataCollection = new ObservableCollection<CoinData>();

            coinDataCollection.Add(new CoinData("BTC", 12.3456m, 15885.03m, -1234m));
            coinDataCollection.Add(new CoinData("BCH", 134.567m, 897.789m, 25.67m));
            coinDataCollection.Add(new CoinData("ETH", 45.678m, 743.67m, -31.45m));
            coinDataCollection.Add(new CoinData("DASH", 63.567m, 1179m, 111.24m));
            coinDataCollection.Add(new CoinData("LTC", 345.679m, 280.92m, -10.72m));
            coinDataCollection.Add(new CoinData("OMG", 134.56m, 14.762m, -0.75m));
            coinDataCollection.Add(new CoinData("XRP", 500.779m, 1.009m, -0.09m));
            coinDataCollection.Add(new CoinData("XMR", 10.765m, 357.50m, 23.45m));
            coinDataCollection.Add(new CoinData("EOS", 1287.57m, 8.3072m, 0.15m));
            coinDataCollection.Add(new CoinData("NLC2", 12345.789m, 14.762m, 0.72m));
            coinDataCollection.Add(new CoinData("NEO", 34.678m, 61.69m, -2.89m));
            coinDataCollection.Add(new CoinData("PAY", 834.67m, 4.23887m, -0.89m));
        }

        private void InitiateReceiveDataCollection()
        {
            receiveDataCollection = new ObservableCollection<ReceiveData>();

            receiveDataCollection.Add(new ReceiveData(DateTime.Now, "SomeLable1", "SomeMessage1", 0.000000125m));
            receiveDataCollection.Add(new ReceiveData(DateTime.Now, "SomeLable2", "SomeMessage2", 0.000000126m));
            receiveDataCollection.Add(new ReceiveData(DateTime.Now, "SomeLable3", "SomeMessage3", 0.000000135m));
            receiveDataCollection.Add(new ReceiveData(DateTime.Now, "SomeLable4", "SomeMessage4", 0.000000145m));
            receiveDataCollection.Add(new ReceiveData(DateTime.Now, "SomeLable5", "SomeMessage5", 0.000000155m));
            receiveDataCollection.Add(new ReceiveData(DateTime.Now, "SomeLable6", "SomeMessage6", 0.000000165m));
            receiveDataCollection.Add(new ReceiveData(DateTime.Now, "SomeLable7", "SomeMessage7", 0.000000175m));
            receiveDataCollection.Add(new ReceiveData(DateTime.Now, "SomeLable8", "SomeMessage8", 0.000000185m));
            receiveDataCollection.Add(new ReceiveData(DateTime.Now, "SomeLable9", "SomeMessage9", 0.000000198m));
        }

        public void AddTransactionData(DateTime dt, string address, string label, decimal coins, decimal amount, string type)
        {
            if (transactionDataCollection == null) InitiateTransactionData();

            transactionDataCollection.Add(new TransactionData(ConvertIconToBitmap("ic_call_missed_outgoing_blue_500_18dp"),
                dt, type, ConvertIconToBitmap("ic_call_missed_outgoing_blue_500_18dp"),
                label, amount, "3NHauBuw6faZv8bD66QLiZpdYx2HoemdAs", "BTC", -0.501m, -0.00029783m,
                address, 255, "bytes", 1, "3660 confirmations", coins));
        }

        private void InitiateTransactionData()
        {
            transactionDataCollection = new ObservableCollection<TransactionData>();

            transactionDataCollection.Add(new TransactionData(ConvertIconToBitmap("ic_call_missed_outgoing_blue_500_18dp"),
                DateTime.Now, "Sent to", ConvertIconToBitmap("ic_call_missed_outgoing_blue_500_18dp"),
                "Todd Bittrex", -3.94142104m, "3NHauBuw6faZv8bD66QLiZpdYx2HoemdAs", "BTC", -0.501m, -0.00029783m,
                "6664a73ff2b139c7ffad8bf562ef5ca861248dbc74f3aa9b1c9da516961dcb4", 255, "bytes", 1, "3660 confirmations", 100.45m));
            transactionDataCollection.Add(new TransactionData(ConvertIconToBitmap("ic_check_green_light_18dp"),
                DateTime.Now, "Sent to", ConvertIconToBitmap("ic_call_missed_outgoing_blue_500_18dp"),
                "Kraken", -0.50129783m, "3NHauBuw6faZv8bD66QLiZpdYx2HoemdAs", "BTC", -0.501m, -0.00029783m,
                "6664a73ff2b139c7ffad8bf562ef5ca861248dbc74f3aa9b1c9da516961dcb4", 255, "bytes", 1, "3660 confirmations", 55.65m));
            transactionDataCollection.Add(new TransactionData(ConvertIconToBitmap("ic_check_green_light_18dp"),
                DateTime.Now, "Received With", ConvertIconToBitmap("ic_call_missed_blue_500_18dp"),
                "1F5zjP6TSssVeyZf36ADcA3qCnX5jLNcuJi", -0.00400000m, "3NHauBuw6faZv8bD66QLiZpdYx2HoemdAs", "BTC", -0.501m, -0.00029783m,
                "6664a73ff2b139c7ffad8bf562ef5ca861248dbc74f3aa9b1c9da516961dcb4", 255, "bytes", 1, "3660 confirmations", 25.3m));
            transactionDataCollection.Add(new TransactionData(ConvertIconToBitmap("ic_check_green_light_18dp"),
                DateTime.Now, "Sent to", ConvertIconToBitmap("ic_call_missed_outgoing_blue_500_18dp"),
                "1JJR1Xf5uqvsP3zogQb391eiBe2ge4vYoG", -1.20013000m, "3NHauBuw6faZv8bD66QLiZpdYx2HoemdAs", "BTC", -0.501m, -0.00029783m,
                "6664a73ff2b139c7ffad8bf562ef5ca861248dbc74f3aa9b1c9da516961dcb4", 255, "bytes", 1, "3660 confirmations", 1.23m));
            transactionDataCollection.Add(new TransactionData(ConvertIconToBitmap("ic_check_green_light_18dp"),
                DateTime.Now, "Sent to", ConvertIconToBitmap("ic_call_missed_outgoing_blue_500_18dp"),
                "114R1Xf5uqvsP3zogQb391eiBe2ge4vYoG", -1.20013000m, "3NHauBuw6faZv8bD66QLiZpdYx2HoemdAs", "BTC", -0.501m, -0.00029783m,
                "6664a73ff2b139c7ffad8bf562ef5ca861248dbc74f3aa9b1c9da516961dcb4", 255, "bytes", 1, "3660 confirmations", 10.4m));
            transactionDataCollection.Add(new TransactionData(ConvertIconToBitmap("ic_check_green_light_18dp"),
                DateTime.Now, "Sent to", ConvertIconToBitmap("ic_call_missed_outgoing_blue_500_18dp"),
                "1PmjFFu5uqvsP3zogQb391eiBe2ge4vYoG", -4.50875000m, "3NHauBuw6faZv8bD66QLiZpdYx2HoemdAs", "BTC", -0.501m, -0.00029783m,
                "6664a73ff2b139c7ffad8bf562ef5ca861248dbc74f3aa9b1c9da516961dcb4", 255, "bytes", 1, "3660 confirmations", 34.45m));
            transactionDataCollection.Add(new TransactionData(ConvertIconToBitmap("ic_check_green_light_18dp"),
                DateTime.Now, "Sent to", ConvertIconToBitmap("ic_call_missed_outgoing_blue_500_18dp"),
                "1H2eFFu5uqvsP3zogQb391eiBe2ge4vYoG", -1.25000000m, "3NHauBuw6faZv8bD66QLiZpdYx2HoemdAs", "BTC", -0.501m, -0.00029783m,
                "6664a73ff2b139c7ffad8bf562ef5ca861248dbc74f3aa9b1c9da516961dcb4", 255, "bytes", 1, "3660 confirmations", 1.09m));
            transactionDataCollection.Add(new TransactionData(ConvertIconToBitmap("ic_check_green_light_18dp"),
                DateTime.Now, "Sent to", ConvertIconToBitmap("ic_call_missed_outgoing_blue_500_18dp"),
                "1JRhxqu5uqvsP3zogQb391eiBe2ge4vYoG", -5.51536000m, "3NHauBuw6faZv8bD66QLiZpdYx2HoemdAs", "BTC", -0.501m, -0.00029783m,
                "6664a73ff2b139c7ffad8bf562ef5ca861248dbc74f3aa9b1c9da516961dcb4", 255, "bytes", 1, "3660 confirmations", 7.69m));
            transactionDataCollection.Add(new TransactionData(ConvertIconToBitmap("ic_check_green_light_18dp"),
                DateTime.Now, "Sent to", ConvertIconToBitmap("ic_call_missed_outgoing_blue_500_18dp"),
                "13wuSqu5uqvsP3zogQb391eiBe2ge4vYoG", -1.25000000m, "3NHauBuw6faZv8bD66QLiZpdYx2HoemdAs", "BTC", -0.501m, -0.00029783m,
                "6664a73ff2b139c7ffad8bf562ef5ca861248dbc74f3aa9b1c9da516961dcb4", 255, "bytes", 1, "3660 confirmations", 1.04m));
            transactionDataCollection.Add(new TransactionData(ConvertIconToBitmap("ic_check_green_light_18dp"),
                DateTime.Now, "Sent to", ConvertIconToBitmap("ic_check_green_light_18dp"),
                "1F5zjP6TSssVeyZf36ADcA3qCnX5jLNcuJi", -1.20013000m, "3NHauBuw6faZv8bD66QLiZpdYx2HoemdAs", "BTC", -0.501m, -0.00029783m,
                "6664a73ff2b139c7ffad8bf562ef5ca861248dbc74f3aa9b1c9da516961dcb4", 255, "bytes", 1, "3660 confirmations", 5.56m));
            transactionDataCollection.Add(new TransactionData(ConvertIconToBitmap("ic_check_green_light_18dp"),
                DateTime.Now, "Sent to", ConvertIconToBitmap("ic_call_missed_outgoing_blue_500_18dp"),
                "1JJR1Xf5uqvsP3zogQb391eiBe2ge4vYoG", -4.83728000m, "3NHauBuw6faZv8bD66QLiZpdYx2HoemdAs", "BTC", -0.501m, -0.00029783m,
                "6664a73ff2b139c7ffad8bf562ef5ca861248dbc74f3aa9b1c9da516961dcb4", 255, "bytes", 1, "3660 confirmations", 12.98m));
            transactionDataCollection.Add(new TransactionData(ConvertIconToBitmap("ic_check_green_light_18dp"),
                DateTime.Now, "Sent to", ConvertIconToBitmap("ic_call_missed_outgoing_blue_500_18dp"),
                "114R1Xf5uqvsP3zogQb391eiBe2ge4vYoG", -1.25000000m, "3NHauBuw6faZv8bD66QLiZpdYx2HoemdAs", "BTC", -0.501m, -0.00029783m,
                "6664a73ff2b139c7ffad8bf562ef5ca861248dbc74f3aa9b1c9da516961dcb4", 255, "bytes", 1, "3660 confirmations", 10.78m));
            transactionDataCollection.Add(new TransactionData(ConvertIconToBitmap("ic_check_green_light_18dp"),
                DateTime.Now, "Sent to", ConvertIconToBitmap("ic_call_missed_outgoing_blue_500_18dp"),
                "1PmjFFu5uqvsP3zogQb391eiBe2ge4vYoG", -1.20013000m, "3NHauBuw6faZv8bD66QLiZpdYx2HoemdAs", "BTC", -0.501m, -0.00029783m,
                "6664a73ff2b139c7ffad8bf562ef5ca861248dbc74f3aa9b1c9da516961dcb4", 255, "bytes", 1, "3660 confirmations", 9.08m));
        }

        private void InitiateCoinInformation()
        {
            coinInformationCollection = new ObservableCollection<CoinInformation>();

            coinInformationCollection.Add(new CoinInformation()
            {
                Name = "BTC",
                Description = "Bitcoin",
                NumberOfCoins = 12.034m,
                Price = 14180m,
                LastUpdate = DateTime.Now,
                PriceChange = -1,
                PricePercentageChange = -0.01,
                ValueChange = -100m,
                ValueChangePercentage = -0.01
            });
            coinInformationCollection.Add(new CoinInformation()
            {
                Name = "ETH",
                Description = "Ethereum",
                NumberOfCoins = 45.674m,
                Price = 1376.10m,
                LastUpdate = DateTime.Now,
                PriceChange = -3,
                PricePercentageChange = -0.22,
                ValueChange = -56.35m,
                ValueChangePercentage = -0.1
            });
            coinInformationCollection.Add(new CoinInformation()
            {
                Name = "XRP",
                Description = "Ripple",
                NumberOfCoins = 1500m,
                Price = 1.98490m,
                LastUpdate = DateTime.Now,
                PriceChange = -0.04375m,
                PricePercentageChange = -2.16,
                ValueChange = -35.42m,
                ValueChangePercentage = -1.05
            });

            coinInformationCollection.Add(new CoinInformation()
            {
                Name = "ZEC",
                Description = "ZCash",
                NumberOfCoins = 253.456m,
                Price = 710.02m,
                LastUpdate = DateTime.Now,
                PriceChange = 6.265m,
                PricePercentageChange = 0.89,
                ValueChange = 4356.89m,
                ValueChangePercentage = 1.02
            });

            coinInformationCollection.Add(new CoinInformation()
            {
                Name = "LTC",
                Description = "Litecoin",
                NumberOfCoins = 100m,
                Price = 256.50m,
                LastUpdate = DateTime.Now,
                PriceChange = -1.49m,
                PricePercentageChange = -0.58,
                ValueChange = -135.42m,
                ValueChangePercentage = -0.35
            });
        }

        private void InitiateSendData()
        {
            sendDataCollection = new ObservableCollection<SendData>();

            sendDataCollection.Add(new SendData(ConvertIconToBitmap("ic_check_green_light_18dp"), DateTime.Now, "Taun Olson", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L",
                1.03m, "BTC"));
            sendDataCollection.Add(new SendData(ConvertIconToBitmap("ic_check_green_light_18dp"), DateTime.Now, "Todd Hoffman", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L",
               .0034m, "LTC"));
            sendDataCollection.Add(new SendData(ConvertIconToBitmap("ic_check_green_light_18dp"), DateTime.Now, "Todd Swasinger", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L",
               2.234m, "ETH"));
        }

        public MemoryStream LoadResource(String name)
        {
            MemoryStream aMem = new MemoryStream();

            var assm = Assembly.GetExecutingAssembly();

            var path = String.Format("CardView.Resources.drawable.{0}", name);

            try
            {
                var aStream = assm.GetManifestResourceStream(path);
                aStream.CopyTo(aMem);
            }
            catch (System.Exception e)
            {
                string str = e.ToString();
            }

            return aMem;
        }

        public Bitmap ConvertIconToBitmap(string iconName)
        {
            return (
                ImageHelper.ToUIImage(new ImageMapStream(LoadResource(iconName + ".png").ToArray())));
        }

    }
}


