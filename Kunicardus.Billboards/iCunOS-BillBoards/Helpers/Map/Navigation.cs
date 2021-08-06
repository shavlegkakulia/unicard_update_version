using System;
using Foundation;

namespace iCunOS.BillBoards
{
	public static class Navigation
	{
		public static bool Active {
			get { 
				return NSUserDefaults.StandardUserDefaults.BoolForKey ("navigation_mode_on"); 
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetBool (value, "navigation_mode_on"); 
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			}
		}
	}
}

