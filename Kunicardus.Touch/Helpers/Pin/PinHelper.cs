using System;
using Foundation;
using UIKit;
using Newtonsoft.Json.Schema;
using Kunicardus.Touch.Providers.SqliteProvider;
using Cirrious.CrossCore;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Models.DB;
using System.Linq;

namespace Kunicardus.Touch
{
	public class PinHelper
	{
		public static string UserId { get; set; }

		public static string CorrectPin { get; set; }

		public static bool IsUserLoggedIn { 
			get {
				using (var db = Mvx.Resolve<ILocalDbProvider> ()) {
					var user = db.Get<UserInfo> ().FirstOrDefault ();
					if (user == null) {
						return false;
					} else
						return true;
				}
			}
		}

		public static PinStatus GetPinStatus (string userId)
		{
			nint value = NSUserDefaults.StandardUserDefaults.IntForKey (userId);
			return ((PinStatus)(int)value);
		}

		public static void SetPinStatus (string userId, PinStatus status)
		{
			NSUserDefaults.StandardUserDefaults.SetInt ((int)status, userId);
			NSUserDefaults.StandardUserDefaults.Synchronize ();
		}
	}

	public enum PinStatus : int
	{
		FirstInit = 0,
		ShouldEnterPin = 1,
		NoPin = 2
	}
}

