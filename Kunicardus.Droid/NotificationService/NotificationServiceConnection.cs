using System;
using Android.Content;
using Android.OS;

namespace Kunicardus.Droid
{
	public class ServiceConnectionEventArgs : EventArgs
	{
		public bool IsConnected { get; private set; }

		public ServiceConnectionEventArgs (bool isConnected)
		{
			IsConnected = isConnected;
		}
	}

	public class NotificationServiceConnection: Java.Lang.Object, IServiceConnection
	{
		IBinder binder;

		public NotificationService Service {
			get {
				NotificationServiceBinder lBinder = binder as NotificationServiceBinder;
				return lBinder != null ? lBinder.Service : null;
			}
		}

		public event EventHandler<ServiceConnectionEventArgs> ConnectionChanged = delegate{};

		public void OnServiceConnected (ComponentName name, Android.OS.IBinder service)
		{
			NotificationServiceBinder serviceBinder = service as NotificationServiceBinder;

			if (serviceBinder != null) {
				this.binder = serviceBinder;
				ConnectionChanged (this, new ServiceConnectionEventArgs (true));
			}
		}

		public void OnServiceDisconnected (ComponentName name)
		{
			ConnectionChanged (this, new ServiceConnectionEventArgs (false));
			this.binder = null;
		}
	}
}

