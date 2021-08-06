using System;
using Kunicardus.Core;
using GoogleAnalytics.iOS;

namespace Kunicardus.Touch
{
	public class GAService : IGoogleAnalyticsService
	{
		private static GAService Instance;

		public static GAService GetGAServiceInstance ()
		{
			if (Instance == null)
				Instance = new GAService ();
			return Instance;
		}

		public void TrackScreen (GAServiceHelper.Pagenames e)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, e.ToString ());
			GAI.SharedInstance.DefaultTracker.Send (GAIDictionaryBuilder.CreateScreenView ().Build ());
		}

		public void TrackScreen (string screenName)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, screenName);
			GAI.SharedInstance.DefaultTracker.Send (GAIDictionaryBuilder.CreateScreenView ().Build ());
		}

		public void TrackEvent (string eventName, string actionName)
		{
			GAI.SharedInstance.DefaultTracker.Send (
				GAIDictionaryBuilder.CreateEvent (eventName,
					actionName, GAServiceHelper.Events.AppEvent, null).Build ());
		}
	}
}

