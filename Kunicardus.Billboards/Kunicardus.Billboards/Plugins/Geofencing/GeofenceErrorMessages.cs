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

namespace Kunicardus.Billboards.Plugins.Geofencing
{
    public static class GeofenceErrorMessages
    {
        public static string GetErrorString(Context context, int errorCode)
        {
            var mResources = context.Resources;
            switch (errorCode)
            {
                case GeofenceStatusCodes.GeofenceNotAvailable:
                    return mResources.GetString(Resource.String.geofence_not_available);
                case GeofenceStatusCodes.GeofenceTooManyGeofences:
                    return mResources.GetString(Resource.String.geofence_too_many_geofences);
                case GeofenceStatusCodes.GeofenceTooManyPendingIntents:
                    return mResources.GetString(Resource.String.geofence_too_many_pending_intents);
                default:
                    return mResources.GetString(Resource.String.unknown_geofence_error);
            }
        }
    }
}