﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace SeriousWallet3.Fragments
{
    public class SendFragmentEx : Fragment
    {
        private static SendFragmentEx sendFragmentEx = null;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static SendFragmentEx NewInstance()
        {
            return ((sendFragmentEx == null) ?
               (sendFragmentEx = new SendFragmentEx { Arguments = new Bundle() }) : sendFragmentEx);        

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.sendfragment, container, false);

            return view;
        }
    }
}