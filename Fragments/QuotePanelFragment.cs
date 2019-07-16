using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using CoinDataModel;
using SeriousWallet3.DataStructures;
using SeriousWallet3.Models;
using SeriousWallet3.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace SeriousWallet3.Fragments
{
    public class Identifier : Java.Lang.Object
    {
        public string Name { get; set; }
        public Identifier(string name) { Name = name; }
    };

    public class GestureListener : GestureDetector.SimpleOnGestureListener
    {
        public override bool OnDown(MotionEvent e)
        {
            return true;
        }

        public override bool OnDoubleTap(MotionEvent e)
        {
            return true;
        }
    };

    public class QuotePanelFragment : Fragment
    {
        string symbolDoubleTapped;
        decimal sumValue = 0;
        decimal coinTotal = 0;
        static QuotePanelFragment quotePanelFragment;
        System.Timers.Timer webQuoteTimer = new System.Timers.Timer(10000);
        System.Timers.Timer blockChainTimers = new System.Timers.Timer(900000);
        Dictionary<string, CardView> cardViewDict;
        Dictionary<string, decimal> symbolValues = new Dictionary<string, decimal>();
        GridLayout cardViewGridLayout;

        const int CARDVIEW_SYMBOL_TEXT_VIEW_ID = 1;
        const int CARDVIEW_IMAGE_UP_DOWN_ID = 2;
        const int CARDVIEW_DESCRIPTION_TEXT_VIEW_ID = 3;
        const int CARDVIEW_CHANGE_TEXT_VIEW_ID = 4;
        const int CARDVIEW_LAST_UPDATE_TEXT_VIEW_ID = 5;
        const int CARDVIEW_VALUE_LABEL_TEXT_VIEW_ID = 6;
        const int CARDVIEW_VALUE_TEXT_VIEW_ID = 7;
        const int CARDVIEW_NUMBER_OF_COINS_LABEL_TEXT_VIEW_ID = 8;
        const int CARDVIEW_NUMBER_OF_COINS_TEXT_VIEW_ID = 9;
        const int CARDVIEW_PRICE_TEXT_VIEW_ID = 10;

        public int DpToPixels(int dp)
        {
            int pixel = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, Resources.DisplayMetrics);
            return pixel;
        }

        private int GetScreenWidthInPixels()
        {
            var metrics = Resources.DisplayMetrics;
            return (metrics.WidthPixels);
        }

        private int GetScreenHeightInPixels()
        {
            var metrics = Resources.DisplayMetrics;
            return (metrics.HeightPixels);
        }

        RelativeLayout.LayoutParams CreateRelativeLayoutParams()
        {
            return (new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent));
        }

        TextView CreateTextViewEx(string text, Color clr, int textSize, TypefaceStyle ts, Context context)
        {
            TextView tv = new TextView(context);
            tv.Text = text;
            tv.SetTextColor(clr);
            tv.SetTypeface(null, ts);
            tv.SetTextSize(ComplexUnitType.Sp, textSize);

            return tv;
        }

        private async Task<CryptoCompareJSON> GetQuotes()
        {
            CryptoCompareJSON cryptoCompareJSON = null;

            string fsyms = "";

            if (cardViewDict.Keys.Count == 0) return cryptoCompareJSON;

            foreach(string key in cardViewDict.Keys)
            {
                fsyms += key + ",";
            }

            fsyms = fsyms.Remove(fsyms.LastIndexOf(','));

            string url = "https://min-api.cryptocompare.com/data/pricemultifull?fsyms=" + fsyms + "&tsyms=USD";

            //string url = "https://min-api.cryptocompare.com/data/pricemultifull?fsyms=BTC,ETH,XRP,ZEC,LTC&tsyms=USD";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            using (WebResponse response = await request.GetResponseAsync())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    var serializer = new JsonSerializer();
                    StreamReader sr = new System.IO.StreamReader(stream);

                    using (var jst = new JsonTextReader(sr))
                    {
                        cryptoCompareJSON = await Task.Run(() => serializer.Deserialize<CryptoCompareJSON>(jst));

                        return cryptoCompareJSON;
                    }
                }
            }                
         }

        public static QuotePanelFragment NewInstance()
        {
            return ((quotePanelFragment == null) ?
                (quotePanelFragment = new QuotePanelFragment { Arguments = new Bundle() }) :
                quotePanelFragment);
        }

        private CardView CreateCardViewScheme3(string symbol, string description, string numberOfCoinsText, string valueText,
           string priceText, Color chgColor, string chgText, string timeText)
        {
            CardView cardView = new CardView(Context);

            int cardViewWidth;
            cardViewWidth = (GetScreenWidthInPixels() - DpToPixels(6)) / 2;

            GridLayout.LayoutParams gridLayoutParams = new GridLayout.LayoutParams();
            gridLayoutParams.Width = cardViewWidth;
            gridLayoutParams.Height = DpToPixels(144);
            gridLayoutParams.LeftMargin = DpToPixels(2);
            gridLayoutParams.RightMargin = DpToPixels(2);
            gridLayoutParams.TopMargin = DpToPixels(2);

            RelativeLayout rl = new RelativeLayout(cardView.Context);
            cardView.Tag = new Identifier(symbol);
            cardView.AddView(rl);

            Button topColorStripe = new Button(cardView.Context);
            topColorStripe.SetBackgroundColor(CryptoCoinColors.GetCryptoCoinColor(symbol));
            RelativeLayout.LayoutParams lpbb = CreateRelativeLayoutParams();
            lpbb.Width = ViewGroup.LayoutParams.MatchParent;
            lpbb.Height = DpToPixels(6);
            topColorStripe.LayoutParameters = lpbb;
            topColorStripe.Id = 1;
            rl.AddView(topColorStripe);

            TextView symTV = CreateTextViewEx(symbol, Color.Black, 30, TypefaceStyle.Bold, cardView.Context);
            RelativeLayout.LayoutParams lps = CreateRelativeLayoutParams();
            lps.AddRule(LayoutRules.Below, topColorStripe.Id);
            symTV.LayoutParameters = lps;
            symTV.Id = 2;
            rl.AddView(symTV);

            TextView descTV = CreateTextViewEx(description, Color.Gray, 12, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpd = CreateRelativeLayoutParams();
            lpd.AddRule(LayoutRules.RightOf, symTV.Id);
            lpd.LeftMargin = DpToPixels(2);
            lpd.TopMargin = DpToPixels(20);
            descTV.LayoutParameters = lpd;
            descTV.Id = 3;
            rl.AddView(descTV);

            TextView coinsLabelTV = CreateTextViewEx("Coins", Color.Gray, 10, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpnc = CreateRelativeLayoutParams();
            lpnc.AddRule(LayoutRules.Below, symTV.Id);
            lpnc.LeftMargin = DpToPixels(2);
            lpnc.TopMargin = DpToPixels(2);
            coinsLabelTV.LayoutParameters = lpnc;
            coinsLabelTV.Id = 4;
            rl.AddView(coinsLabelTV);

            TextView coinsTV = CreateTextViewEx(numberOfCoinsText, Color.Black, 12, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpcv = CreateRelativeLayoutParams();
            lpcv.AddRule(LayoutRules.RightOf, coinsLabelTV.Id);
            lpcv.AddRule(LayoutRules.AlignTop, coinsLabelTV.Id);
            lpcv.LeftMargin = DpToPixels(5);
            coinsTV.LayoutParameters = lpcv;
            coinsTV.Id = 5;
            rl.AddView(coinsTV);

            TextView valueLabelTV = CreateTextViewEx("Value", Color.Gray, 10, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpvl = CreateRelativeLayoutParams();
            lpvl.AddRule(LayoutRules.Below, coinsTV.Id);
            lpvl.LeftMargin = DpToPixels(2);
            lpvl.TopMargin = DpToPixels(2);
            valueLabelTV.LayoutParameters = lpvl;
            valueLabelTV.Id = 6;
            rl.AddView(valueLabelTV);

            TextView valueTV = CreateTextViewEx(valueText, Color.ParseColor("#EFCC00"), 16, TypefaceStyle.Bold, cardView.Context);
            RelativeLayout.LayoutParams lpv = CreateRelativeLayoutParams();
            lpv.AddRule(LayoutRules.RightOf, valueLabelTV.Id);
            lpv.AddRule(LayoutRules.AlignTop, valueLabelTV.Id);
            lpv.LeftMargin = DpToPixels(2);
            valueTV.LayoutParameters = lpv;
            valueTV.Id = 7;
            rl.AddView(valueTV);

            TextView priceLabelTV = CreateTextViewEx("Price", Color.Gray, 10, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpvp = CreateRelativeLayoutParams();
            lpvp.AddRule(LayoutRules.Below, valueTV.Id);
            lpvp.LeftMargin = DpToPixels(2);
            lpvp.TopMargin = DpToPixels(2);
            priceLabelTV.LayoutParameters = lpvp;
            priceLabelTV.Id = 8;
            rl.AddView(priceLabelTV);

            TextView priceTV = CreateTextViewEx(priceText, chgColor, 14, TypefaceStyle.Bold, cardView.Context);
            RelativeLayout.LayoutParams lpp = CreateRelativeLayoutParams();
            lpp.AddRule(LayoutRules.RightOf, priceLabelTV.Id);
            lpp.AddRule(LayoutRules.AlignTop, priceLabelTV.Id);
            lpp.LeftMargin = DpToPixels(2);
            lpp.TopMargin = DpToPixels(2);
            priceTV.LayoutParameters = lpp;
            priceTV.Id = 9;
            rl.AddView(priceTV);


            TextView chgTV = CreateTextViewEx(chgText, chgColor, 12, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpc = CreateRelativeLayoutParams();
            lpc.AddRule(LayoutRules.Below, priceTV.Id);
            lpc.TopMargin = DpToPixels(2);
            chgTV.LayoutParameters = lpc;
            chgTV.Id = 10;
            rl.AddView(chgTV);

            TextView timeTV = CreateTextViewEx(timeText, Color.Gray, 8, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpt = CreateRelativeLayoutParams();
            lpt.AddRule(LayoutRules.Below, chgTV.Id);
            lpt.AddRule(LayoutRules.AlignLeft, chgTV.Id);
            timeTV.LayoutParameters = lpt;
            timeTV.Id = 11;
            rl.AddView(timeTV);

            cardView.LayoutParameters = gridLayoutParams;

            return cardView;
        }

        private CardView CreateCardViewScheme2(string symbol, string description, string numberOfCoinsText, string valueText,
            string priceText, Color chgColor)
        {
            CardView cardView = new CardView(Context);

            int cardViewWidth;
            cardViewWidth = (GetScreenWidthInPixels() - DpToPixels(6)) / 2;

            GridLayout.LayoutParams gridLayoutParams = new GridLayout.LayoutParams();
            gridLayoutParams.Width = cardViewWidth;
            gridLayoutParams.Height = DpToPixels(170);
            gridLayoutParams.LeftMargin = DpToPixels(2);
            gridLayoutParams.RightMargin = DpToPixels(2);
            gridLayoutParams.TopMargin = DpToPixels(2);

            RelativeLayout rl = new RelativeLayout(cardView.Context);
            cardView.Tag = new Identifier(symbol);
            cardView.AddView(rl);

            Button topColorStripe = new Button(cardView.Context);
            topColorStripe.SetBackgroundColor(Color.Aqua);
            RelativeLayout.LayoutParams lpbb = CreateRelativeLayoutParams();
            lpbb.Width = ViewGroup.LayoutParams.MatchParent;
            lpbb.Height = DpToPixels(6);
            topColorStripe.LayoutParameters = lpbb;
            topColorStripe.Id = 1;
            rl.AddView(topColorStripe);

            TextView symTV = CreateTextViewEx(symbol, Color.Black, 30, TypefaceStyle.Bold, cardView.Context);
            RelativeLayout.LayoutParams lps = CreateRelativeLayoutParams();
            lps.AddRule(LayoutRules.Below, topColorStripe.Id);
            symTV.LayoutParameters = lps;
            symTV.Id = 2;
            rl.AddView(symTV);

            TextView descTV = CreateTextViewEx(description, Color.Gray, 12, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpd = CreateRelativeLayoutParams();
            lpd.AddRule(LayoutRules.RightOf, symTV.Id);
            lpd.LeftMargin = DpToPixels(2);
            lpd.TopMargin = DpToPixels(20);
            descTV.LayoutParameters = lpd;
            descTV.Id = 3;
            rl.AddView(descTV);

            TextView coinsLabelTV = CreateTextViewEx("Coins", Color.Gray, 10, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpnc = CreateRelativeLayoutParams();
            lpnc.AddRule(LayoutRules.Below, symTV.Id);
            lpnc.LeftMargin = DpToPixels(2);
            lpnc.TopMargin = DpToPixels(2);
            coinsLabelTV.LayoutParameters = lpnc;
            coinsLabelTV.Id = 4;
            rl.AddView(coinsLabelTV);

            TextView coinsTV = CreateTextViewEx(numberOfCoinsText, Color.Black, 12, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpcv = CreateRelativeLayoutParams();
            lpcv.AddRule(LayoutRules.Below, coinsLabelTV.Id);
            lpcv.LeftMargin = DpToPixels(2);
            coinsTV.LayoutParameters = lpcv;
            coinsTV.Id = 5;
            rl.AddView(coinsTV);

            TextView valueLabelTV = CreateTextViewEx("Value", Color.Gray, 10, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpvl = CreateRelativeLayoutParams();
            lpvl.AddRule(LayoutRules.Below, coinsTV.Id);
            lpvl.LeftMargin = DpToPixels(2);
            lpvl.TopMargin = DpToPixels(2);
            valueLabelTV.LayoutParameters = lpvl;
            valueLabelTV.Id = 6;
            rl.AddView(valueLabelTV);

            TextView valueTV = CreateTextViewEx(valueText, Color.ParseColor("#EFCC00"), 22, TypefaceStyle.Bold, cardView.Context);
            RelativeLayout.LayoutParams lpv = CreateRelativeLayoutParams();
            lpv.AddRule(LayoutRules.Below, valueLabelTV.Id);
            lpv.LeftMargin = DpToPixels(2);
            valueTV.LayoutParameters = lpv;
            valueTV.Id = 7;
            rl.AddView(valueTV);

            TextView priceLabelTV = CreateTextViewEx("Price", Color.Gray, 10, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpvp = CreateRelativeLayoutParams();
            lpvp.AddRule(LayoutRules.Below, valueTV.Id);
            lpvp.LeftMargin = DpToPixels(2);
            lpvp.TopMargin = DpToPixels(2);
            priceLabelTV.LayoutParameters = lpvp;
            priceLabelTV.Id = 8;
            rl.AddView(priceLabelTV);

            TextView priceTV = CreateTextViewEx(priceText, chgColor, 14, TypefaceStyle.Bold, cardView.Context);
            RelativeLayout.LayoutParams lpp = CreateRelativeLayoutParams();
            lpp.AddRule(LayoutRules.Below, priceLabelTV.Id);
            lpp.LeftMargin = DpToPixels(2);
            lpp.TopMargin = DpToPixels(2);
            priceTV.LayoutParameters = lpp;
            priceTV.Id = 9;
            rl.AddView(priceTV);

            cardView.LayoutParameters = gridLayoutParams;

            return cardView;
        }

        private CardView CreateCardViewScheme1(string symbol, string description, string chgText, string timeText,
            string valueText, string numberOfCoinsText, string priceText, Color chgColor, int changeIcon)
        {
            CardView cardView = new CardView(Context);

            int cardViewWidth;          //175
            cardViewWidth = (GetScreenWidthInPixels() - DpToPixels(6))/2;  //210

            GridLayout.LayoutParams gridLayoutParams = new GridLayout.LayoutParams();
            gridLayoutParams.Width = cardViewWidth;
            gridLayoutParams.Height = DpToPixels(170);
            gridLayoutParams.LeftMargin = DpToPixels(2);
            gridLayoutParams.RightMargin = DpToPixels(2);
            gridLayoutParams.TopMargin = DpToPixels(2);

            RelativeLayout rl = new RelativeLayout(cardView.Context);
            cardView.Tag = new Identifier(symbol);
            cardView.AddView(rl);

            GestureDetector _gestureDetector = new GestureDetector(cardView.Context, new GestureListener());
            _gestureDetector.DoubleTap += (object sender, GestureDetector.DoubleTapEventArgs e) =>
            {
                //apply double tap code here
                //var activity = (MainActivity)this.Activity;
                //activity.DisplayCandleChart(symbolDoubleTapped);
                Toast.MakeText(Context, "Double click", ToastLength.Short).Show();
            };

            cardView.Touch += (object sender, View.TouchEventArgs e) =>
            {
                CardView cv = sender as CardView;

                if (cv != null)
                {
                    symbolDoubleTapped = ((Identifier)cv.Tag).Name;
                }

                _gestureDetector.OnTouchEvent(e.Event);
            };

            Button topColorStripe = new Button(cardView.Context);
            //topColorStripe.SetBackgroundColor(CryptoCoinColors.GetCryptoCoinColor(symbol));
            RelativeLayout.LayoutParams lpbb = CreateRelativeLayoutParams();
            lpbb.Width = ViewGroup.LayoutParams.MatchParent;
            lpbb.Height = DpToPixels(6);
            topColorStripe.LayoutParameters = lpbb;
            topColorStripe.Id = 100;
            rl.AddView(topColorStripe);

            TextView symTV = CreateTextViewEx(symbol, Color.Black, 30, TypefaceStyle.Bold, cardView.Context);
            //symTV.LayoutParameters = CreateRelativeLayoutParams();
            RelativeLayout.LayoutParams lps = CreateRelativeLayoutParams();
            lps.AddRule(LayoutRules.Below, topColorStripe.Id);
            symTV.LayoutParameters = lps;
            symTV.Id = CARDVIEW_SYMBOL_TEXT_VIEW_ID;
            rl.AddView(symTV);

            ImageView iv = new ImageView(cardView.Context);
            RelativeLayout.LayoutParams lpi = CreateRelativeLayoutParams();
            lpi.AddRule(LayoutRules.RightOf, symTV.Id);
            lpi.AddRule(LayoutRules.AlignTop, symTV.Id);
            lpi.TopMargin = DpToPixels(5);
            iv.LayoutParameters = lpi;
            iv.SetImageResource(changeIcon);
            iv.Id = CARDVIEW_IMAGE_UP_DOWN_ID;
            rl.AddView(iv);

            TextView descTV = CreateTextViewEx(description, Color.Gray, 12, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpd = CreateRelativeLayoutParams();
            lpd.AddRule(LayoutRules.Below, symTV.Id);
            lpd.LeftMargin = DpToPixels(2);
            lpd.TopMargin = DpToPixels(-10);
            descTV.LayoutParameters = lpd;
            descTV.Id = CARDVIEW_DESCRIPTION_TEXT_VIEW_ID;
            rl.AddView(descTV);

            TextView chgTV = CreateTextViewEx(chgText, chgColor, 12, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpc = CreateRelativeLayoutParams();
            lpc.AddRule(LayoutRules.RightOf, iv.Id);
            lpc.AddRule(LayoutRules.AlignTop, iv.Id);
            lpc.TopMargin = DpToPixels(2);
            chgTV.LayoutParameters = lpc;
            chgTV.Id = CARDVIEW_CHANGE_TEXT_VIEW_ID;
            rl.AddView(chgTV);

            TextView timeTV = CreateTextViewEx(timeText, Color.Gray, 8, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpt = CreateRelativeLayoutParams();
            lpt.AddRule(LayoutRules.Below, chgTV.Id);
            lpt.AddRule(LayoutRules.AlignLeft, chgTV.Id);
            timeTV.LayoutParameters = lpt;
            timeTV.Id = CARDVIEW_LAST_UPDATE_TEXT_VIEW_ID;
            rl.AddView(timeTV);

            TextView valueLabelTV = CreateTextViewEx("Value", Color.Gray, 10, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpvl = CreateRelativeLayoutParams();
            lpvl.AddRule(LayoutRules.Below, descTV.Id);
            lpvl.LeftMargin = DpToPixels(2);
            lpvl.TopMargin = DpToPixels(17);
            valueLabelTV.LayoutParameters = lpvl;
            valueLabelTV.Id = CARDVIEW_VALUE_LABEL_TEXT_VIEW_ID;
            rl.AddView(valueLabelTV);

            TextView valueTV = CreateTextViewEx(valueText, Color.ParseColor("#EFCC00"), 22, TypefaceStyle.Bold, cardView.Context);
            RelativeLayout.LayoutParams lpv = CreateRelativeLayoutParams();
            lpv.AddRule(LayoutRules.Below, valueLabelTV.Id);
            lpv.LeftMargin = DpToPixels(2);
            lpv.TopMargin = DpToPixels(-3);
            valueTV.LayoutParameters = lpv;
            valueTV.Id = CARDVIEW_VALUE_TEXT_VIEW_ID;
            rl.AddView(valueTV);

            TextView coinsLabelTV = CreateTextViewEx("Number of coins", Color.Gray, 10, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpnc = CreateRelativeLayoutParams();
            lpnc.AddRule(LayoutRules.Below, valueTV.Id);
            lpnc.LeftMargin = DpToPixels(2);
            lpnc.TopMargin = DpToPixels(2);
            coinsLabelTV.LayoutParameters = lpnc;
            coinsLabelTV.Id = CARDVIEW_NUMBER_OF_COINS_LABEL_TEXT_VIEW_ID;
            rl.AddView(coinsLabelTV);

            TextView coinsTV = CreateTextViewEx(numberOfCoinsText, Color.Black, 12, TypefaceStyle.Normal, cardView.Context);
            RelativeLayout.LayoutParams lpcv = CreateRelativeLayoutParams();
            lpcv.AddRule(LayoutRules.Below, coinsLabelTV.Id);
            lpcv.LeftMargin = DpToPixels(2);
            lpnc.TopMargin = DpToPixels(-1);
            coinsTV.LayoutParameters = lpcv;
            coinsTV.Id = CARDVIEW_NUMBER_OF_COINS_TEXT_VIEW_ID;
            rl.AddView(coinsTV);

            TextView priceTV = CreateTextViewEx(priceText, chgColor, 14, TypefaceStyle.Bold, cardView.Context);
            RelativeLayout.LayoutParams lpp = CreateRelativeLayoutParams();
            lpp.AddRule(LayoutRules.Below, descTV.Id);
            lpp.LeftMargin = DpToPixels(2);
            lpp.TopMargin = DpToPixels(-5);
            priceTV.LayoutParameters = lpp;
            priceTV.Id = CARDVIEW_PRICE_TEXT_VIEW_ID;
            rl.AddView(priceTV);

            Android.Widget.Toolbar tb = new Android.Widget.Toolbar(cardView.Context);
            RelativeLayout.LayoutParams lpb = CreateRelativeLayoutParams();
            lpb.AddRule(LayoutRules.Below, coinsTV.Id);
            lpb.TopMargin = -15;
            tb.Tag = new Identifier(symbol);
            tb.InflateMenu(Resource.Menu.CardToolbarMenu);

            tb.MenuItemClick += (sender, e) => {
                Android.Widget.Toolbar tbx = sender as Android.Widget.Toolbar;
                string sym = ((Identifier)tb.Tag).Name;
                var activity = (MainActivity)this.Activity;

                switch (e.Item.ItemId)
                {
                    case (Resource.Id.cardmenu_qrcode):
                        activity.ShowCryptoCurrencyAddressFragment(sym);
                        break;

                    case (Resource.Id.cardmenu_chart):
                        activity.ShowCandleFragment(sym);
                        break;

                    case (Resource.Id.cardmenu_transactions):
                        activity.ShowTransactionFragment(sym);
                        break;

                    case (Resource.Id.cardmenu_send):
                        activity.ShowSendFragment(sym);
                        break;

                    case (Resource.Id.cardmenu_receive):
                        activity.ShowReceiveFragment(sym);
                        break;

                    default:
                        break;
                }
            };

            tb.LayoutParameters = lpb;
            tb.Id = 11;
            rl.AddView(tb);

            cardView.LayoutParameters = gridLayoutParams;

            return cardView;
        }

        string GetPriceStr(double change, double changePct)
        {
            string priceSign = change > 0 ? "+" : "";
            string priceStr = priceSign + System.String.Format("{0:$#,##0.00}", change) + "(" + changePct.ToString("G3") + "%)";

            return priceStr;
        }

        void UpdateNumberofCoins(string symbol, decimal numberofCoins)
        { 
            if (symbol == null) return;

            CardView cardView = cardViewDict[symbol];

            System.Diagnostics.Debug.WriteLine("Update # of coins - symbol: {0}, numberOfcoins {1}", symbol, numberofCoins);
            
            var numberofCoinsView = cardView.FindViewById<TextView>(CARDVIEW_NUMBER_OF_COINS_TEXT_VIEW_ID);
            numberofCoinsView.Text = System.Convert.ToString(numberofCoins);
        }

        void UpdateCardView(string symbol, double price, double change, double changePct, DateTime lastUpdate)
        {
            if (symbol == null) return;

            string priceStr = GetPriceStr(change, changePct);

            CardView cardView = cardViewDict[symbol];

            var textView = cardView.FindViewById<TextView>(CARDVIEW_PRICE_TEXT_VIEW_ID);
            textView.Text = price.ToString("C");
            textView.SetTextColor(change > 0 ? Color.ParseColor("#4CAF50") : Color.Red);

            var timetxtView = cardView.FindViewById<TextView>(CARDVIEW_LAST_UPDATE_TEXT_VIEW_ID);
            timetxtView.Text = lastUpdate.ToString();

            var priceChgView = cardView.FindViewById<TextView>(CARDVIEW_CHANGE_TEXT_VIEW_ID);
            priceChgView.Text = priceStr;
            priceChgView.SetTextColor(change > 0 ? Color.ParseColor("#4CAF50") : Color.Red);

            var numberofCoinsView = cardView.FindViewById<TextView>(CARDVIEW_NUMBER_OF_COINS_TEXT_VIEW_ID);
            decimal numberOfCoins = System.Convert.ToDecimal(numberofCoinsView.Text);

            var valueView = cardView.FindViewById<TextView>(CARDVIEW_VALUE_TEXT_VIEW_ID);
            decimal value = numberOfCoins * (decimal)price;
            valueView.Text = value.ToString("C");

            if (!symbolValues.TryAdd(symbol, value)) symbolValues[symbol] = value;

            ImageView imageView = cardView.FindViewById<ImageView>(CARDVIEW_IMAGE_UP_DOWN_ID);
            imageView.SetImageResource(change > 0 ? Resource.Drawable.ic_arrow_drop_up_green_500_18dp : 
                Resource.Drawable.ic_arrow_drop_down_red_500_18dp);
        }

        void GenerateCardsFromDatabase()
        {
            string currentWallet = DataLayer.GetCurrentWallet();

            List<Coin> coins = DataLayer.GetCoinObjects(currentWallet);

            foreach (Coin c in coins)
            {
                CardView cardView = CreateCardViewScheme1(c.Symbol, c.Description, "", "", "", System.String.Format("{0:n}", c.NumberOfCoins), "",
                    Color.Green, Resource.Drawable.ic_arrow_drop_up_green_500_18dp);

                cardViewDict.Add(c.Symbol, cardView);
                cardViewGridLayout.AddView(cardView);

                UpdateCoinBalance(c.Symbol, c.PrivateKey);
            }
        }

        public void AddCard(string symbol, string description, string privateKey, string publicAddress, string walletName)
        {
            CardView cardView = CreateCardViewScheme1(symbol, description, "", "", "", "0", "",
                    Color.Green, Resource.Drawable.ic_arrow_drop_up_green_500_18dp);

            // @@todo: Add to database
            Coin coin = new Coin();
            coin.WalletName = walletName;
            coin.Symbol = symbol;
            coin.Description = description;
            coin.PrivateKey = privateKey;
            coin.PublicAddress = publicAddress;

            DataLayer.WriteObject(coin);

            cardViewDict.Add(symbol, cardView);

            cardViewGridLayout.AddView(cardView);

            UpdateCoinBalance(symbol, privateKey);
        }

        void GenerateCards()
        {
            DataRepository dataRepository = new DataRepository();
            var activity = (MainActivity)this.Activity;

            sumValue = 0;
            coinTotal = 0;

            foreach (CoinInformation ci in dataRepository.CoinInformationCollection)
            {
                string priceSign = ci.PriceChange > 0 ? "+" : "";

                string priceStr = priceSign + System.String.Format("{0:$#,##0.00}", ci.PriceChange) + "(" + ci.PricePercentageChange.ToString() + "%)";

                CardView cardView = CreateCardViewScheme1(ci.Name, ci.Description, priceStr, ci.LastUpdate.ToString(),
                    ci.TotalValue.ToString("C"), System.String.Format("{0:n}", ci.NumberOfCoins), ci.Price.ToString("C"), ci.PriceChangeColor,
                    ci.PriceChangeIcon);

                //CardView cardView = CreateCardViewScheme3(ci.Name, ci.Description, System.String.Format("{0:n}", ci.NumberOfCoins),
                //    ci.TotalValue.ToString("C"), ci.Price.ToString("C"), ci.PriceChangeColor, priceStr, ci.LastUpdate.ToString());

                sumValue += ci.TotalValue;
                coinTotal += ci.NumberOfCoins;

                cardViewDict.Add(ci.Name, cardView);
                cardViewGridLayout.AddView(cardView);
            }

            activity.SetTotalValue(sumValue);
        }


        private async void UpdateCoinBalance(string symbol, string privateKey)
        {

            var activity = (MainActivity)this.Activity;

            try
            {
                CryptoManagers.CoinBalanceStruct coinBalance =
                    await CryptoClasses.CoinManagerDict[symbol].GetAccountBalance(privateKey);

                Activity.RunOnUiThread(() =>
                {
                    UpdateNumberofCoins(coinBalance.symbol, coinBalance.confirmedBalance);
                });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Coin balance - symbol: {0}, - exception : {1}", symbol, e.ToString());
                activity.ShowErrorFromFragment(e.ToString());
            }
        }

        private void UpdateCoinBalances()
        {
            DataRepository dataRepository = new DataRepository();

            string currentWallet = DataLayer.GetCurrentWallet();

            List<Coin> coins = DataLayer.GetCoinObjects(currentWallet);

            foreach (Coin coin in coins)
            {
                UpdateCoinBalance(coin.Symbol, coin.PrivateKey);
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        private void InitializeBlockChainTimer()
        {
            DataRepository dataRepository = new DataRepository();

            blockChainTimers.Elapsed += async (sender, e) =>
                {
                    foreach (CoinInformation ci in dataRepository.CoinInformationCollection)
                    {
                        //@@todo: get current wallet from database
                        Database.Coin coin = Database.DataLayer.GetCoinObject(ci.Name, "");

                        CryptoManagers.CoinBalanceStruct coinBalance = await CryptoClasses.CoinManagerDict[ci.Name].GetAccountBalance(coin.PrivateKey);

                        //update ui
                    }
                };
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment

            var view = inflater.Inflate(Resource.Layout.quotepanelfragment, container, false);
            cardViewGridLayout = view.FindViewById<GridLayout>(Resource.Id.gridCardLayout);

            var activity = (MainActivity)Activity;

            cardViewDict = new Dictionary<string,CardView>();

            //GenerateCards();

            GenerateCardsFromDatabase();

            UpdateCoinBalances();


            webQuoteTimer.Elapsed += async (sender, e) =>
            {
                CryptoCompareJSON crypotCompareJSON = await GetQuotes();

                if (crypotCompareJSON == null) return;

                RAW raw = crypotCompareJSON.Raw;

                System.Reflection.PropertyInfo[] propertyInfo = raw.GetType().GetProperties();

                foreach (var propInfo in propertyInfo)
                {
                    var value = propInfo.GetValue(raw, null);

                    SYMBOL sym = value as SYMBOL;

                    if (sym == null) continue;

                    string symbol = sym.Usd.FROMSYMBOL;
                    double price = sym.Usd.PRICE;
                    DateTime lastUpdate = DateTimeOffset.FromUnixTimeSeconds(sym.Usd.LASTUPDATE).LocalDateTime;
                    double change = sym.Usd.CHANGEDAY;
                    double changePct = sym.Usd.CHANGEPCTDAY;

                    Activity.RunOnUiThread(() =>
                    {
                        UpdateCardView(symbol, price, change, changePct, lastUpdate);

                        activity.SetTotalValue(symbolValues.Sum(x => x.Value));
                    });
                }
            };

            webQuoteTimer.Start();

            return view;
        }

        public override void OnDestroy()
        {
            webQuoteTimer.Stop();
            base.OnDestroy();
        }
    }
}