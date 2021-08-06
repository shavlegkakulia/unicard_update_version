//using System;
//using Cirrious.MvvmCross.Plugins.File;
//using System.Xml.Linq;
//using Kuni.Core.Helpers.Device;
//using System.IO;
//using System.Xml;
//
//namespace Kuni.Core.Helpers.AppSettings
//{
//	public class BaseAppSettings
//	{
//		private readonly string _filePath;
//		private const string TAG = "AppSettings";
//		private readonly IMvxFileStore _fileStore;
//		private readonly IDevice _device;
//		private readonly IConfigReader _configReader;
//
//		public XDocument Settings {
//			get { return _configReader.Settings; }
//		}
//
//		private readonly object writeLock = new object ();
//
//		/// <summary>
//		/// Constructor: Loads and parses configuration xml file using injected FileStore and path
//		/// </summary>
//		/// <param name="store">FileStore injected from IoC</param>
//		/// <param name="logger"></param>
//		public BaseAppSettings (IMvxFileStore store, IDevice device, IConfigReader configReader)
//		{
//			_fileStore = store;
//			_device = device;
//			_configReader = configReader;
////			_filePath = _fileStore.PathCombine (_device.DataPath, Constants.ConfigFileName);
//
//			LoadConfiguration ();
//		}
//
//		#region private helper methods
//
//		/// <summary>
//		/// Loads the configuration file form the devices default file location.
//		/// </summary>
//		/// <exception cref="XmlException">Throws XmlException if file contains invalid Xml</exception>
//		/// <exception cref="FileNotFoundException">Throws FileNotFoundException if configuration file cannot be loaded.</exception>
//		private void LoadConfiguration ()
//		{
//			string configContents;
//
//			var success = _fileStore.TryReadTextFile (_filePath, out configContents);
//
//			if (!success) {
//				throw new FileNotFoundException ("Unable to load configuration file : " + Constants.ConfigFileName);
//			}
//
//			try {
//				_configReader.Load (configContents, _filePath);
//			} catch (XmlException ex) {
//				throw new XmlException (String.Format ("Invalid XML in configuration file: {0}.", Constants.ConfigFileName), ex);
//			}
//		}
//
//		private string GetConfigurationValue (string configName, string defValue, XElement startNode = null)
//		{
//			string ret = defValue;
//			try {
//				ret = _configReader.GetConfigurationValue (configName, startNode);
//			} catch (ArgumentException) {
//			}
//			return ret;
//		}
//
//		private bool GetConfigurationFlag (string nodeName, bool defValue, XElement startNode = null)
//		{
//			bool ret = defValue;
//			try {
//				ret = _configReader.GetConfigurationFlag (nodeName, startNode);
//			} catch (ArgumentException) {
//			}
//			return ret;
//		}
//
//		private void UpdateConfigurationFlag (string configName, bool value, XElement startNode = null)
//		{
//			_configReader.UpdateConfigurationFlag (configName, value, startNode);
//		}
//
//		private void UpdateConfigurationValue (string configName, string value, XElement startNode = null)
//		{
//			_configReader.UpdateConfigurationValue (configName, value, startNode);
//		}
//
//		#endregion
//
//		#region Configuration Properties
//
//		public bool UploadUsingWifi {
//			get { return GetConfigurationFlag ("UploadUsingWifi", true); }
//			set { UpdateConfigurationFlag ("UploadUsingWifi", value); }
//		}
//
//		public string UADeviceToken {
//			get { return GetConfigurationValue ("UADeviceToken", "Not Registered"); }
//			set { UpdateConfigurationValue ("UADeviceToken", value); } 
//		}
//
//		#endregion
//	}
//}
//
