using System;

namespace Kunicardus.Core.Helpers.AppSettings
{
	public interface IConfigBundlePlugin
	{
		string ConfigText { get; }
	}
}

