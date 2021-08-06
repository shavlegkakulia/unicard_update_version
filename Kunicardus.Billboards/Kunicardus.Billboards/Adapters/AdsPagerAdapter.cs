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
using Android.Support.V4.App;
using Kunicardus.Billboards.Core.Models;
using Kunicardus.Billboards.Fragments;
using Android.Support.V4.View;
using Java.Util;

namespace Kunicardus.Billboards.Adapters
{
    public class AdsPagerAdapter : FragmentStatePagerAdapter
    {
       public List<Android.Support.V4.App.Fragment> _fragmentList = new List<Android.Support.V4.App.Fragment>();

       public AdsPagerAdapter(Android.Support.V4.App.FragmentManager fm) : base(fm)
        {
        }

        public override int Count
        {
            get { return _fragmentList.Count; }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return _fragmentList[position];
        }

        //public override int GetItemPosition(Java.Lang.Object objectValue)
        //{
        //    return PositionNone;
        //}

        //public override float GetPageWidth(int position)
        //{            
        //    return 0.85f;
        //}

        public void RemoveFragment(int index)
        {
            _fragmentList.RemoveAt(index);
        }

        public View GetView(int position)
        {
            return _fragmentList[position].View;
        }

        public void AddFragment(AdsDynamicFragment fragment)
        {
            _fragmentList.Add(fragment);
        }

        public void AddFragmentView(Func<LayoutInflater, ViewGroup, Bundle, View> view)
        {
            _fragmentList.Add(new AdsDynamicFragment(view));
        }

        public void ResetFragments()
        {
            _fragmentList = new List<Android.Support.V4.App.Fragment>();
        }
       
        
        }
}