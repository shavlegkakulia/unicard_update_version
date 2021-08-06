using System;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using MvvmCross.Plugins.File;

namespace Kunicardus.Core.Helpers.AppSettings
{
	public class ConfigReader : IConfigReader
	{
		private readonly IMvxFileStore _fileStore;

		private XDocument _settings;
		private object _writeLock = new object ();
		private string _filePath;

		public ConfigReader (IMvxFileStore fileStore)
		{
			_fileStore = fileStore;
		}

		public XDocument Settings { 
			get { return _settings; }
		}

		#region Loading

		public void Load (string text)
		{
			_settings = XDocument.Parse (text);
		}

		public void Load (string text, string filePath)
		{
			_settings = XDocument.Parse (text);
			_filePath = filePath;
		}

		#endregion

		#region Retrieving

		public string GetConfigurationValue (string configName, XElement startNode = null)
		{
			startNode = GetStartNode (startNode);

			var configNode = GetConfigurationNode (configName, startNode);

			var attribute = configNode.Attribute ("value");

			if (null == attribute) {
				throw new XmlException (String.Format ("Invalid XML - invalid or missing value attribute in node: {0}.", configNode.Name));
			}

			return attribute.Value;
		}

		public bool GetConfigurationFlag (string nodeName, XElement startNode = null)
		{
			startNode = GetStartNode (startNode);

			var configNode = GetConfigurationNode (nodeName, startNode);

			var attribute = configNode.Attribute ("flag");

			if (null == attribute) {
				throw new ArgumentException (String.Format ("Invalid XML - invalid or missing flag attribute in node: {0}.", configNode.Name));
			}

			bool flag;

			var success = Boolean.TryParse (attribute.Value, out flag);

			if (!success) {
				throw new ArgumentException (string.Format ("Invalid XML - cannot parse out boolean value: {0} in node: {1}", attribute.Name, configNode.Name));
			}

			return flag;
		}

		public XElement GetConfigurationNode (string nodeName, XElement startNode = null)
		{
			startNode = GetStartNode (startNode);
			var configNode = startNode.Element (nodeName);

			if (configNode == null) {
				throw new ArgumentException (String.Format ("Invalid configuration name: {0}.", nodeName));
			}

			return configNode;
		}

		public XElement GetStartNode (XElement startNode)
		{
			return startNode ?? (startNode = _settings.Root);
		}

		#endregion

		#region Updating


		public void UpdateConfigurationFlag (string configName, bool value, XElement startNode = null)
		{
			var configNode = GetNodeForWrite (configName, startNode);
			configNode.SetAttributeValue ("flag", value.ToString ());
			UpdateConfigurationFile ();
		}

		public void UpdateConfigurationValue (string configName, string value, XElement startNode = null)
		{
			var configNode = GetNodeForWrite (configName, startNode);

			configNode.SetAttributeValue ("value", value.ToString ());

			UpdateConfigurationFile ();
		}

		public void UpdateConfigurationFile ()
		{
			if (string.IsNullOrWhiteSpace (_filePath))
				throw new IOException ("No file to write specified");

			try {
				lock (_writeLock) {
					_fileStore.WriteFile (_filePath, _settings.ToString ());
				}
			} catch (Exception ex) {
				throw new IOException (string.Format ("Unable to update configuration file: {0}", Constants.ConfigFileName), ex);
			}
		}

		public XElement GetNodeForWrite (string configName, XElement startNode = null)
		{
			startNode = GetStartNode (startNode);

			var configNode = startNode.Element (configName);

			if (configNode != null)
				return configNode;

			configNode = new XElement (configName);
			_settings.Root.Add (configNode);

			return configNode;
		}

		#endregion
	}
}

