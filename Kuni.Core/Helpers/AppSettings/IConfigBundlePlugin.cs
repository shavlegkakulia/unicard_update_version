using System;

namespace Kuni.Core.Helpers.AppSettings
{
	public interface IConfigBundlePlugin
	{
		string ConfigText { get; }
	}
}

