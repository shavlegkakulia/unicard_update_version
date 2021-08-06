using System;
using CoreLocation;
using UIKit;

namespace Kunicardus.Touch
{
	public class LocationManager
	{
		public CLLocationManager locMgr;
		bool IsFirtTime;

		public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };
		public event EventHandler<LocationUpdatedEventArgs> FirstLocationUpdated = delegate { };

		public LocationManager ()
		{
			this.locMgr = new CLLocationManager ();
			this.IsFirtTime = true;
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				locMgr.RequestWhenInUseAuthorization ();
			}
		}

		public CLLocationManager LocMgr {
			get { return this.locMgr; }
		}

		public void StartLocationUpdates ()
		{
			IsFirtTime = true;
			//set the desired accuracy, in meters
			LocMgr.DesiredAccuracy = 100;
			LocMgr.LocationsUpdated += LocMgr_LocationsUpdated;		
			LocMgr.StartUpdatingLocation ();
		}

		public void StopLocationUpdates ()
		{
			LocMgr.StopUpdatingLocation ();
			LocMgr.LocationsUpdated -= LocMgr_LocationsUpdated;
			this.IsFirtTime = true;
		}

		void LocMgr_LocationsUpdated (object sender, CLLocationsUpdatedEventArgs e)
		{
			// fire our custom Location Updated event
			if (this.IsFirtTime) {
				FirstLocationUpdated (this, new LocationUpdatedEventArgs (e.Locations [e.Locations.Length - 1]));
				this.IsFirtTime = false;
			} else {
				LocationUpdated (this, new LocationUpdatedEventArgs (e.Locations [e.Locations.Length - 1]));
			}
		}

	
	}
}

