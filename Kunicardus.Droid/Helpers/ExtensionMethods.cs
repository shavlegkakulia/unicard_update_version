using System;
using System.Text;
using Android.Content;
using Android.Views;
using Android.Widget;
//using MvvmCross.Platforms.Android.Views;
using MvvmCross.ViewModels;
using MvvmCross;
using MvvmCross.Platforms.Android.Views;

namespace Kunicardus.Droid.Helpers
{
	public static class ExtensionMethods
	{
		public static Intent CreateIntent (this IMvxChildViewModelOwner View, IMvxViewModel subViewModel)
		{
			var translator = Mvx.IoCProvider.IoCConstruct<IMvxAndroidViewModelRequestTranslator> ();
			var intentWithKey = translator.GetIntentWithKeyFor ((MvvmCross.ViewModels.IMvxViewModel)subViewModel, MvxViewModelRequest.GetDefaultRequest(typeof(View)));
			View.OwnedSubViewModelIndicies.Add (intentWithKey.Item2);
			return intentWithKey.Item1;
		}

		public static void ClearOwnedSubIndicies (this IMvxChildViewModelOwner View)
		{
			var translator = Mvx.IoCProvider.IoCConstruct<IMvxAndroidViewModelRequestTranslator> ();
			foreach (var ownedSubViewModelIndex in View.OwnedSubViewModelIndicies) {
				translator.RemoveSubViewModelWithKey (ownedSubViewModelIndex);
			}
			View.OwnedSubViewModelIndicies.Clear ();
		}

		public static View GetViewByPosition (this ListView listView, int pos)
		{
			int firstListItemPosition = listView.FirstVisiblePosition;
			int lastListItemPosition = firstListItemPosition + listView.ChildCount - 1;

			if (pos < firstListItemPosition || pos > lastListItemPosition) {
				return listView.Adapter.GetView (pos, null, listView);
			} else {
				int childIndex = pos - firstListItemPosition;
				return listView.GetChildAt (childIndex);
			}
		}

		public static string FormatNumber (this string value)
		{
			if (string.IsNullOrEmpty (value)) {
				return value;
			}
			var builder = new StringBuilder ();
			int count = 0;
			foreach (var c in value) {
				builder.Append (c);
				if ((++count % 4) == 0) {
					builder.Append (' ');
				}
			}
			return builder.ToString ();
		}
	}
}