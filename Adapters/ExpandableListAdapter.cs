using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace SeriousWallet3
{
    public class ExpandedMenuModel
    {
        public ExpandedMenuModel()
        {

        }
        public string Name { get; set; }
        public int Image { get; set; }
    }

    class ExpandableListAdapter : BaseExpandableListAdapter
    {
        private Activity context;
        private List<ExpandedMenuModel> listDataHeader; // header titles

        // child data in format of header title, child title
        private Dictionary<ExpandedMenuModel, List<string>> listDataChild;
        ExpandableListView expandList;

        public ExpandableListAdapter(Activity context, List<ExpandedMenuModel> listDataHeader,
            Dictionary<ExpandedMenuModel, List<string>> listChildData, ExpandableListView mView)
        {
            this.context = context;
            this.listDataHeader = listDataHeader;
            this.listDataChild = listChildData;
            expandList = mView;
        }

        public override int GroupCount
        {
            get
            {
                return listDataHeader.Count;
            }
        }

        public override bool HasStableIds
        {
            get
            {
                return false;
            }
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return listDataChild[listDataHeader[groupPosition]][childPosition];
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            int childCount = listDataChild[listDataHeader[groupPosition]].Count;
            return childCount;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild,
            View convertView, ViewGroup parent)
        {
            string childText = (string)GetChild(groupPosition, childPosition);
            if (convertView == null)
            {
                convertView = context.LayoutInflater.Inflate(Resource.Layout.ListSubMenu, null);
            }
            TextView txtListChild = (TextView)convertView.FindViewById(Resource.Id.submenu);
            txtListChild.Text = childText;
            txtListChild.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_info_grey_700_18dp, 0, 0, 0);
            return convertView;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return new JavaObjectWrapper<ExpandedMenuModel>() { Obj = listDataHeader[groupPosition] };
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            ExpandedMenuModel headerTitle = listDataHeader[groupPosition];

            convertView = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.ListHeader, null);
            TextView lblListHeader = (TextView)convertView.FindViewById(Resource.Id.submenu);
            ImageView headerIcon = (ImageView)convertView.FindViewById(Resource.Id.iconimage);
            lblListHeader.Text = headerTitle.Name;
            headerIcon.SetImageResource(headerTitle.Image);

            return convertView;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }

        public class JavaObjectWrapper<T> : Java.Lang.Object
        {
            public T Obj { get; set; }
        }
    }
}