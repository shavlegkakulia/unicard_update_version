using System;
using Kuni.Core.Helpers.AppSettings;
using System.IO;
using Android.App;

namespace Kunicardus.Droid.Helpers.AppSettings
{
	public class DroidConfigBundleProvider : IConfigBundlePlugin
	{
		#region IConfigBundlePlugin implementation

		public string ConfigText {
			get {
				using (var sr = new StreamReader (Application.Context.Resources.OpenRawResource (Resource.Raw.UnicardConfig))) {
					return sr.ReadToEnd ();
				}
			}
		}

		#endregion
	}
}

