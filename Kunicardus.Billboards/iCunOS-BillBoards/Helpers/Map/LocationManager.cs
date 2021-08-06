using System;
using CoreLocation;
using UIKit;
using System.Collections.Generic;
using Kunicardus.Billboards.Core.DbModels;
using Kunicardus.Billboards.Core.ViewModels;
using System.Linq;
using Foundation;
using System.Runtime.Remoting.Messaging;

namespace iCunOS.BillBoards
{
	public class LocationManager
	{
		
		#region Variables

		public CLLocationManager locMgr;
		public bool IsFirtTime;
		private int _badges = 1;
		private List<Billboard> _billlboardsForMonitoring = new List<Billboard> ();
		private bool _monitoring = false;
		BillboardsViewModel _viewModel;

		#endregion

		#region Eventhandlers

		public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };
		public event EventHandler<LocationUpdatedEventArgs> FirstLocationUpdated = delegate { };
		public event EventHandler<Billboard> BillboardPassed;
		public event EventHandler<decimal> MinDistanceCalculated;

		#endregion

		#region Ctors

		public LocationManager ()
		{
			this.locMgr = new CLLocationManager ();
			this.IsFirtTime = true;
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				LocMgr.RequestWhenInUseAuthorization ();
			}

			LocationUpdated += BackgroundUpdates;
		}

		#endregion

		#region Props

		public CLLocationManager LocMgr {
			get { return this.locMgr; }
		}

		#endregion

		#region Methods

		public void StartMonitoringRegion (List<Billboard> billboards, BillboardsViewModel viewModel)
		{			
			_monitoring = true;
			if (CLLocationManager.LocationServicesEnabled) {

				if (CLLocationManager.Status != CLAuthorizationStatus.Denied) {					
					// TO STAFFFFFF
					_billlboardsForMonitoring = billboards;
					if (_viewModel == null) {
						_viewModel = viewModel;
					}
				} else {
					UIApplication.SharedApplication.InvokeOnMainThread (() => {
						new UIAlertView ("Ooops!", "App is not authorized to use location data", null, "OK").Show ();
					});
				}					
			} else {
				UIApplication.SharedApplication.InvokeOnMainThread (() => {
					new UIAlertView ("Ooops!", "Location services not enabled, please enable this in your Settings", null, "OK").Show ();
				});
			}
		}

		public void StopMonitoringRegion ()
		{
			_monitoring = false;
			_billlboardsForMonitoring = new List<Billboard> ();
		}

		public void StartLocationUpdates ()
		{
			if (CLLocationManager.LocationServicesEnabled) {
				IsFirtTime = true;
				//set the desired accuracy, in meters
				LocMgr.DesiredAccuracy = CLLocation.AccurracyBestForNavigation;
				LocMgr.LocationsUpdated += LocMgr_LocationsUpdated;		
				LocMgr.StartUpdatingLocation ();
			}
		}

		public void StopLocationUpdates ()
		{
			LocMgr.StopUpdatingLocation ();
			LocMgr.LocationsUpdated -= LocMgr_LocationsUpdated;
			this.IsFirtTime = true;
		}

		#endregion

		#region Events

		void LocMgr_LocationsUpdated (object sender, CLLocationsUpdatedEventArgs e)
		{			
			// fire our custom Location Updated event
			if (this.IsFirtTime) {
				FirstLocationUpdated (this, new LocationUpdatedEventArgs (e.Locations [e.Locations.Length - 1]));
				LocationUpdated (this, new LocationUpdatedEventArgs (e.Locations [e.Locations.Length - 1]));
				this.IsFirtTime = false;
			} else {
				LocationUpdated (this, new LocationUpdatedEventArgs (e.Locations [e.Locations.Length - 1]));
			}
		}

		public void BackgroundUpdates (object sender, LocationUpdatedEventArgs e)
		{
			CLLocation location = e.Location;
			Console.WriteLine ("Altitude: " + location.Altitude + " meters");
			Console.WriteLine ("Longitude: " + location.Coordinate.Longitude);
			Console.WriteLine ("Latitude: " + location.Coordinate.Latitude);
			Console.WriteLine ("Course: " + location.Course);
			Console.WriteLine ("Speed: " + location.Speed);
			if (_monitoring && _billlboardsForMonitoring.Count > 0) {
				foreach (var billboard in _billlboardsForMonitoring) {
					CheckBillboard (billboard, location);
				}
				var minDist = MinDistance (_billlboardsForMonitoring);
				if (MinDistanceCalculated != null)
					MinDistanceCalculated (this, minDist);
			}
		}

		private decimal MinDistance (List<Billboard> billboards)
		{
			decimal minDist = 0;
			if (billboards.Count > 0) {
				minDist = billboards [0].Distance;

				for (int i = 1; i < billboards.Count; i++) {
					if (billboards [i].Distance < minDist) {
						minDist = billboards [i].Distance;
					}
				}
			}
			return minDist;
		}

		private void CheckBillboard (Billboard b, CLLocation l)
		{
			var distInMeters = GetDistanceInMetres ((double)b.Latitude, 
				                   (double)b.Longitude, 
				                   (double)l.Coordinate.Latitude, 
				                   (double)l.Coordinate.Longitude);
			var range = RangeCalculator.InRange ((decimal)l.Course, b.StartBearing, b.EndBearing);
			b.Distance = Convert.ToDecimal (distInMeters);
			if (distInMeters.HasValue && distInMeters.Value <= (double)b.AlertDistance
			    && range) {
				var response = _viewModel.MarkBillboardAsSeen (b.BillboardId, b.AdvertismentId);
				if (response) {
					Console.WriteLine ("Just passed billboard id={0}-{1}-{2}", b.BillboardId, l.Course, distInMeters.Value);
					if (BillboardPassed != null) {
						BillboardPassed (this, b);
					} else {
						CreateNotification (b);
					}
				}
			}
		}

		private void CreateNotification (Billboard b)
		{
			// create the notification
			var notification = new UILocalNotification ();

			// set the fire date (the date time in which it will fire)


			// configure the alert
			notification.AlertAction = "View Alert";
			notification.AlertBody = string.Format ("თქვენ გაიარეთ ბილბორდი: {0}", b.MerchantName);

			// modify the badge
			notification.ApplicationIconBadgeNumber = _badges;

			// set the sound to be the default sound
			notification.SoundName = UILocalNotification.DefaultSoundName;



			// schedule it
			UIApplication.SharedApplication.PresentLocalNotificationNow (notification);
			_badges++;
		}

		public  double? GetDistanceInMetres (double lat1, double lon1, double lat2, double lon2)
		{

			if (lat1 == lat2 && lon1 == lon2)
				return 0;

			var theta = lon1 - lon2;

			var distance = Math.Sin (deg2rad (lat1)) * Math.Sin (deg2rad (lat2)) +
			               Math.Cos (deg2rad (lat1)) * Math.Cos (deg2rad (lat2)) *
			               Math.Cos (deg2rad (theta));

			distance = Math.Acos (distance);
			if (double.IsNaN (distance))
				return null;

			distance = rad2deg (distance);
			distance = distance * 60.0 * 1.1515 * 1609.344;

			return distance;
		}

		private  double deg2rad (double deg)
		{
			return (deg * Math.PI / 180.0);
		}

		private  double rad2deg (double rad)
		{
			return (rad / Math.PI * 180.0);
		}

		#endregion
	}

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

	public class RegionChangedEventArgs: EventArgs
	{
		CLCircularRegion region;

		public RegionChangedEventArgs (CLCircularRegion region)
		{
			this.region = region;
		}

		public CLCircularRegion Region {
			get { return region; }
		}
	}
}

