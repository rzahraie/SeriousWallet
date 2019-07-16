using Android.App;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Widget;
using Android.Support.Design.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SeriousWallet3.AddressBookInfo;

namespace SeriousWallet3.Fragments
{
    class ClickListener : Object, IClickListener
    {
        Context context;
        TextView addressTextView;

        //@@todo:change
        public Android.Support.V7.Widget.RecyclerView recyclerView;

        public ClickListener(Context context, TextView tv)
        {
            this.context = context;
            addressTextView = tv;
        }

        public void OnClick(View view, int position)
        {
            
        }

        public void onClick(View view, string text)
        {
            //addressTextView.Text = text;
        }

        public void OnLongClick(View view, int position)
        {
            //Toast.MakeText(context, "Long click " + System.Convert.ToString(position), ToastLength.Long);
        }
    }

    public class AddressBookFragment : Fragment
    {
        private static AddressBookFragment addressBookFragment = null;
        Android.Support.V7.Widget.RecyclerView recyclerView;
        AddressBookAdapter addressBookAdapter;
        TextInputEditText searchEditView;

        public static AddressBookFragment NewInstance()
        {
            return ((addressBookFragment == null) ?
                (addressBookFragment = new AddressBookFragment { Arguments = new Bundle() }) :
                addressBookFragment);
        }

        public void CreateRecyclerView(View view)
        {
            recyclerView = view.FindViewById<Android.Support.V7.Widget.RecyclerView>(Resource.Id.addressRecyclerView);

            Android.Support.V7.Widget.DividerItemDecoration dividerItemDecoration = new
                Android.Support.V7.Widget.DividerItemDecoration(recyclerView.Context, Android.Support.V7.Widget.LinearLayoutManager.Vertical);

            List<AddressBook> addressBookList = new List<AddressBook>();

            addressBookAdapter = new AddressBookAdapter(addressBookList);

            recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(Context));
            recyclerView.SetAdapter(addressBookAdapter);
            recyclerView.AddItemDecoration(dividerItemDecoration);
            recyclerView.AddOnItemTouchListener(new RecyclerViewOnItemTouchListener(Context, recyclerView, new ClickListener(Context, searchEditView)));

            addressBookList.Add(new AddressBook("Steven Howchenhowser", "showchenhowdress@gmail.com", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"));
            addressBookList.Add(new AddressBook("Taun Olson", "taun@seriouscoin.io", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"));
            addressBookList.Add(new AddressBook("Todd Hoffman", "toddh@seriouscoin.io", "PKM90iag9jJgTHD1VXjvLCEnZuQ3rTCF0Q"));
            addressBookList.Add(new AddressBook("Todd Swasinger", "todds@seriouscoin.io", "20M90iag9jJgTHD1VXjvLCEnZuQ3rTCF0Q"));
            addressBookList.Add(new AddressBook("Ramin Zahraie", "ramin@seriouscoin.io", "58M90iag9jJgTHD1VXjvLCEnZuQ3rTCH1X"));
            addressBookList.Add(new AddressBook("Steve Smith", "ssmith@gmail.com", "58M90iag9jJgTHD1VXjvLCEnZuQ3rTCH2H"));
            addressBookList.Add(new AddressBook("William Johnson", "wjohnson@gmail.com", "58M90iag9jJgTHD1VXjvLCEnZuQ3rTCH27T"));

            addressBookAdapter.NotifyDataSetChanged();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            View view = inflater.Inflate(Resource.Layout.addressBookLayoutEx2, container, false);

            searchEditView = view.FindViewById<TextInputEditText>(Resource.Id.searchEdit);
            searchEditView.TextChanged += SearchEditView_TextChanged;

            CreateRecyclerView(view);

            return view;
        }

        private void SearchEditView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            addressBookAdapter.Filter(e.Text.ToString());
        }
    }
}