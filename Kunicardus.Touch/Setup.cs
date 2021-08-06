using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Touch.Platform;
using UIKit;
using Cirrious.CrossCore;
using Kunicardus.Core.Helpers.Device;
using Kunicardus.Touch.Helpers.Device;
using Kunicardus.Core.Helpers.AppSettings;
using Kunicardus.Touch.Helpers.AppSettings;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Plugins.Connectivity;
using Kunicardus.Core.Utilities.Logger;
using Kunicardus.Core.UnicardApiProvider;
using Kunicardus.Core.Services.Abstract;
using Kunicardus.Core.Plugins.IUIThreadPlugin;
using Kunicardus.Core;
using Kunicardus.Touch.Plugins.Connectivity;
using Cirrious.MvvmCross.Plugins.File;
using System;
using Kunicardus.Core.Plugins.UIDialogPlugin;
using Kunicardus.Touch.Plugins.UIDialogPlugin;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Kunicardus.Touch.Providers.SqliteProvider;

namespace Kunicardus.Touch
{
	public class Setup : MvxTouchSetup
	{
		
		public Setup (MvxApplicationDelegate applicationDelegate, UIWindow window, IMvxTouchViewPresenter presenter)
			: base (applicationDelegate, presenter)
		{
		}

		protected override IMvxApplication CreateApp ()
		{
			FirstTimeSetup ();
			return new Core.App ();
		}

		protected override IMvxTrace CreateDebugTrace ()
		{
			return new DebugTrace ();
		}

		protected override void InitializeLastChance ()
		{
			base.InitializeLastChance ();

		}

		private void InitializeIoc ()
		{
			
			Mvx.RegisterType<IUIDialogPlugin, TouchUIDialogPlugin> ();
			Mvx.LazyConstructAndRegisterSingleton<IDevice, TouchDevicePlugin> ();
			Mvx.LazyConstructAndRegisterSingleton<IConfigBundlePlugin, TouchConfigBundleProvider> ();
			Mvx.LazyConstructAndRegisterSingleton<IAppSettings, AppSettings> ();
			Mvx.LazyConstructAndRegisterSingleton<IConnectivityPlugin, TouchConnectivityPlugin> ();
			Mvx.LazyConstructAndRegisterSingleton<ILoggerService, CoreLogger> ();
			Mvx.LazyConstructAndRegisterSingleton<IUnicardApiProvider, UnicardApiProvider> ();
			Mvx.LazyConstructAndRegisterSingleton<IAuthService,AuthService> ();
			Mvx.LazyConstructAndRegisterSingleton<IUIThreadPlugin, UIThreadPlugin> ();
			Mvx.LazyConstructAndRegisterSingleton<ICustomSecurityProvider,CustomSecurityProvider> ();
			Mvx.RegisterType<IGoogleAnalyticsService,GAService> ();

			Mvx.RegisterType<ILocalDbProvider, TouchSqliteProvider> ();
			Mvx.RegisterType<IConfigReader, ConfigReader> ();

			CleanUpTempFiles ();
		}

		private void FirstTimeSetup ()
		{
			CopyConfigIfNotExists ();
			InitializeIoc ();
		}

		private void CopyConfigIfNotExists ()
		{
			string emptyXML = @"<Configuration></Configuration>";
			;
			var device = new TouchDevicePlugin ();
			var fileStore = Mvx.Resolve<IMvxFileStore> ();
			//var configBundle = new TouchConfigBundleProvider ();
			var fullPath = fileStore.PathCombine (device.DataPath, Constants.ConfigFileName);

			//in case there is have a file which has the same name as DataFileFolder
			if (fileStore.Exists (device.DataPath))
				fileStore.DeleteFile (device.DataPath);

			fileStore.EnsureFolderExists (device.DataPath);

			if (fileStore.Exists (fullPath))
				return;

			fileStore.WriteFile (fullPath, emptyXML);
		}

		private void CleanUpTempFiles ()
		{
			try {
				var device = Mvx.Resolve<IDevice> ();
				var fileStore = Mvx.Resolve<IMvxFileStore> ();

				var tempPath = fileStore.PathCombine (device.DocumentsPath, "temp");
				if (!fileStore.FolderExists (tempPath))
					return;

				foreach (var item in fileStore.GetFilesIn(tempPath)) {
					fileStore.DeleteFile (item);
				}
				fileStore.DeleteFolder (tempPath, true);
			} catch (Exception e) {
				var logger = Mvx.Resolve<ILoggerService> ();
				logger.Error ("AppSetup", "Error deleting temp: " + e.Message);
				logger.Debug ("AppSetup", e.StackTrace);
			}
		}
	}
}