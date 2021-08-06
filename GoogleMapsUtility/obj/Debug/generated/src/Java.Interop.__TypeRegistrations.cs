using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Java.Interop {

	partial class __TypeRegistrations {

		public static void RegisterPackages ()
		{
#if MONODROID_TIMING
			var start = DateTime.Now;
			Android.Util.Log.Info ("MonoDroid-Timing", "RegisterPackages start: " + (start - new DateTime (1970, 1, 1)).TotalMilliseconds);
#endif // def MONODROID_TIMING
			Java.Interop.TypeManager.RegisterPackages (
					new string[]{
						"com/google/maps/android",
						"com/google/maps/android/geometry",
						"com/google/maps/android/projection",
						"com/google/maps/android/quadtree",
						"com/google/maps/android/ui",
					},
					new Converter<string, Type>[]{
						lookup_com_google_maps_android_package,
						lookup_com_google_maps_android_geometry_package,
						lookup_com_google_maps_android_projection_package,
						lookup_com_google_maps_android_quadtree_package,
						lookup_com_google_maps_android_ui_package,
					});
#if MONODROID_TIMING
			var end = DateTime.Now;
			Android.Util.Log.Info ("MonoDroid-Timing", "RegisterPackages time: " + (end - new DateTime (1970, 1, 1)).TotalMilliseconds + " [elapsed: " + (end - start).TotalMilliseconds + " ms]");
#endif // def MONODROID_TIMING
		}

		static Type Lookup (string[] mappings, string javaType)
		{
			var managedType = Java.Interop.TypeManager.LookupTypeMapping (mappings, javaType);
			if (managedType == null)
				return null;
			return Type.GetType (managedType);
		}

		static string[] package_com_google_maps_android_mappings;
		static Type lookup_com_google_maps_android_package (string klass)
		{
			if (package_com_google_maps_android_mappings == null) {
				package_com_google_maps_android_mappings = new string[]{
					"com/google/maps/android/BuildConfig:Android.Gms.Maps.Utils.BuildConfig",
				};
			}

			return Lookup (package_com_google_maps_android_mappings, klass);
		}

		static string[] package_com_google_maps_android_geometry_mappings;
		static Type lookup_com_google_maps_android_geometry_package (string klass)
		{
			if (package_com_google_maps_android_geometry_mappings == null) {
				package_com_google_maps_android_geometry_mappings = new string[]{
					"com/google/maps/android/geometry/Bounds:Android.Gms.Maps.Utils.Geometry.Bounds",
					"com/google/maps/android/geometry/Point:Android.Gms.Maps.Utils.Geometry.Point",
				};
			}

			return Lookup (package_com_google_maps_android_geometry_mappings, klass);
		}

		static string[] package_com_google_maps_android_projection_mappings;
		static Type lookup_com_google_maps_android_projection_package (string klass)
		{
			if (package_com_google_maps_android_projection_mappings == null) {
				package_com_google_maps_android_projection_mappings = new string[]{
					"com/google/maps/android/projection/Point:Android.Gms.Maps.Utils.Projection.Point",
				};
			}

			return Lookup (package_com_google_maps_android_projection_mappings, klass);
		}

		static string[] package_com_google_maps_android_quadtree_mappings;
		static Type lookup_com_google_maps_android_quadtree_package (string klass)
		{
			if (package_com_google_maps_android_quadtree_mappings == null) {
				package_com_google_maps_android_quadtree_mappings = new string[]{
					"com/google/maps/android/quadtree/PointQuadTree:Android.Gms.Maps.Utils.Quadtree.PointQuadTree",
				};
			}

			return Lookup (package_com_google_maps_android_quadtree_mappings, klass);
		}

		static string[] package_com_google_maps_android_ui_mappings;
		static Type lookup_com_google_maps_android_ui_package (string klass)
		{
			if (package_com_google_maps_android_ui_mappings == null) {
				package_com_google_maps_android_ui_mappings = new string[]{
					"com/google/maps/android/ui/BubbleIconFactory:Android.Gms.Maps.Utils.UI.BubbleIconFactory",
					"com/google/maps/android/ui/IconGenerator:Android.Gms.Maps.Utils.UI.IconGenerator",
					"com/google/maps/android/ui/SquareTextView:Android.Gms.Maps.Utils.UI.SquareTextView",
				};
			}

			return Lookup (package_com_google_maps_android_ui_mappings, klass);
		}
	}
}
