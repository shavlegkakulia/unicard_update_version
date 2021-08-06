using System;
using Kuni.Core.Helpers.Device;
using Android.App;
using System.IO;
using MvvmCross;
using MvvmCross.Plugin.File;

namespace Kunicardus.Droid.Helpers.Device
{
	public class DroidDevicePlugin : IDevice
	{
		#region IDevice implementation

		public string DeviceId {
			get {
				return Android.Provider.Settings.Secure.GetString (Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
			}
		}

		public string DocumentsPath {
			get {
				return System.Environment.GetFolderPath (System.Environment.SpecialFolder.ApplicationData);
			}
		}

		public string DataPath {
			get {
				return Path.Combine (DocumentsPath, "UnicardData");
			}
		}

		public string Platform {
			get {
				return "android";
			}
		}

		public string Type {
			get { return "Android"; }
		}

		public void ResetApplication ()
		{
			IMvxFileStore fileStore = Mvx.Resolve<IMvxFileStore> ();
			fileStore.DeleteFolder (DataPath, true);
			Android.OS.Process.KillProcess (Android.OS.Process.MyPid ());
		}

		#endregion


	}
}

