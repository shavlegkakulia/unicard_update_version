using System;

namespace Kuni.Core.Utilities.Logger
{
	public class LoggingEvent
	{
		public LoggingEvent (DateTime timestamp, string level, string tag, string message)
		{
			Level = level;
			Tag = tag;
			Message = message;
			Timestamp = timestamp;
		}

		public string Tag { get; private set; }

		public string Message { get; private set; }

		public string Level { get; private set; }

		public DateTime Timestamp { get; private set; }
	}
}

