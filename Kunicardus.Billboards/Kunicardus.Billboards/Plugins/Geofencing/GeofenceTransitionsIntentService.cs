using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Location;
using Android.Util;
using Android.Support.V7.App;
using Android.Graphics;
using Kunicardus.Billboards.Activities;
using Kunicardus.Billboards.Core.Enums;
using Kunicardus.Billboards.Core.Services.Abstract;
using Kunicardus.Billboards.Core.DbModels;
using Kunicardus.Billboards.Core.Services;
using Kunicardus.Billboards.Core.ViewModels;
using System.Threading.Tasks;
using Android.Provider;

namespace Kunicardus.Billboards.Plugins.Geofencing
{
    [Service]
    public class GeofenceTransitionsIntentService : IntentService
    {
        protected const string TAG = "geofence-transitions-service";
        //public static Direction Direction { get; set; }
        public static decimal Direction { get; set; }
        public static decimal Speed { get; set; }
        public static Dictionary<IGeofence,CustomGeofence> GeofenceDictionary { get; set; }
        public static BillboardsViewModel ViewModel { get; set; }
        public static CustomGeofence TriggeredMarker { get; set; }

        public GeofenceTransitionsIntentService() : base(TAG)
        {
        }

        public static CustomGeofence GetTriggeredMarker()
        {
            return TriggeredMarker;
        }

        protected override void OnHandleIntent(Intent intent)
        {
            GeofencingEvent geofencingEvent = GeofencingEvent.FromIntent(intent);
            #region Error Logging
            if (geofencingEvent.HasError)
            {
                string errorMessage = GeofenceErrorMessages.GetErrorString(ApplicationContext, geofencingEvent.ErrorCode);
                Log.Debug("Geofencing Message: ", errorMessage);
                return;
            }
            #endregion

            var geofenceTransition = geofencingEvent.GeofenceTransition;

            if (geofenceTransition == Android.Gms.Location.Geofence.GeofenceTransitionEnter && Speed>15)
            {
                var triggeringGeofences = geofencingEvent.TriggeringGeofences.ToList();
                var GeofencesToSkip = GeofenceDictionary.Values.Where(x => !RangeCalculator.InRange(Direction, x.StartBearing, x.EndBearing) || x.IsSeen).Select(x=>x.RequestId);

                triggeringGeofences.RemoveAll(x => GeofencesToSkip.Contains(x.RequestId));

                //foreach (var item in GeofencesToSkip)
                //{
                //    var geofenceToRemove = triggeringGeofences.Where(x => x.RequestId == item.RequestId).FirstOrDefault();
                //    if (geofenceToRemove != null)
                //    {
                //        triggeringGeofences.Remove(geofenceToRemove);
                //    }
                //}

                if (triggeringGeofences.Count > 0)
                {
                    TriggeredMarker = GeofenceDictionary.Values.Where(x=>x.RequestId == triggeringGeofences.First().RequestId).FirstOrDefault();

                    var geofenceTransitionDetails = GetGeofenceTransitionDetails(this, geofenceTransition, triggeringGeofences);

                    if (!string.IsNullOrEmpty(geofenceTransitionDetails))
                    {
                        SendNotification(geofenceTransitionDetails);
                    }
                    Log.Info(TAG, geofenceTransitionDetails);
                }
            }
            else
            {
                // Log the error.
                Log.Error(TAG, GetString(Resource.String.geofence_transition_invalid_type, new[] { new Java.Lang.Integer(geofenceTransition) }));
            }
        }

        string GetGeofenceTransitionDetails(Context context, int geofenceTransition, IList<IGeofence> triggeringGeofences)
        {
            var geofenceTransitionString = GetTransitionString(geofenceTransition);

            var triggeringGeofencesIdsList = new List<string>();
            foreach (var geofence in triggeringGeofences)
            {
                triggeringGeofencesIdsList.Add(geofence.RequestId);
            }
            var triggeringGeofencesIdsString = string.Join(", ", triggeringGeofencesIdsList);

            return geofenceTransitionString + ": " + triggeringGeofencesIdsString;
        }

        void SendNotification(string notificationDetails)
        {
            var notificationIntent = new Intent(ApplicationContext, typeof(MainActivity));

            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);

            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));

            stackBuilder.AddNextIntent(notificationIntent);

            var notificationPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            


            using (Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.app_icon))
            {
                var builder = new NotificationCompat.Builder(this);

                builder.SetSmallIcon(Resource.Drawable.app_icon)
                       .SetLargeIcon(bitmap)
                       .SetContentTitle("უახლოვდებით ბილბორდს")
                       .SetContentText(GetString(Resource.String.geofence_transition_entered))
                       .SetContentIntent(notificationPendingIntent)
                       .SetSound(Settings.System.DefaultNotificationUri);


                builder.SetAutoCancel(true);

                var mNotificationManager = (NotificationManager)GetSystemService(Context.NotificationService);

                mNotificationManager.Notify(0, builder.Build());
            }
        }

        string GetTransitionString(int transitionType)
        {
            switch (transitionType)
            {
                case Android.Gms.Location.Geofence.GeofenceTransitionEnter:
                    return GetString(Resource.String.geofence_transition_entered);
                case Android.Gms.Location.Geofence.GeofenceTransitionExit:
                    return GetString(Resource.String.geofence_transition_exited);
                default:
                    return GetString(Resource.String.unknown_geofence_transition);
            }
        }
    }
}