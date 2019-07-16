using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace SeriousWallet3.Fragments
{
    public class CryptoCurrencyAddressFragment : Fragment
    {
        static CryptoCurrencyAddressFragment cryptoCurrencyAddressFragment;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static CryptoCurrencyAddressFragment NewInstance()
        {
            return ((cryptoCurrencyAddressFragment == null) ?
                (cryptoCurrencyAddressFragment = new CryptoCurrencyAddressFragment { Arguments = new Bundle() }) :
                cryptoCurrencyAddressFragment);
        }

        //private void CreateCardViews(View view)
        //{
        //    ImageButton privateButton = view.FindViewById<ImageButton>(Resource.Id.privateAddressButtonEx);
        //    ImageView privateImageView = view.FindViewById<ImageView>(Resource.Id.privateAddressImageView);
        //    ImageView publicImageView = view.FindViewById<ImageView>(Resource.Id.publicAddressImageView);


        //    privateButton.Tag = new Identifier("Down");
        //    privateImageView.Visibility = ViewStates.Gone;
        //    publicImageView.Visibility = ViewStates.Gone;

        //    privateButton.Click += (sender, e) =>
        //    {
        //        string tag = ((Identifier)privateButton.Tag).Name;

        //        if (tag == "Down")
        //        {
        //            ((Identifier)privateButton.Tag).Name = "Up";
        //            privateImageView.Visibility = ViewStates.Visible;
        //            privateButton.SetBackgroundResource(Resource.Drawable.ic_keyboard_arrow_up_black_18dp);
        //        }
        //        else
        //        {
        //            ((Identifier)privateButton.Tag).Name = "Down";
        //            privateImageView.Visibility = ViewStates.Gone;
        //            privateButton.SetBackgroundResource(Resource.Drawable.ic_keyboard_arrow_down_black_18dp);
        //        }
        //    };
        //}

        private void CreateImages(View view)
        {
            ImageButton privateButton = view.FindViewById<ImageButton>(Resource.Id.privateAddressButton);
            ImageButton publicButton = view.FindViewById<ImageButton>(Resource.Id.PublicAddressButton);

            FrameLayout privateFrameLayout = view.FindViewById<FrameLayout>(Resource.Id.privateFrameLayout);
            FrameLayout publicFrameLayout = view.FindViewById<FrameLayout>(Resource.Id.publicFrameLayout);

            privateButton.Tag = new Identifier("Down");
            privateFrameLayout.Visibility = ViewStates.Gone;
            publicFrameLayout.Visibility = ViewStates.Gone;

            privateButton.Click += (sender, e) =>
            {
                string tag = ((Identifier)privateButton.Tag).Name;

                publicFrameLayout.Visibility = ViewStates.Gone;

                if (tag == "Down")
                {
                    ((Identifier)privateButton.Tag).Name = "Up";
                    privateFrameLayout.Visibility = ViewStates.Visible;
                    privateButton.SetBackgroundResource(Resource.Drawable.ic_keyboard_arrow_up_black_18dp);
                }
                else
                {
                    ((Identifier)privateButton.Tag).Name = "Down";
                    privateFrameLayout.Visibility = ViewStates.Gone;
                    privateButton.SetBackgroundResource(Resource.Drawable.ic_keyboard_arrow_down_black_18dp);
                }
            };

            //
            publicButton.Tag = new Identifier("Down");

            publicButton.Click += (sender, e) =>
            {
                string tag = ((Identifier)privateButton.Tag).Name;

                privateFrameLayout.Visibility = ViewStates.Gone;

                if (tag == "Down")
                {
                    ((Identifier)privateButton.Tag).Name = "Up";
                    publicFrameLayout.Visibility = ViewStates.Visible;
                    publicButton.SetBackgroundResource(Resource.Drawable.ic_keyboard_arrow_up_black_18dp);
                }
                else
                {
                    ((Identifier)privateButton.Tag).Name = "Down";
                    publicFrameLayout.Visibility = ViewStates.Gone;
                    publicButton.SetBackgroundResource(Resource.Drawable.ic_keyboard_arrow_down_black_18dp);
                }
            };
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            View view = inflater.Inflate(Resource.Layout.cryptocurrencyaddressfragment, container, false);

            CreateImages(view);

            return view;
        }
    }
}