using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V7.Preferences;

namespace SeriousWallet3
{
    public class PrefFragment : PreferenceFragmentCompat
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            View view = base.OnCreateView(inflater, container, savedInstanceState);
            view.SetBackgroundColor(Android.Graphics.Color.White);

            return view;
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.Preferences, rootKey);
        }

        public static PrefFragment newInstance()
        {
            PrefFragment fragment = new PrefFragment();
            return fragment;
        }
    }
}