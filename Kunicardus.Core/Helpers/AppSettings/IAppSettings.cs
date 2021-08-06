using System;

namespace Kunicardus.Core.Helpers.AppSettings
{
	public interface IAppSettings
	{
		string LocalDbFileName { get; }

		int LogLevel { get; }

		string LogFilePath { get; }

		string UnicardServiceUrl { get; }
	}
}

