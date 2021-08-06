using System;

namespace Kuni.Core.Utilities.Logger
{
	public interface ILoggerService
	{
		void Verbose (string tag, string message, params object[] args);

		void Debug (string tag, string message, params object[] args);

		void Info (string tag, string message, params object[] args);

		void Warn (string tag, string message, params object[] args);

		void Error (string tag, string message, params object[] args);

		void Fatal (string tag, string message, params object[] args);
	}
}

