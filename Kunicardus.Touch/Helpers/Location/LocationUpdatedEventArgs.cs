using System;
using CoreLocation;

namespace Kunicardus.Touch
{
	public class LocationUpdatedEventArgs : EventArgs
	{
		CLLocation location;

		public LocationUpdatedEventArgs (CLLocation location)
		{
			this.location = location;
		}

		public CLLocation Location {
			get { return location; }
		}
	}
}

