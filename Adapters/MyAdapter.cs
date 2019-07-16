using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;


namespace SeriousWallet3.AddressBookInfo
{ 
    public interface IRecyclerViewClickListener
    {
        void OnClick(View view, int position);
    }

    class RecyclerViewClickListener : Object, IRecyclerViewClickListener
    {
        public delegate void ItemClick(View view, int position);

        public RecyclerViewClickListener(ItemClick itemClickDelegate)
        {
            ItemClickDelegate = itemClickDelegate;
        }

        public ItemClick ItemClickDelegate { get; }

        public void OnClick(View view, int position)
        {
            ItemClickDelegate(view, position);
        }
    }

    class RowViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
    {
        private IRecyclerViewClickListener mListener;
        TextView nameView;
        TextView emailView;
        TextView cryptoAddressView;
        LinearLayout itemLayout;

        public TextView NameView { get => nameView; set => nameView = value; }
        public TextView EmailView { get => emailView; set => emailView = value; }
        public TextView CryptoAddressView { get => cryptoAddressView; set => cryptoAddressView = value; }
        public LinearLayout ItemLayout { get => itemLayout; set => itemLayout = value; }

        public RowViewHolder(View view, IRecyclerViewClickListener listener) : base(view)
        {
            nameView = view.FindViewById<TextView>(Resource.Id.addressName);
            emailView = view.FindViewById<TextView>(Resource.Id.addressEmail);
            cryptoAddressView = view.FindViewById<TextView>(Resource.Id.walletAddress);
            ItemLayout = view.FindViewById<LinearLayout>(Resource.Id.recyclerItemLayout);

            mListener = listener;

            view.SetOnClickListener(this);
        }

        public void OnClick(View view)
        {
            mListener.OnClick(view, AdapterPosition);
        }
    }

    class RecyclerViewAdapter : RecyclerView.Adapter
    {
        private IRecyclerViewClickListener mListener;
        private List<AddressBook> mDataset = new List<AddressBook>();
        List<AddressBook> addressBookList;
        List<AddressBook> copyAddressBookList;
        List<AddressBook> tempAddressBookList;

        public List<AddressBook> AddressBookList { get => addressBookList; set => addressBookList = value; }

        public RecyclerViewAdapter(IRecyclerViewClickListener listener)
        {
            mListener = listener;
        }

        public void UpdateData(List<AddressBook> addressBookList)
        {
            AddressBookList = addressBookList;
            copyAddressBookList = addressBookList;
            tempAddressBookList = new List<AddressBook>();

            NotifyDataSetChanged();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.addressBookLayout, parent, false);
            RowViewHolder adapterBookViewHolder = new RowViewHolder(itemView, mListener);

            return adapterBookViewHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AddressBook addressBook = AddressBookList[position];
            RowViewHolder adapterBookViewHolder = holder as RowViewHolder;

            adapterBookViewHolder.NameView.Text = addressBook.Name;
            adapterBookViewHolder.EmailView.Text = addressBook.Email;
            adapterBookViewHolder.CryptoAddressView.Text = addressBook.CryptoAddress;
        }

        public void Filter(string text)
        {
            tempAddressBookList.Clear();

            foreach (AddressBook ab in copyAddressBookList)
            {
                if (ab.CryptoAddress.ToLower().Contains(text.ToString()) || (ab.Email.ToLower().Contains(text.ToString())) ||
                    (ab.Name.ToLower().Contains(text.ToString())))
                //if (ab.Name.ToLower().Contains(text.ToString()))
                {
                    tempAddressBookList.Add(ab);
                }
            }

            if (text.ToString().Equals(""))
            {
                addressBookList = copyAddressBookList;
                System.Diagnostics.Debug.Write("Insert full address book.");
            }
            else
            {
                addressBookList = tempAddressBookList;
                System.Diagnostics.Debug.Print("Inserted addressbook with {0} entries", tempAddressBookList.Count());
            }

            NotifyDataSetChanged();
        }

        public override int ItemCount => AddressBookList.Count();
    }
}