using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Android.OS;
using Sqo;

namespace SeriousWallet3.Database
{
    public class User
    {
        public int OID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EMail { get; set; }
    }

    public class CurrentWallet
    {
        public string WalletName { get; set; }
    }

    public class Coin
    {
        public string WalletName { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
        public decimal NumberOfCoins { get; set; }
        public string PrivateKey { get; set; }
        public string PublicAddress { get; set; }
    }

    public class CoinData
    {
        public string Symbol { get; set; }
        public string Description { get; set; }
        public decimal LastValue { get; set; }
        public DateTime LastUpdate { get; set; }
        public string PriceHistoryFile { get; set; }
        public string ColorCode { get; set; }
    }

    public class Signatory
    {
        public string WalletName { get; set; }
        public string Name { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string EMailAddress { get; set; }
        public string PublicAddress { get; set; }
    }

    public enum TransactionTypeEnum
    {
        SEND,
        RECEIVE
    }
   
    public class TransactionData
    {
        public string WalletName { get; set; }
        public string Symbol { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }
        public DateTime Date { get; set; }
        public string To { get; set; }
        public string ToAddress { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }
        public decimal Coins { get; set; }
        public decimal TotalOut { get; set; }
        public string Currency { get; set; }
        public decimal AmountInCurrency { get; set; }
        public decimal MinerFee { get; set; }
        public decimal Change { get; set; }
        public string ChangeAddress { get; set; }
        public int HashCode { get; set; }
        public int VirtualSize { get; set; }
        public bool HasWitness { get; set; }
        public uint Version { get; set; }
    }

    class SiaqodbFactory
    {
        public static string siaoqodbPath;
        private static Siaqodb instance;
        ///<summary>
        /// Set the path where the database file will reside
        ///</summary>
        public static void SetPath(string path)
        {
            siaoqodbPath = path;
        }
        ///<summary>
        /// Acquire an instance of the database engine
        ///</summary>
        public static Siaqodb GetInstance()
        {
            if (instance == null)
            {
                instance = new Siaqodb(siaoqodbPath);
            }
            return instance;
        }
        ///<summary>
        /// Close the database
        ///</summary>
        public static void CloseDatabase()
        {
            if (instance != null)
            {
                instance.Close();
                instance = null;
            }
        }
    }

    public class DataLayer
    {
        static bool firstTimeUse = false;
        static Siaqodb siaqodb;

        public static bool FirstTimeUse { get => firstTimeUse; set => firstTimeUse = value; }

        static DataLayer()
        {
            SiaqodbConfigurator.SetLicense("kyZxVRSJUn2jEcKwJ1J1Gb5kiA+fy/i2mgpxPY1rFgs=");

            SiaqodbFactory.SetPath(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
            siaqodb = SiaqodbFactory.GetInstance();

            if (siaqodb.Count<User>() == 0) firstTimeUse = true;
        }

        public static void WriteObject(Object obj)
        {
            siaqodb.StoreObject(obj);
        }

        public static List<Signatory> GetSignatories(string walletName)
        {
            var q = from Signatory s in siaqodb where s.WalletName == walletName select s;

            List<Signatory> list = new List<Signatory>();

            foreach (Signatory s in q)
            {
                list.Add(s);
            }

            return list;
        }

        public static T GetSingleObject<T>()
        {
            T tt = default(T);

            var q = from T t in siaqodb select t;

            foreach(T t in q)
            {
                tt = t;
            }

            return tt;
        }

        public static ObservableCollection<CoinDataModel.ChartDataModel> GetTransactionChartData()
        {
            CoinDataModel.DataRepository dataRepository = new CoinDataModel.DataRepository();
            ObservableCollection<CoinDataModel.TransactionData> transdata = dataRepository.TransactionDataCollection;

            ObservableCollection<CoinDataModel.ChartDataModel> cm = new ObservableCollection<CoinDataModel.ChartDataModel>();

            int i = 0;

            foreach (CoinDataModel.TransactionData ct in transdata)
            {
                int multiply = (ct.Type == "Sent to") ? -1 : 1;

                cm.Add(new CoinDataModel.ChartDataModel(i.ToString(), multiply*System.Convert.ToDecimal(ct.Coins)));

                i++;
            }

            return cm;
        }

        public static ObservableCollection<CoinDataModel.Wallet> GetWalletsEx()
        {
            ObservableCollection<CoinDataModel.Wallet> wallets = new ObservableCollection<CoinDataModel.Wallet>();

            var q = from CoinDataModel.Wallet w in siaqodb select w;

            foreach (CoinDataModel.Wallet w in q)
            {
                wallets.Add(w);
            }

            return wallets;
        }

        public static List<CoinData> GetCoinDataObject(string symbol)
        {
            var q = from CoinData cd in siaqodb where cd.Symbol == symbol select cd;

            List<CoinData> coinDataList = new List<CoinData>();

            foreach (CoinData cd in q)
            {
                coinDataList.Add(cd);
            }

            return coinDataList;
        }

        public static List<Coin> GetCoinObjects(string walletName = null)
        {
            List<Coin> coins = new List<Coin>();

            var q = (walletName == null) ? from Coin c in siaqodb select c : 
                from Coin c in siaqodb where c.WalletName == walletName select c;

            foreach (Coin c in q)
            {
                coins.Add(c);
            }

            return coins;
        }

        //@@todo: problematic
        public static Coin GetCoinObject(string symbol, string walletName = null)
        {
            Coin coin = null;

            var q = from Coin c in siaqodb where c.Symbol == symbol select c;

            foreach(Coin c in q)
            {
                coin = c;
            }

            //@@todo: remove dummy data. use data returned by coin structure
            //string privateAddress = "cTb6CtukM9rB2QirndzDddfsY9avfEUbesTdA9E2q9emkcFPPKd9";
            //string publicAddress = "mtddQEtjueBuFWVeSy63CMJPY9GQXuJfpL";

            if (coin == null) coin = new Coin();

            //coin.PrivateKey = privateAddress;
            //coin.PublicAddress = publicAddress;

            return coin;
        }

        public static User GetUserObject()
        {
            User us = null;

            var q = from User user in siaqodb select user;

            foreach(User u in q)
            {
                us = u;
            }

            return us;
        }

        public static bool DoesWalletExist(string walletName)
        {
            var q = from CoinDataModel.Wallet w in siaqodb where w.WalletName == walletName select w;

            bool b = (q.Count() != 0) ? true : false;

            return b;
        }

        public static string GetCurrentWallet()
        {
            var q = from CurrentWallet cw in siaqodb select cw;
            string currentWallet = "";

            foreach (CurrentWallet cw in q)
            {
                currentWallet = cw.WalletName;
            }

            return currentWallet;
        }

        public static void SetCurrentWallet(string walletName)
        {
            CurrentWallet currentWallet = null; ;

            var q = from CurrentWallet cw in siaqodb select cw;

           foreach (CurrentWallet cw in q)
            {
                currentWallet = cw;
            }

            if (currentWallet == null) currentWallet = new CurrentWallet();

            currentWallet.WalletName = walletName;

            WriteObject(currentWallet);
        }

        public static async Task<ObservableCollection<T>> GetObjectsAsync<T>()
        {
            var q = from T t in siaqodb select t;

            List<T> list = (List<T>)(await q.ToListAsync());

            ObservableCollection<T> obs = new ObservableCollection<T>(list);

            return obs;
        }

        public static void DeleteObject<T>(T t)
        {
            siaqodb.Delete(t);
        }
    }
}