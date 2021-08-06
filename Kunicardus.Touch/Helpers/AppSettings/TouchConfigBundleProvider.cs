using System;
using Kunicardus.Core.Helpers.AppSettings;
using Foundation;
using System.IO;

namespace Kunicardus.Touch.Helpers.AppSettings
{
	public class TouchConfigBundleProvider : IConfigBundlePlugin
	{

		#region IConfigBundlePlugin implementation

		public string ConfigText {
			get {
				var path = NSBundle.MainBundle.PathForResource ("Raw/UnicardConfig.xml", null);
				using (var reader = new StreamReader (path)) {
					return reader.ReadToEnd ();
				}
			}
		}

		#endregion
	}
}

