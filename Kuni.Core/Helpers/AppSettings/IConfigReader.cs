using System;
using System.Xml.Linq;

namespace Kuni.Core.Helpers.AppSettings
{
	public interface IConfigReader
	{
		XDocument Settings { get; }

		void Load (string text);

		void Load (string text, string filePath);

		string GetConfigurationValue (string configName, XElement startNode = null);

		bool GetConfigurationFlag (string nodeName, XElement startNode = null);

		XElement GetConfigurationNode (string nodeName, XElement startNode = null);

		void UpdateConfigurationValue (string configName, string value, XElement startNode = null);

		void UpdateConfigurationFlag (string configName, bool value, XElement startNode = null);

	}
}

