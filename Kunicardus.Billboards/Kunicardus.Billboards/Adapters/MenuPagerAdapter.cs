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
using Android.Support.V4.View;
using Android.Support.V4.App;
using Kunicardus.Billboards.Fragments;
using Java.Util;
using Kunicardus.Billboards.Activities;
using Kunicardus.Billboards.Helpers;

namespace Kunicardus.Billboards.Adapters
{
    public class MenuPagerAdapter : FragmentPagerAdapter
    {
        private HashMap _fragmentList;
        private MainActivity _activity;
        CustomViewPager _pager;
        BaseFragment _currentFragment;
        Android.Support.V4.App.FragmentManager _fragmentManager;
        Dictionary<int, BaseFragment> _fragmetns = new Dictionary<int, BaseFragment>();

        public MenuPagerAdapter(Android.Support.V4.App.FragmentManager fm, MainActivity activity, CustomViewPager pager)
            : base(fm)
        {
            _fragmentList = new HashMap();
            _fragmentManager = fm;
            _activity = activity;
            _pager = pager;
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            BaseFragment _fragment = null;

            switch (position)
            {
                case 0:
                    _fragment = new HomePageFragment();
                    break;
                case 1:
                    _fragment = new BillboardsFragment();
                    break;
                case 2:
                    _fragment = new AdsFragment();
                    break;
                case 3:
                    _fragment = new HistoryFragment();
                    break;
                case 4:
                    _fragment = new InfoFragment();
                    break;
                default: 
                    break;
            }
            if (!_fragmetns.Keys.Contains(position))
            {
                _fragmetns.Add(position, (BaseFragment)_fragment);
            }
            return (Android.Support.V4.App.Fragment)_fragment;
        }

        private string FormatTitle(string title)
        {
            return System.String.Join(System.Environment.NewLine, title.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            Java.Lang.Object obj = base.InstantiateItem(container, position);
            _fragmentList.Put(position, obj);
            return obj;
        }

        public override void DestroyItem(View container, int position, Java.Lang.Object @object)
        {
            _fragmentList.Remove(position);
            base.DestroyItem(container, position, @object);
        }

        public override int Count
        {
            get { return 5; }
        }

        public override int GetItemPosition(Java.Lang.Object objectValue)
        {
            return PositionNone;
        }

        public void SetParameter(object parameter = null)
        {
            _parameter = parameter;
        }

        object _parameter;
        public void ActivateFragment(int position)
        {
            _activity.ChangePageTitle(position);
            _currentFragment = _fragmetns[position];
            _currentFragment.OnActivate(_parameter);
        }

        public bool TooglePreviewMode(bool value)
        {
            return ((BillboardsFragment)_fragmetns[1]).PreviewModeEnabled = value; 
        }

        public bool CheckPreviewMode()
        {
            return ((BillboardsFragment)_fragmetns[1]).PreviewModeEnabled;
        }

        public void UpdateDistance(string distance)
        {
            ((HomePageFragment)_fragmetns[0]).UpdateDistance(distance);
        }
    }

    public class ViewPageListenerForActionBar : ViewPager.SimpleOnPageChangeListener
    {
        private ActionBar _bar;

        public ViewPageListenerForActionBar(ActionBar bar)
        {
            _bar = bar;
        }

        public override void OnPageSelected(int position)
        {
            _bar.SetSelectedNavigationItem(position);
        }
    }

    public static class ViewPagerExtensions
    {
        public static ListView _listview;
        public static Activity _context;
        public static ActionBar.Tab GetViewPageTab(this ViewPager viewPager, ActionBar actionBar, string name)
        {
            var tab = actionBar.NewTab();
            tab.SetText(name);
            tab.TabSelected += (o, e) =>
            {
                viewPager.SetCurrentItem(actionBar.SelectedNavigationIndex, false);
            };
            return tab;
        }

    }
}