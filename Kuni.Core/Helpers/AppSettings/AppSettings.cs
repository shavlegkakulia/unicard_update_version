using System;
using Kuni.Core.Helpers.Device;
using System.Xml.Linq;
using MvvmCross.Plugin.File;

namespace Kuni.Core.Helpers.AppSettings
{
	public class AppSettings : IAppSettings
	{
		private readonly IConfigBundlePlugin _bundleProvider;
		private readonly IMvxFileStore _fileStore;

		private IConfigReader _bundleConfigReader;

		public AppSettings (IMvxFileStore store, 
		                    IDevice device, 
		                    IConfigReader configReader, 
		                    IConfigBundlePlugin bundleProvider)
		{
			_bundleProvider = bundleProvider;
			_fileStore = store;
		}


		private IConfigReader BundleConfigReader {
			get {
				if (null == _bundleConfigReader) {
					_bundleConfigReader = new ConfigReader (_fileStore);
					_bundleConfigReader.Load (_bundleProvider.ConfigText);
				}
				return _bundleConfigReader;
			}
		}

		private string GetConfigurationValue (string configName, XElement startNode = null)
		{
			string ret = "";
			try {
				ret = BundleConfigReader.GetConfigurationValue (configName, startNode);
			} catch (ArgumentException) {
			}
			return ret;
		}

		private bool GetConfigurationFlag (string nodeName, XElement startNode = null)
		{
			bool ret = false;
			try {
				ret = BundleConfigReader.GetConfigurationFlag (nodeName, startNode);
			} catch (ArgumentException) {
			}
			return ret;
		}

		private XElement GetConfigurationNode (string nodeName, XElement startNode = null)
		{
			return BundleConfigReader.GetConfigurationNode (nodeName, startNode);
		}

		public string LogFilePath {
			get { return GetConfigurationValue ("LogFilePath"); }
		}

		public int LogLevel {
			get { return  int.Parse (GetConfigurationValue ("LogLevel")); }
		}

		public string LocalDbFileName {
			get { return GetConfigurationValue ("LocalDbFileName"); }
		}

		public bool OfflineModeEnabled {
			get { return GetConfigurationFlag ("OfflineModeEnabled"); }

		}

		public string UnicardServiceUrl {
			get { return GetConfigurationValue ("UnicardServiceUrl"); }
		}
	}
}

