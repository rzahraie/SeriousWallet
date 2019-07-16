using System;
using System.Collections.Generic;

namespace SeriousWallet3.Models
{
    public class CryptoCompareJSON
    {
        public RAW Raw { get; set; }
    }

    public class RAW
    {
        public BTC BTC { get; set; }
        public ETH ETH { get; set; }
        public XRP XRP { get; set; }
        public ZEC ZEC { get; set; }
        public LTC LTC { get; set; }
        public EOS EOS { get; set; }
        public XMR XMR { get; set; }
    }

    public class DISPLAY
    {
        public BTC BTC { get; set; }
        public ETH ETH { get; set; }
        public XRP XRP { get; set; }
        public ZEC ZEC { get; set; }
        public LTC LTC { get; set; }
        public EOS EOS { get; set; }
        public XMR XMR { get; set; }
    }

    public class BTC : SYMBOL
    {

    }

    public class ETH : SYMBOL
    {

    }

    public class XRP : SYMBOL
    {

    }

    public class ZEC : SYMBOL
    {

    }

    public class LTC : SYMBOL
    {

    }

    public class EOS : SYMBOL
    {

    }

    public class NEO : SYMBOL
    {

    }

    public class XMR : SYMBOL
    {

    }

    public class SYMBOL
    {
        public USD Usd { get; set; }
    }

    public class USD
    {
        public string TYPE { get; set; }
        public string MARKET { get; set; }
        public string FROMSYMBOL { get; set; }
        public string TOSYMBOL { get; set; }
        public int FLAGS { get; set; }
        public double PRICE { get; set; }
        public long LASTUPDATE { get; set; }
        public double LASTVOLUME { get; set; }
        public double LASTVOLUMETO { get; set; }
        public string LASTTRADEID { get; set; }
        public double VOLUMEDAY { get; set; }
        public double VOLUMEDAYTO { get; set; }
        public double VOLUME24HOUR { get; set; }
        public double VOLUME24HOURTO { get; set; }
        public double OPENDAY { get; set; }
        public double HIGHDAY { get; set; }
        public double LOWDAY { get; set; }
        public double OPEN24HOUR { get; set; }
        public double HIGH24HOUR { get; set; }
        public double LOW24HOUR { get; set; }
        public string LASTMARKET { get; set; }
        public double CHANGE24HOUR { get; set; }
        public double CHANGEPCT24HOUR { get; set; }
        public double CHANGEDAY { get; set; }
        public double CHANGEPCTDAY { get; set; }
        public double SUPPLY { get; set; }
        public double MKTCAP { get; set; }
        public double TOTALVOLUME24H { get; set; }
        public double TOTALVOLUME24HTO { get; set; }
    }
}