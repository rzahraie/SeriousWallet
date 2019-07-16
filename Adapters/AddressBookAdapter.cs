using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System;

namespace SeriousWallet3.AddressBookInfo
{
    #region OBSELETE_DELETE
    //class TextWatcher : Java.Lang.Object, ITextWatcher
    //{
    //    AddressBookAdapter addressBookAdapter;
    //    List<AddressBook> tempaddressBookList;
    //    List<AddressBook> originalAdressBookList;

    //    public TextWatcher(ref AddressBookAdapter addressBookAdapter)
    //    {
    //        tempaddressBookList = new List<AddressBook>();
    //        this.addressBookAdapter = addressBookAdapter;
    //        originalAdressBookList = addressBookAdapter.AddressBookList;
    //    }

    //    public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
    //    {

    //    }

    //    public void AfterTextChanged(IEditable s)
    //    {
    //        foreach(AddressBook ab in originalAdressBookList)
    //        {
    //            if (ab.CryptoAddress.ToLower().Contains(s.ToString()) || (ab.Email.ToLower().Contains(s.ToString())) || 
    //                (ab.Name.ToLower().Contains(s.ToString())))
    //            {
    //                tempaddressBookList.Add(ab);
    //            }
    //        }

    //        if (s.ToString().Equals(""))
    //            addressBookAdapter.UpdateAddressBook(originalAdressBookList);
    //        else
    //            addressBookAdapter.UpdateAddressBook(tempaddressBookList);
    //    }

    //    public void OnTextChanged(ICharSequence s, int start, int before, int count)
    //    {

    //    }
    //};

    //class RecyclerViewOnItemTouchListener : Java.Lang.Object, RecyclerView.IOnItemTouchListener
    //{
    //    int prevSelectedRecyclerItem;
    //    TextView addressTextView;

    //    public RecyclerViewOnItemTouchListener(TextView tv) : base()
    //    {
    //        addressTextView = tv;
    //    }

    //    private void HandleItemSelection(RecyclerView rv, MotionEvent e)
    //    {
    //        (rv.FindViewHolderForAdapterPosition(prevSelectedRecyclerItem) as
    //            AdapterBookViewHolder)?.ItemLayout.SetBackgroundColor(Android.Graphics.Color.Transparent);

    //        View view = rv.FindChildViewUnder(e.GetX(), e.GetY());
    //        int position = rv.GetChildAdapterPosition(view);
    //        var adapter = rv.GetAdapter();
    //        var holder = rv.FindViewHolderForAdapterPosition(position);

    //        string txt = (holder as AdapterBookViewHolder).CryptoAddressView.Text;
    //        (holder as AdapterBookViewHolder).ItemLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#d5d5d5"));

    //        //Toast.MakeText(context, "Address selected: " + txt, ToastLength.Short).Show();
    //        addressTextView.Text = txt;

    //        prevSelectedRecyclerItem = position;
    //    }

    //    public bool OnInterceptTouchEvent(RecyclerView recyclerView, MotionEvent @event)
    //    {
    //        //recyclerView.ScrollState == RecyclerView.ScrollStateIdle
    //        if (recyclerView.ScrollState != RecyclerView.ScrollStateDragging && @event.Action == MotionEventActions.Down)
    //        {
    //            HandleItemSelection(recyclerView, @event);
    //            return true;
    //        }

    //        return false;
    //    }

    //    public void OnRequestDisallowInterceptTouchEvent(bool disallow)
    //    {
    //        // nothing here
    //    }

    //    public void OnTouchEvent(RecyclerView recyclerView, MotionEvent @event)
    //    {
    //        //Toast.MakeText(context, "Touch was invoked", ToastLength.Short).Show();
    //    }

    //    public void OnDoubleTap(RecyclerView recyclerView, MotionEvent @event)
    //    {
    //        //Toast.MakeText(context, "Double Tap", ToastLength.Short).Show();
    //    }
    //};

    //class AddressBookAdapterEx : RecyclerView.Adapter
    //{
    //    List<AddressBook> addressBookList;
    //    List<AddressBook> copyAddressBookList;
    //    List<AddressBook> tempAddressBookList;

    //    public AddressBookAdapterEx(List<AddressBook> addressBookList)
    //    {
    //        AddressBookList = addressBookList;
    //        copyAddressBookList = addressBookList;
    //        tempAddressBookList = new List<AddressBook>();
    //    }

    //    public List<AddressBook> AddressBookList { get => addressBookList; set => addressBookList = value; }

    //    public event EventHandler<int> eventHandler;

    //    public override int ItemCount => AddressBookList.Count();

    //    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    //    {
    //        AddressBook addressBook = AddressBookList[position];
    //        AdapterBookViewHolder adapterBookViewHolder = holder as AdapterBookViewHolder;

    //        if (holder == adapterBookViewHolder && adapterBookViewHolder != null)
    //        {
    //            adapterBookViewHolder.NameView.Text = addressBook.Name;
    //            adapterBookViewHolder.EmailView.Text = addressBook.Email;
    //            adapterBookViewHolder.CryptoAddressView.Text = addressBook.CryptoAddress;
    //        }
    //    }

    //    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    //    {
    //        View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.addressBookLayout, parent, false);
    //        AdapterBookViewHolder adapterBookViewHolder = new AdapterBookViewHolder(itemView, clickEvent);

    //        return adapterBookViewHolder;
    //    }

    //    public void clickEvent(int position)
    //    {
    //        eventHandler?.Invoke(this, position);
    //    }
    //};
    #endregion

    public delegate void InsertAddress(string address);
    public delegate void InsertName(string name);
    public delegate void InsertCryptoAddress(string cryptoAddress);

    class AddressBook
    {
        string name;
        string email;
        string cryptoAddress;
        bool selected;

        public AddressBook(string name, string email, string cryptoAddress)
        {
            Name = name;
            Email = email;
            CryptoAddress = cryptoAddress;
            Selected = false;
        }

        public string Name { get => name; set => name = value; }
        public string Email { get => email; set => email = value; }
        public string CryptoAddress { get => cryptoAddress; set => cryptoAddress = value; }
        public bool Selected { get => selected; set => selected = value; }
    };

    public interface IClickListener
    {
        void OnClick(View view, int position);
        void onClick(View view, string text);
        void OnLongClick(View view, int position);
    }

    class RecyclerViewOnItemTouchListener : Java.Lang.Object, RecyclerView.IOnItemTouchListener, GestureDetector.IOnGestureListener
    {
        private IClickListener clickListener;
        private GestureDetector gestureDetector;
        private RecyclerView recyclerView;
        int prevSelectedRecyclerItem;

        InsertAddress insertAddressDelegate;
        InsertName insertNameDelegate;
        InsertCryptoAddress insertCryptoAddressDelegate;

        public RecyclerViewOnItemTouchListener(Context context, RecyclerView rv, IClickListener clickListener)
        {
            this.clickListener = clickListener;
            gestureDetector = new GestureDetector(context, this);
            recyclerView = rv;
        }

        public RecyclerViewOnItemTouchListener(Context context, RecyclerView rv, 
            InsertAddress insertAddress, InsertName insertName, InsertCryptoAddress insertCryptoAddress, IClickListener clickListener)
        {
            this.clickListener = clickListener;
            gestureDetector = new GestureDetector(context, this);
            insertAddressDelegate = insertAddress;
            insertNameDelegate = insertName;
            insertCryptoAddressDelegate = insertCryptoAddress;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY) { return false; }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY) { return false; }

        public void OnShowPress(MotionEvent e) { }

        public void OnRequestDisallowInterceptTouchEvent(bool disallow) { }

        public void OnTouchEvent(RecyclerView recyclerView, MotionEvent @event) { }

        public bool OnDown(MotionEvent e) { return false; }

        public bool OnSingleTapUp(MotionEvent e) { return true; }

        public void OnLongPress(MotionEvent e)
        {
            View child = recyclerView.FindChildViewUnder(e.GetX(), e.GetY());

            if (child != null && clickListener != null && gestureDetector.OnTouchEvent(e))
            {
                clickListener.OnLongClick(child, recyclerView.GetChildAdapterPosition(child));
            }
        }

        public bool OnInterceptTouchEvent(RecyclerView recyclerView, MotionEvent e)
        {
            recyclerView.FindViewHolderForAdapterPosition(prevSelectedRecyclerItem)?.ItemView.SetBackgroundColor(Color.Transparent);

            View child = recyclerView.FindChildViewUnder(e.GetX(), e.GetY());

            if (child != null && clickListener != null && gestureDetector.OnTouchEvent(e))
            {
                int position = recyclerView.GetChildAdapterPosition(child);
                var holder = recyclerView.FindViewHolderForAdapterPosition(position);

                holder.ItemView.SetBackgroundColor(Color.ParseColor("#d5d5d5"));

                string txt = (holder as AdapterBookViewHolder).NameView.Text;

                insertAddressDelegate((holder as AdapterBookViewHolder).EmailView.Text);
                insertCryptoAddressDelegate((holder as AdapterBookViewHolder).CryptoAddressView.Text);
                insertNameDelegate((holder as AdapterBookViewHolder).NameView.Text);

                clickListener.OnClick(child, position);

                clickListener.onClick(child, txt);

                prevSelectedRecyclerItem = position;
            }

            return false;
        }
    };

    class AdapterBookViewHolder : RecyclerView.ViewHolder
    {
        TextView nameView;
        TextView emailView;
        TextView cryptoAddressView;
        LinearLayout itemLayout;

        public TextView NameView { get => nameView; set => nameView = value; }
        public TextView EmailView { get => emailView; set => emailView = value; }
        public TextView CryptoAddressView { get => cryptoAddressView; set => cryptoAddressView = value; }
        public LinearLayout ItemLayout { get => itemLayout; set => itemLayout = value; }

        public AdapterBookViewHolder(View view) : base(view)
        {
            nameView = view.FindViewById<TextView>(Resource.Id.addressName);
            emailView = view.FindViewById<TextView>(Resource.Id.addressEmail);
            cryptoAddressView = view.FindViewById<TextView>(Resource.Id.walletAddress);
            ItemLayout = view.FindViewById<LinearLayout>(Resource.Id.recyclerItemLayout);
        }
    };

    class AddressBookAdapter : RecyclerView.Adapter
    {
        List<AddressBook> addressBookList;
        List<AddressBook> copyAddressBookList;
        List<AddressBook> tempAddressBookList;
       
        AdapterBookViewHolder adapterBookViewHolder;

        public List<AddressBook> AddressBookList { get => addressBookList; set => addressBookList = value; }

        public override int ItemCount => AddressBookList.Count();

        public void SetSelected(int position, bool selected)
        {
            if ((position >= 0) && (position <= AddressBookList.Count))
                AddressBookList[position].Selected = selected;    
        }

        public AddressBookAdapter(List<AddressBook> addressBookList)
        {
            AddressBookList = addressBookList;
            copyAddressBookList = addressBookList;
            tempAddressBookList = new List<AddressBook>();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.addressBookLayout, parent, false);
            AdapterBookViewHolder adapterBookViewHolder = new AdapterBookViewHolder(itemView);

            return adapterBookViewHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AddressBook addressBook = AddressBookList[position];
            adapterBookViewHolder = holder as AdapterBookViewHolder;

            adapterBookViewHolder.NameView.Text = addressBook.Name;
            adapterBookViewHolder.EmailView.Text = addressBook.Email;
            adapterBookViewHolder.CryptoAddressView.Text = addressBook.CryptoAddress;
        }

        public void Filter(string text)
        {
            var lowerText = text.ToLower();

            tempAddressBookList.Clear();

            foreach (AddressBook ab in copyAddressBookList)
            {
                if (ab.CryptoAddress.ToLower().Contains(lowerText.ToString()) || (ab.Email.ToLower().Contains(lowerText.ToString())) ||
                    (ab.Name.ToLower().Contains(lowerText.ToString())))
                {
                    tempAddressBookList.Add(ab);
                }
            }

            if (lowerText.ToString().Equals(""))
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
    };
}