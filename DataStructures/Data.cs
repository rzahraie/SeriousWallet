using System.Collections.Generic;
using Android.Graphics;
using SeriousWallet3.CryptoManagers;

namespace SeriousWallet3.DataStructures
{
    class SendJSONMail
    {
        public string transactionId { set; get; }
        public string senderEmail { set; get; }
        public string transactionType { set; get; }
        public string coinAmount { set; get; }
        public string senderName { set; get; }
        public string authentication { set; get; }
        public string coinType { set; get; }
        public string transactionAddress { set; get; }
        public string senderPhone { set; get; }
    };

    public class SeriousWalletTransactionMessage
    {
        public string TransactionId { get; set; }
        public string SenderEmail { get; set; }
        public string TransactionType { get; set; }
        public string CoinAmount { get; set; }
        public string SenderName { get; set; }
        public string Authentication { get; set; }
        public string CoinType { get; set; }
        public string TransactionAddress { get; set; }
        public string SenderPhone { get; set; }

        public string ToStringEx()
        {

            return (TransactionId + "," + SenderEmail + "," + TransactionType + "," + CoinAmount + "," + SenderName + "," + Authentication +
                "," + CoinType + "," + TransactionAddress + "," + SenderPhone);
        }
    };

    public class CryptoClasses
    {
        static Dictionary<string, CoinManager> coinManagerDict = new Dictionary<string, CoinManager>();
        static Dictionary<string, (string, Color, CoinManager)> coinInformation = new Dictionary<string, (string, Color, CoinManager)>();
        static List<string> symbolDescriptionList = new List<string>();

        public static Dictionary<string, CoinManager> CoinManagerDict { get => coinManagerDict;}
        public static List<string> SymbolDescriptionList { get => symbolDescriptionList;}

        static CryptoClasses()
        {
            symbolDescriptionList.Add("BTC   - Bitcoin");
            symbolDescriptionList.Add("DASH - Dash");
            symbolDescriptionList.Add("EOS - EOS");
            symbolDescriptionList.Add("ETH - Ethereum");
            symbolDescriptionList.Add("MIOTA - IOTA");
            symbolDescriptionList.Add("NANO - NANO");
            symbolDescriptionList.Add("LTC - Litecoin");
            symbolDescriptionList.Add("OMG - OmiseGO");
            symbolDescriptionList.Add("XEM - NEM");
            symbolDescriptionList.Add("XLM - Stellar Lumens");
            symbolDescriptionList.Add("XRP - Ripple");
            symbolDescriptionList.Add("XMR - Monero");
            

            coinInformation.Add("BTC", ("Bitcoin", Color.Gold, new BitCoinManager()));
            coinInformation.Add("DASH", ("Dash", Color.Red, new CoinManager()));
            coinInformation.Add("EOS", ("EOS", Color.DarkGreen, new CoinManager()));
            coinInformation.Add("ETH", ("Ethereum", Color.SandyBrown, new EthereumCoinManager()));
            coinInformation.Add("IOTA",("MIOTA", Color.Violet, new CoinManager()));
            coinInformation.Add("NANO", ("NANO", Color.DarkOrange, new CoinManager()));
            coinInformation.Add("NEO", ("NEO", Color.Green, new CoinManager()));
            coinInformation.Add("LTC", ("Litecoin", Color.Turquoise, new LiteCoinManager()));
            coinInformation.Add("OMG", ("OmiseGO", Color.Olive, new CoinManager()));
            coinInformation.Add("XEM", ("NEM", Color.Pink, new CoinManager()));
            coinInformation.Add("XLM", ("Stellar Lumens", Color.Purple, new CoinManager()));
            coinInformation.Add("XRP", ("Ripple", Color.DarkGray, new RippleCoinManager()));
            coinInformation.Add("XMR", ("Monero", Color.Yellow, new CoinManager()));
            coinInformation.Add("ZEC", ("ZCash", Color.Honeydew, new ZCashCoinManager()));

            CoinManagerDict.Add("BTC", new BitCoinManager());
            CoinManagerDict.Add("DASH", new CoinManager());
            CoinManagerDict.Add("EOS", new CoinManager());
            CoinManagerDict.Add("ETH", new EthereumCoinManager());
            CoinManagerDict.Add("MIOTA", new CoinManager());
            CoinManagerDict.Add("NANO", new CoinManager());
            CoinManagerDict.Add("LTC", new LiteCoinManager());
            CoinManagerDict.Add("OMG", new CoinManager());
            CoinManagerDict.Add("NEO", new RippleCoinManager());
            CoinManagerDict.Add("XEM", new CoinManager());
            CoinManagerDict.Add("XLM", new LiteCoinManager());
            CoinManagerDict.Add("XRP", new RippleCoinManager());
            CoinManagerDict.Add("XMR", new CoinManager());
            CoinManagerDict.Add("ZEC", new ZCashCoinManager());
        }

       
    }

   public class CryptoCoinColors
   {
        static Dictionary<string, Color> cryptoColorDictionary;
        static Dictionary<string, string> cryptoSymbolNameDictionary;
        

        static CryptoCoinColors()
        {
            cryptoColorDictionary = new Dictionary<string, Color>();
            cryptoSymbolNameDictionary = new Dictionary<string, string>();

            cryptoSymbolNameDictionary.Add("Bitcoin", "BTC");
            cryptoSymbolNameDictionary.Add("Dash", "DASH");
            cryptoSymbolNameDictionary.Add("EOS", "EOS");
            cryptoSymbolNameDictionary.Add("Ethereum", "ETH");
            cryptoSymbolNameDictionary.Add("IOTA", "MIOTA");
            cryptoSymbolNameDictionary.Add("NANO", "NANO");
            cryptoSymbolNameDictionary.Add("Litecoin", "LTC");
            cryptoSymbolNameDictionary.Add("OmiseGO", "OMG");
            cryptoSymbolNameDictionary.Add("NEO", "NEO");
            cryptoSymbolNameDictionary.Add("NEM", "XEM");
            cryptoSymbolNameDictionary.Add("Stellar Lumens", "XLM");
            cryptoSymbolNameDictionary.Add("Ripple", "XRP");
            cryptoSymbolNameDictionary.Add("Monero", "XMR");
            cryptoSymbolNameDictionary.Add("ZCash", "ZEC");

            cryptoColorDictionary.Add("BTC", Color.Gold);
            cryptoColorDictionary.Add("DASH", Color.Red);
            cryptoColorDictionary.Add("EOS", Color.DarkGreen);
            cryptoColorDictionary.Add("ETH", Color.SandyBrown);
            cryptoColorDictionary.Add("MIOTA", Color.Violet);
            cryptoColorDictionary.Add("NANO", Color.DarkOrange);
            cryptoColorDictionary.Add("NEO", Color.Green);
            cryptoColorDictionary.Add("LTC", Color.Turquoise);
            cryptoColorDictionary.Add("OMG", Color.Olive);
            cryptoColorDictionary.Add("XEM", Color.Pink);
            cryptoColorDictionary.Add("XLM", Color.Purple);
            cryptoColorDictionary.Add("XRP", Color.DarkGray);
            cryptoColorDictionary.Add("XMR", Color.Yellow);
            cryptoColorDictionary.Add("ZEC", Color.Honeydew);
        }

        public static Color GetCryptoCoinColor(string cryptoCoinSymbolOrName)
        {
            if (!cryptoColorDictionary.ContainsKey(cryptoCoinSymbolOrName))
                return cryptoColorDictionary[cryptoSymbolNameDictionary[cryptoCoinSymbolOrName]];
            else
                return cryptoColorDictionary[cryptoCoinSymbolOrName];
        }
   }
}