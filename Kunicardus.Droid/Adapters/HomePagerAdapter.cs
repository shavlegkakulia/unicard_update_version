using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Support.V4.App;
using Kunicardus.Droid.Fragments;
using MvvmCross.Droid.Support.V4;
using Java.Util;
//using MvvmCross.Droid.Views.Fragments;

namespace Kunicardus.Droid.Adapters
{
	public class HomePagerAdapter : FragmentPagerAdapter
	{
		private HashMap _fragmentList;
		BaseMvxFragment _currentFragment;
		Dictionary<int,BaseMvxFragment> _fragmetns = new Dictionary<int,BaseMvxFragment> ();

		public int PageCount  { get { return 9; } }

		public int OrgId {
			get;
			set;
		}

		public HomePagerAdapter (Android.Support.V4.App.FragmentManager fm, MainView activity)
			: base (fm)
		{
			_fragmentList = new HashMap ();
			InitFragments ();
//			_activity = activity;
		}

		public override Android.Support.V4.App.Fragment GetItem (int position)
		{	
			return (Android.Support.V4.App.Fragment)_fragmetns [position];
		}

		private string FormatTitle (string title)
		{
			return System.String.Join (System.Environment.NewLine, title.Split (new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
		}

		private void InitFragments ()
		{
			for (int i = 0; i < PageCount; i++) {

				MvxFragment _fragment = null;
				switch (i) {
				case 0:
					_fragment = new HomePageFragment ();
					break;
				case 1:
					_fragment = new MyPageFragment ();
					break;
				case 2:
					//should skip
					_fragment = new CardFragment ();
					break;
				case  3:
					_fragment = new BaseCatalogFragment ();
					break;
				case 4:
//					_fragment = new MerchantsFragment ();
					//should skip
					_fragment = new CardFragment ();
					break;
				case 5:
					_fragment = new OrganisationListFragment ();
					break;
				case 6:
					_fragment = new NewsListFragment ();
					break;
				case 7:
					_fragment = new AboutFragment ();
					break;
				case 8:
					_fragment = new SettingsFragment ();
					break;
				}
				if (!_fragmetns.Keys.Contains (i)) {
					_fragmetns.Add (i, (BaseMvxFragment)_fragment);
				}
			}
		}

		public override void DestroyItem (View container, int position, Java.Lang.Object @object)
		{
			_fragmentList.Remove (position);
			base.DestroyItem (container, position, @object);
		}

		public override int Count {
			get { return PageCount; }
		}

		public override int GetItemPosition (Java.Lang.Object objectValue)
		{
			return PositionNone;
		}

		public void ActivateFragment (int position)
		{
			_currentFragment = _fragmetns [position];
			_currentFragment.OnActivate ();
		}


		public BaseMvxFragment GetFragment (int position)
		{
			var frags = _fragmetns.Values.ToList ();
			return frags [position];
		}

	}
}