using System;
using Android.OS;

namespace Kunicardus.Droid
{
	public class NotificationServiceBinder:Binder
	{
		public NotificationService Service { get; private set; }

		public NotificationServiceBinder (NotificationService service)
		{
			this.Service = service;
		}
	}
}

