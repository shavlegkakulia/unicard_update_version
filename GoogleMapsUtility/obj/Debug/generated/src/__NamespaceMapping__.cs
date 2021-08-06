using System;

[assembly:global::Android.Runtime.NamespaceMapping (Java = "com.google.maps.android", Managed="Android.Gms.Maps.Utils")]
[assembly:global::Android.Runtime.NamespaceMapping (Java = "com.google.maps.android.geometry", Managed="Android.Gms.Maps.Utils.Geometry")]
[assembly:global::Android.Runtime.NamespaceMapping (Java = "com.google.maps.android.projection", Managed="Android.Gms.Maps.Utils.Projection")]
[assembly:global::Android.Runtime.NamespaceMapping (Java = "com.google.maps.android.quadtree", Managed="Android.Gms.Maps.Utils.Quadtree")]
[assembly:global::Android.Runtime.NamespaceMapping (Java = "com.google.maps.android.ui", Managed="Android.Gms.Maps.Utils.UI")]

delegate float _JniMarshal_PP_F (IntPtr jnienv, IntPtr klass);
delegate IntPtr _JniMarshal_PP_L (IntPtr jnienv, IntPtr klass);
delegate void _JniMarshal_PP_V (IntPtr jnienv, IntPtr klass);
delegate bool _JniMarshal_PPDD_Z (IntPtr jnienv, IntPtr klass, double p0, double p1);
delegate bool _JniMarshal_PPDDDD_Z (IntPtr jnienv, IntPtr klass, double p0, double p1, double p2, double p3);
delegate void _JniMarshal_PPI_V (IntPtr jnienv, IntPtr klass, int p0);
delegate void _JniMarshal_PPIIII_V (IntPtr jnienv, IntPtr klass, int p0, int p1, int p2, int p3);
delegate IntPtr _JniMarshal_PPL_L (IntPtr jnienv, IntPtr klass, IntPtr p0);
delegate void _JniMarshal_PPL_V (IntPtr jnienv, IntPtr klass, IntPtr p0);
delegate bool _JniMarshal_PPL_Z (IntPtr jnienv, IntPtr klass, IntPtr p0);
delegate void _JniMarshal_PPLI_V (IntPtr jnienv, IntPtr klass, IntPtr p0, int p1);
