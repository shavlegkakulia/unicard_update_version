using System;
using Kuni.Core.Utilities.Logger;

namespace Kuni.Core.Utilities.Timing
{
	public class TimingUtility : IDisposable
	{
		private readonly ILoggerService _loggerService;
		private readonly string _description;
		private readonly DateTime _startTime;

		private const string TAG = "TimingUtility";

		public TimingUtility (ILoggerService loggerService, string description)
		{
			_description = description;
			_startTime = DateTime.Now;
			_loggerService = loggerService;
		}

		public void Dispose ()
		{
			if (null != _loggerService)
				_loggerService.Debug (TAG, string.Format ("Time for {0} is {1}ms", _description, (DateTime.Now - _startTime).Milliseconds));
		}
	}
}

