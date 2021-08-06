using System;
using Kuni.Core;

namespace Kunicardus.Droid
{
	public class GoogleAnalyticsDroid:IGoogleAnalyticsService
	{
		#region IGoogleAnalyticsService implementation

		public void TrackScreen (GAServiceHelper.Pagenames e)
		{
			throw new NotImplementedException ();
		}

		public void TrackScreen (string screenName)
		{
			throw new NotImplementedException ();
		}

		public void TrackEvent (string GAEventCategory, string EventToTrack)
		{
			//GAService.GetGASInstance ().Track_App_Event (GAEventCategory, EventToTrack);
		}

		#endregion
	}
}

