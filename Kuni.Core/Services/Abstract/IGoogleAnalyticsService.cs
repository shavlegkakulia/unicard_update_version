using System;

namespace Kuni.Core
{
	public interface IGoogleAnalyticsService
	{
		void TrackEvent (String GAEventCategory, String EventToTrack);

		void TrackScreen (GAServiceHelper.Pagenames e);

		void TrackScreen (string screenName);
	}
}

