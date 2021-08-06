using System;
//using Android.Gms.Analytics;
using Android.Content;

namespace Kunicardus.Droid
{
	public class GAService
	{
		////test Tracking Id
		////		public string TrackingId = "UA-67609429-1";
		////live Tracking Id
		//public string TrackingId = "UA-41502131-3";
		////private static GoogleAnalytics GAInstance;
		////private static Tracker GATracker;

		//#region Instantiation ...

		//private static GAService thisRef;

		//private GAService ()
		//{
		//	// no code req'd
		//}

		//public static GAService GetGASInstance ()
		//{
		//	if (thisRef == null)
		//		// it's ok, we can call this constructor
		//		thisRef = new GAService ();
		//	return thisRef;
		//}

		//#endregion

		//public void Initialize (Context AppContext)
		//{
		//	GAInstance = GoogleAnalytics.GetInstance (AppContext);
		//	GAInstance.SetLocalDispatchPeriod (10);
		//	GATracker = GAInstance.NewTracker (TrackingId);
		//	GATracker.EnableExceptionReporting (true);
		//	GATracker.EnableAdvertisingIdCollection (true);
		//	GATracker.EnableAutoActivityTracking (false);

		//	//GAService.GetGASInstance ().Track_App_Event ("Application", "Application Start");
		//}

		//public void Track_App_Page (String PageNameToTrack)
		//{
		//	GATracker.SetScreenName (PageNameToTrack);
		//	GATracker.Send (new HitBuilders.ScreenViewBuilder ().Build ());
		//}

		//public void Track_App_Page (int index)
		//{
		//	Kuni.Core.GAServiceHelper.Pagenames en = (Kuni.Core.GAServiceHelper.Pagenames)index;
		//	GATracker.SetScreenName (en.ToString ());
		//	GATracker.Send (new HitBuilders.ScreenViewBuilder ().Build ());
		//}

		//public void Track_App_Event (String EventToTrack, String GAEventCategory)
		//{
		//	HitBuilders.EventBuilder builder = new HitBuilders.EventBuilder ();
		//	builder.SetCategory (GAEventCategory);
		//	builder.SetAction (EventToTrack);
		//	builder.SetLabel ("AppEvent");

		//	GATracker.Send (builder.Build ());
		//}
	}
}

