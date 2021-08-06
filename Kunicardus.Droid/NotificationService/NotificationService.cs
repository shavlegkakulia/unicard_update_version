using System;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Content;
using System.Timers;

namespace Kunicardus.Droid
{
	[Service]
	public class NotificationService:Service
	{
		IBinder binder = null;
		const string logTag = "NotificationService ";

		public NotificationService ()
		{
		}

		#region implemented abstract members of Service

		public override Android.OS.IBinder OnBind (Android.Content.Intent intent)
		{
			binder = new NotificationServiceBinder (this);
			return binder;
		}

		#endregion

		Timer timer;


		System.Threading.Timer _timer;

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Debug (logTag, "OnStartCommand: service is running");
			_timer = new System.Threading.Timer ((o) => {
				//todo : check in the server
				SendNotification ();
			}, null, 0, 40000);

			return StartCommandResult.Sticky;
		}

		private void SendNotification ()
		{
//			var nMgr = (NotificationManager)this.GetSystemService (NotificationService);
//			var notification = new Notification ((Resource.Drawable.unicard, "Incoming Dart");
//			var intent = new Intent (this, typeof(MainView));
//
//			//intent.SetComponent(new ComponentName(this, "dart.androidapp.ContactsActivity"));
//			var pendingIntent = PendingIntent.GetActivity (this, 0, intent, 0);
//			notification.SetLatestEventInfo (this, "You've got something to read", "You have received a message", pendingIntent);
//			nMgr.Notify (0, notification);

			Notification.Builder builder = new Notification.Builder (this)
				.SetContentTitle ("Unicard news")
				.SetContentText ("notification from unicard")
				.SetSmallIcon (Resource.Drawable.menu_unicard_logo);

			// Build the notification:
			Notification notification = builder.Build ();
//			notification.on
			// Get the notification manager:
			NotificationManager notificationManager =
				GetSystemService (Context.NotificationService) as NotificationManager;
			
			PendingIntent contentIntent = PendingIntent.GetActivity (this, 0,
				                              new Intent (this, typeof(MainView)), 0);

			// Set the info for the Views that show in the notification panel.
			notification.SetLatestEventInfo (this, "text ",
				"text 2", contentIntent);
			
			// Publish the notification:
			const int notificationId = 0;
			notificationManager.Notify (notificationId, notification);
		}

		public override void OnDestroy ()
		{
			Log.Debug (logTag, "OnDestroy: service is stopping");
			_timer.Dispose ();
		}

	}
}

