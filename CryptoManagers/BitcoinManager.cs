using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.Protocol;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace SeriousWallet3.CryptoManagers
{
    public struct CoinBalanceStruct
    {
        public string symbol;
        public decimal balance;
        public decimal confirmedBalance;
        public Coin[] coins;
    };

   public class CoinManager
   { 
        public CoinManager()
        { }

        public delegate void ReportResult(string message);

        public virtual async Task<CoinBalanceStruct> GetAccountBalance(string privateAddress)
        {
            CoinBalanceStruct bitcoinBalanceStruct = new CoinBalanceStruct();
            bitcoinBalanceStruct.confirmedBalance = await Task.Run(() => 1.0m);

            return bitcoinBalanceStruct;
        }

        public virtual async Task SendCoinTransaction(string fromPrivateAddress,
            string to, string sendPublicAddress, decimal amountToSend, decimal amountInCurrency, string currency, string walletName, 
            ReportResult reportResult)
        {
           
        }
    }

    class EthereumCoinManager : CoinManager
    {
        public EthereumCoinManager()
        {

        }

        public override async Task<CoinBalanceStruct> GetAccountBalance(string privateAddress)
        {
            CoinBalanceStruct ethereumBalanceStruct = new CoinBalanceStruct();
            ethereumBalanceStruct.symbol = "ETH";
            ethereumBalanceStruct.confirmedBalance = await Task.Run(() => 5.67m);

            return ethereumBalanceStruct;
        }
    }

    class RippleCoinManager : CoinManager
    {
        public RippleCoinManager()
        {

        }

        public override async Task<CoinBalanceStruct> GetAccountBalance(string privateAddress)
        {
            CoinBalanceStruct rippleBalanceStruct = new CoinBalanceStruct();
            rippleBalanceStruct.symbol = "XRP";
            rippleBalanceStruct.confirmedBalance = await Task.Run(() => 1500m);

            return rippleBalanceStruct;
        }
    }

    class ZCashCoinManager : CoinManager
    {
        public ZCashCoinManager()
        {

        }

        public override async Task<CoinBalanceStruct> GetAccountBalance(string privateAddress)
        {
            CoinBalanceStruct zcashBalanceStruct = new CoinBalanceStruct();
            zcashBalanceStruct.symbol = "ZEC";
            zcashBalanceStruct.confirmedBalance = await Task.Run(() => 25.45m);

            return zcashBalanceStruct;
        }
    }

    class LiteCoinManager : CoinManager
    {
        public LiteCoinManager()
        {

        }

        public override async Task<CoinBalanceStruct> GetAccountBalance(string privateAddress)
        {
            CoinBalanceStruct litecoinBalanceStruct = new CoinBalanceStruct();
            litecoinBalanceStruct.symbol = "LTC";
            litecoinBalanceStruct.confirmedBalance = await Task.Run(() => 52.56m);

            return litecoinBalanceStruct;
        }
    }

    class BitCoinManager : CoinManager
    {
        public BitCoinManager()
        {

        }

        public override async Task<CoinBalanceStruct> GetAccountBalance(string privateAddress)
        {
            CoinBalanceStruct sbalance = new CoinBalanceStruct();
            sbalance.symbol = "BTC";

            var secret = new BitcoinSecret(privateAddress);
            var privKey = secret.PrivateKey;
            var pubKey = privKey.PubKey.GetAddress(Network.TestNet);

            QBitNinjaClient client = new QBitNinjaClient(Network.TestNet);
            var balance = await Task.Run(() => client.GetBalance(pubKey, true).Result);

            sbalance.balance = 0.0M;
            sbalance.confirmedBalance = 0.0M;

            var unSpendCoins = new List<Coin>();
            var unSpendCoinsConfirmed = new List<Coin>();

            foreach (var operation in balance.Operations)
            {
                unSpendCoins.AddRange(operation.ReceivedCoins.Select(coin => coin as Coin));
                if (operation.Confirmations > 0) unSpendCoinsConfirmed.AddRange(operation.ReceivedCoins.Select(coin => coin as Coin));
            }

            sbalance.balance = unSpendCoins.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC));
            sbalance.confirmedBalance = unSpendCoinsConfirmed.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC));
            sbalance.coins = unSpendCoinsConfirmed.ToArray();

            return sbalance;
        }

        private void WriteTransactionToDatabase(Database.TransactionData transactionSendData)
        {
            Database.DataLayer.WriteObject(transactionSendData);
        }

        public override async Task SendCoinTransaction(string fromPrivateAddress, 
            string sendPublicAddress, string to, 
            decimal amountToSend, decimal amountInCurrency, string currency, string walletName, 
            ReportResult reportResult)
        {
            CoinBalanceStruct sbalance = await GetAccountBalance(fromPrivateAddress);

            var txBuilder = new TransactionBuilder();
            var secret = new BitcoinSecret(fromPrivateAddress);
            var toAmount = new Money(amountToSend, MoneyUnit.BTC);
            var minerFee = new Money(0.001m, MoneyUnit.BTC);

            Database.TransactionData transactionSendData = new Database.TransactionData();

            transactionSendData.Symbol = "BTC";
            transactionSendData.WalletName = walletName;
            transactionSendData.TransactionType = Database.TransactionTypeEnum.SEND;
            transactionSendData.AmountInCurrency = amountInCurrency;
            transactionSendData.Coins = amountToSend;
            transactionSendData.Change = sbalance.confirmedBalance - amountToSend;
            transactionSendData.To = to;
            transactionSendData.ToAddress = sendPublicAddress;
            transactionSendData.ChangeAddress = secret.GetAddress().ToString();
            //@@todo
            transactionSendData.Currency = currency;
            transactionSendData.Date = DateTime.Now;
   
            if (amountToSend >= sbalance.confirmedBalance)
            {
                transactionSendData.Status = "Fail";
                transactionSendData.Response = "Bitcoin transaction failed: There are not enough coins in your bitcoin account.";

                WriteTransactionToDatabase(transactionSendData);

                reportResult(transactionSendData.Response);

                return;
            }

            var tx = txBuilder.AddCoins(sbalance.coins)
                       .AddKeys(secret)
                       .Send(BitcoinAddress.Create(sendPublicAddress, Network.TestNet), toAmount)
                       .SendFees(minerFee)
                       .SetChange(secret.GetAddress())
                       .BuildTransaction(true);

            if (!txBuilder.Verify(tx))
            {
                transactionSendData.Status = "Fail";
                transactionSendData.Response = "Bitcoin transaction failed: Unable to verify bitcoin transaction.";

                WriteTransactionToDatabase(transactionSendData);

                reportResult(transactionSendData.Response);

                return;
            }
            
            QBitNinjaClient client = new QBitNinjaClient(Network.TestNet);

            BroadcastResponse broadcastResponse = await Task.Run(() => client.Broadcast(tx).Result);

            if (!broadcastResponse.Success)
            {
                reportResult("Error broadcasting transaction " + broadcastResponse.Error.ErrorCode.ToString() + " : " +
                    broadcastResponse.Error.Reason.ToString());
                transactionSendData.Response = broadcastResponse.Error.Reason;
            }
            else
            {
                reportResult("Transaction was broadcasted successfully.");
                transactionSendData.Response = "Success";
            }

            transactionSendData.Status = broadcastResponse.Success ? "Success" : "Fail";
            
            transactionSendData.HasWitness = tx.HasWitness;
            transactionSendData.MinerFee = minerFee.ToDecimal(MoneyUnit.BTC);
            transactionSendData.TotalOut = tx.TotalOut.ToDecimal(MoneyUnit.BTC);
            transactionSendData.Version = tx.Version;
            transactionSendData.HashCode = tx.GetHashCode();
            transactionSendData.VirtualSize = tx.GetVirtualSize();

            WriteTransactionToDatabase(transactionSendData);

            //using (var node = NBitcoin.Protocol.Node.Connect(Network.TestNet, ""))
            //{
            //    node.VersionHandshake();
            //    node.SendMessage(new NBitcoin.Protocol.InvPayload(tx));
            //    node.SendMessage(new NBitcoin.Protocol.TxPayload(tx));
            //    System.Threading.Thread.Sleep(500);
            //    node.Disconnect();
            //}

        }
    }
}