using System;
using Kunicardus.Core.Helpers.Device;
using UIKit;
using Foundation;
using System.IO;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.File;
using ObjCRuntime;

namespace Kunicardus.Touch.Helpers.Device
{
	public class TouchDevicePlugin : IDevice
	{
		private string _documentsPath;

		public string DeviceId {
			get { return UIDevice.CurrentDevice.IdentifierForVendor.AsString (); }
		}

		public string DocumentsPath {
			get { return _documentsPath ?? (_documentsPath = SystemDocumentsPath ()); }
		}

		private string SystemDocumentsPath ()
		{
			int SystemVersion = Convert.ToInt16 (UIDevice.CurrentDevice.SystemVersion.Split ('.') [0]);
			if (SystemVersion >= 8) {
				return NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0].Path;
			}
			return Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
		}

		public string DataPath {
			get {
				return Path.Combine (DocumentsPath, "UnicardData");
			}
		}

		public string Platform {
			get { return "ios"; }
		}

		public string Type {
			get {
				return !UIDevice.CurrentDevice.Model.ToLowerInvariant ().Contains ("ipad") ? "iPhone" : "iPad";
			}
		}

		public void ResetApplication ()
		{
			var fileStore = Mvx.Resolve<IMvxFileStore> ();
			fileStore.DeleteFolder (DataPath, true);
			UIApplication.SharedApplication.PerformSelector (new Selector ("terminateWithSuccess"), null, 0.0f);
		}
	}
}

