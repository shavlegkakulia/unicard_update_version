using System;
using Kunicardus.Core.Helpers.AppSettings;
using Kunicardus.Core.Helpers.Device;
using MvvmCross.Plugins.File;

namespace Kunicardus.Core.Utilities.Logger
{
	public class CoreLogger : ILoggerService
	{
		private readonly LogLevel _logLevel = LogLevel.Debug;
		//Default log level
		private readonly IMvxFileStore _fileStore;
		private readonly IAppSettings _settings;
		private readonly IDevice _device;

		private readonly string _logFilePath;
		private readonly object _writingLogFile = new object ();
		private string _logFileContent = string.Empty;

		public CoreLogger (IMvxFileStore store, IAppSettings appSettings, IDevice device)
		{
			_fileStore = store;
			_settings = appSettings;
			_device = device;

			_logLevel = (LogLevel)_settings.LogLevel;
			_logFilePath = _fileStore.PathCombine (_device.DocumentsPath, _settings.LogFilePath);
			_fileStore.WriteFile (_logFilePath, string.Empty);
		}

		// TODO: replace with more accurate high resolution timestamp

		public event EventHandler<LoggingEvent> LogHandler;

		private void DoLog (string level, string tag, string message, object[] args)
		{
			var m = string.Format (message, args);
			var timestamp = DateTime.Now;  

			var logMessage = string.Format ("({0}) tag:{1}: level:{2} message:{3}", timestamp, tag, _logLevel, m);

			System.Diagnostics.Debug.WriteLine (logMessage);
			AppendToLogFile (logMessage);

			if (null != LogHandler) {
				LogHandler (this, new LoggingEvent (timestamp, level, tag, m));
			}
		}

		private void AppendToLogFile (string logMessage)
		{
			_logFileContent += "\n" + logMessage;

			if (string.IsNullOrEmpty (_logFilePath))
				return;

			lock (_writingLogFile) {
				_fileStore.WriteFile (_logFilePath, _logFileContent);
			}
		}

		public virtual void Verbose (string tag, string message, params object[] args)
		{
			if (_logLevel <= LogLevel.Verbose)
				DoLog ("Verbose", tag, message, args);
		}

		public virtual void Debug (string tag, string message, params object[] args)
		{
			if (_logLevel <= LogLevel.Debug)
				DoLog ("Debug", tag, message, args);
		}

		public virtual void Info (string tag, string message, params object[] args)
		{
			if (_logLevel <= LogLevel.Info)
				DoLog ("Info", tag, message, args);
		}

		public virtual void Warn (string tag, string message, params object[] args)
		{
			if (_logLevel <= LogLevel.Warn)
				DoLog ("Warn", tag, message, args);
		}

		public virtual void Error (string tag, string message, params object[] args)
		{
			if (_logLevel <= LogLevel.Error)
				DoLog ("Error", tag, message, args);
		}

		public virtual void Fatal (string tag, string message, params object[] args)
		{
			if (_logLevel <= LogLevel.Fatal)
				DoLog ("Fatal", tag, message, args);
		}
	}
}

