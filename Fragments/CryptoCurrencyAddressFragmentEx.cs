using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace SeriousWallet3.Fragments
{
    public class CryptoCurrencyAddressAdapter : FragmentPagerAdapter
    {
        Context ctxt = null;
        string[] titles = { "Private address", "Public address"};
        string privateAddress;
        string publicAddress;

        public CryptoCurrencyAddressAdapter(Context ctxt, FragmentManager mgr, 
            string privateAddress, string publicAddress) : base(mgr)
        {
            this.privateAddress = privateAddress;
            this.publicAddress = publicAddress;
            this.ctxt = ctxt;
        }

        public override int Count { get { return 2; } }

        public override Fragment GetItem(int position)
        {
            return CryptoCurrencyAddressFragmentEx.NewInstance(titles[position], 
                ( position == 0) ? privateAddress : publicAddress);
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(titles[position]);
        }
    }

    class CryptoCurrencyAddressPager : Fragment
    {
        string privateAddress;
        string publicAddress;

        public CryptoCurrencyAddressPager(string privateAddress, string publicAddress) : base()
        {
            this.privateAddress = privateAddress;
            this.publicAddress = publicAddress;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.viewpager, container, false);

            ViewPager viewPager = view.FindViewById<ViewPager>(Resource.Id.cryptoCurrencyPager);

            viewPager.Adapter = BuildAdapter();

            return view;
        }

        private CryptoCurrencyAddressAdapter BuildAdapter()
        {
            return (new CryptoCurrencyAddressAdapter(Context, ChildFragmentManager, privateAddress, publicAddress));
        }
    }

    class CryptoCurrencyAddressFragmentEx : Fragment
    {
        static string title;
        static string address;

        public static CryptoCurrencyAddressFragmentEx NewInstance(string xtitle, string xaddress)
        {
            title = xtitle;
            address = xaddress;

             CryptoCurrencyAddressFragmentEx cryptoCurrencyAddressFragmentEx = 
                new CryptoCurrencyAddressFragmentEx();

            Bundle args = new Bundle();
            args.PutString("Address", address);
            cryptoCurrencyAddressFragmentEx.Arguments = args;

            return cryptoCurrencyAddressFragmentEx;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.cryptocurrencyfragmentex2, container, false);

            TextView textView = view.FindViewById<TextView>(Resource.Id.addressTextView);
            ImageView imageView = view.FindViewById<ImageView>(Resource.Id.addressImageView);

            textView.Text = Arguments.GetString("Address","");
            imageView.SetImageBitmap(Utilities.AndroidUtility.CreateQRCode(textView.Text));

            return view;
        }
    }
}