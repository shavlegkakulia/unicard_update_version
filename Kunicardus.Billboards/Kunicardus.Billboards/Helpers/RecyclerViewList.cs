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

namespace Kunicardus.Billboards.Helpers
{
    public class RecyclerViewList<T> 
    {
        private List<T> mItems;
        private RecyclerView.Adapter mAdapter;

        public RecyclerViewList()
        {
            mItems = new List<T>();
        }

        public RecyclerView.Adapter Adapter
        {
            get { return mAdapter; }
            set { mAdapter = value; }
        }

        public void Add(T item)
        {
            mItems.Add(item);

            if (Adapter != null)
            {
                Adapter.NotifyItemInserted(0);
            }          
        }

        public void AddAll(List<T> items)
        {
            mItems = items;

            if (Adapter != null)
            {
                Adapter.NotifyItemInserted(mItems.Count-1);
            }
        }

        public List<T> GetItems()
        {
            return mItems;
        }

        public void Remove (int position)
        {
            mItems.RemoveAt(position);

            if (Adapter != null)
            {
                Adapter.NotifyItemRemoved(position);
                //Adapter.NotifyDataSetChanged();
            }
        }

        public void RemoveAll()
        {
            mItems.Clear();

            //if (Adapter != null)
            //{
            //    Adapter.NotifyItemRemoved(0);
            //}
        }

        public T this[int index]
        {
            get { return mItems[index]; }
            set { mItems[index] = value; }
        }

        public int Count
        {
            get { return mItems.Count; }
        }
    }
}