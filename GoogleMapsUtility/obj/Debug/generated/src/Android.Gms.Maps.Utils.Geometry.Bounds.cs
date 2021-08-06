using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Android.Gms.Maps.Utils.Geometry {

	// Metadata.xml XPath class reference: path="/api/package[@name='com.google.maps.android.geometry']/class[@name='Bounds']"
	[global::Android.Runtime.Register ("com/google/maps/android/geometry/Bounds", DoNotGenerateAcw=true)]
	public partial class Bounds : global::Java.Lang.Object {



		// Metadata.xml XPath field reference: path="/api/package[@name='com.google.maps.android.geometry']/class[@name='Bounds']/field[@name='maxX']"
		[Register ("maxX")]
		public double MaxX {
			get {
				const string __id = "maxX.D";

				var __v = _members.InstanceFields.GetDoubleValue (__id, this);
				return __v;
			}
			set {
				const string __id = "maxX.D";

				try {
					_members.InstanceFields.SetValue (__id, this, value);
				} finally {
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='com.google.maps.android.geometry']/class[@name='Bounds']/field[@name='maxY']"
		[Register ("maxY")]
		public double MaxY {
			get {
				const string __id = "maxY.D";

				var __v = _members.InstanceFields.GetDoubleValue (__id, this);
				return __v;
			}
			set {
				const string __id = "maxY.D";

				try {
					_members.InstanceFields.SetValue (__id, this, value);
				} finally {
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='com.google.maps.android.geometry']/class[@name='Bounds']/field[@name='midX']"
		[Register ("midX")]
		public double MidX {
			get {
				const string __id = "midX.D";

				var __v = _members.InstanceFields.GetDoubleValue (__id, this);
				return __v;
			}
			set {
				const string __id = "midX.D";

				try {
					_members.InstanceFields.SetValue (__id, this, value);
				} finally {
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='com.google.maps.android.geometry']/class[@name='Bounds']/field[@name='midY']"
		[Register ("midY")]
		public double MidY {
			get {
				const string __id = "midY.D";

				var __v = _members.InstanceFields.GetDoubleValue (__id, this);
				return __v;
			}
			set {
				const string __id = "midY.D";

				try {
					_members.InstanceFields.SetValue (__id, this, value);
				} finally {
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='com.google.maps.android.geometry']/class[@name='Bounds']/field[@name='minX']"
		[Register ("minX")]
		public double MinX {
			get {
				const string __id = "minX.D";

				var __v = _members.InstanceFields.GetDoubleValue (__id, this);
				return __v;
			}
			set {
				const string __id = "minX.D";

				try {
					_members.InstanceFields.SetValue (__id, this, value);
				} finally {
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='com.google.maps.android.geometry']/class[@name='Bounds']/field[@name='minY']"
		[Register ("minY")]
		public double MinY {
			get {
				const string __id = "minY.D";

				var __v = _members.InstanceFields.GetDoubleValue (__id, this);
				return __v;
			}
			set {
				const string __id = "minY.D";

				try {
					_members.InstanceFields.SetValue (__id, this, value);
				} finally {
				}
			}
		}
		static readonly JniPeerMembers _members = new XAPeerMembers ("com/google/maps/android/geometry/Bounds", typeof (Bounds));
		internal static IntPtr class_ref {
			get {
				return _members.JniPeerType.PeerReference.Handle;
			}
		}

		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected override IntPtr ThresholdClass {
			get { return _members.JniPeerType.PeerReference.Handle; }
		}

		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

		protected Bounds (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='com.google.maps.android.geometry']/class[@name='Bounds']/constructor[@name='Bounds' and count(parameter)=4 and parameter[1][@type='double'] and parameter[2][@type='double'] and parameter[3][@type='double'] and parameter[4][@type='double']]"
		[Register (".ctor", "(DDDD)V", "")]
		public unsafe Bounds (double p0, double p1, double p2, double p3)
			: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			const string __id = "(DDDD)V";

			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [4];
				__args [0] = new JniArgumentValue (p0);
				__args [1] = new JniArgumentValue (p1);
				__args [2] = new JniArgumentValue (p2);
				__args [3] = new JniArgumentValue (p3);
				var __r = _members.InstanceMethods.StartCreateInstance (__id, ((object) this).GetType (), __args);
				SetHandle (__r.Handle, JniHandleOwnership.TransferLocalRef);
				_members.InstanceMethods.FinishCreateInstance (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_contains_Lcom_google_maps_android_geometry_Bounds_;
#pragma warning disable 0169
		static Delegate GetContains_Lcom_google_maps_android_geometry_Bounds_Handler ()
		{
			if (cb_contains_Lcom_google_maps_android_geometry_Bounds_ == null)
				cb_contains_Lcom_google_maps_android_geometry_Bounds_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_Z) n_Contains_Lcom_google_maps_android_geometry_Bounds_);
			return cb_contains_Lcom_google_maps_android_geometry_Bounds_;
		}

		static bool n_Contains_Lcom_google_maps_android_geometry_Bounds_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Geometry.Bounds> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Geometry.Bounds> (native_p0, JniHandleOwnership.DoNotTransfer);
			bool __ret = __this.Contains (p0);
			return __ret;
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.geometry']/class[@name='Bounds']/method[@name='contains' and count(parameter)=1 and parameter[1][@type='com.google.maps.android.geometry.Bounds']]"
		[Register ("contains", "(Lcom/google/maps/android/geometry/Bounds;)Z", "GetContains_Lcom_google_maps_android_geometry_Bounds_Handler")]
		public virtual unsafe bool Contains (global::Android.Gms.Maps.Utils.Geometry.Bounds p0)
		{
			const string __id = "contains.(Lcom/google/maps/android/geometry/Bounds;)Z";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue ((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object) p0).Handle);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
				global::System.GC.KeepAlive (p0);
			}
		}

		static Delegate cb_contains_Lcom_google_maps_android_geometry_Point_;
#pragma warning disable 0169
		static Delegate GetContains_Lcom_google_maps_android_geometry_Point_Handler ()
		{
			if (cb_contains_Lcom_google_maps_android_geometry_Point_ == null)
				cb_contains_Lcom_google_maps_android_geometry_Point_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_Z) n_Contains_Lcom_google_maps_android_geometry_Point_);
			return cb_contains_Lcom_google_maps_android_geometry_Point_;
		}

		static bool n_Contains_Lcom_google_maps_android_geometry_Point_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Geometry.Bounds> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Geometry.Point> (native_p0, JniHandleOwnership.DoNotTransfer);
			bool __ret = __this.Contains (p0);
			return __ret;
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.geometry']/class[@name='Bounds']/method[@name='contains' and count(parameter)=1 and parameter[1][@type='com.google.maps.android.geometry.Point']]"
		[Register ("contains", "(Lcom/google/maps/android/geometry/Point;)Z", "GetContains_Lcom_google_maps_android_geometry_Point_Handler")]
		public virtual unsafe bool Contains (global::Android.Gms.Maps.Utils.Geometry.Point p0)
		{
			const string __id = "contains.(Lcom/google/maps/android/geometry/Point;)Z";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue ((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object) p0).Handle);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
				global::System.GC.KeepAlive (p0);
			}
		}

		static Delegate cb_contains_DD;
#pragma warning disable 0169
		static Delegate GetContains_DDHandler ()
		{
			if (cb_contains_DD == null)
				cb_contains_DD = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPDD_Z) n_Contains_DD);
			return cb_contains_DD;
		}

		static bool n_Contains_DD (IntPtr jnienv, IntPtr native__this, double p0, double p1)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Geometry.Bounds> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.Contains (p0, p1);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.geometry']/class[@name='Bounds']/method[@name='contains' and count(parameter)=2 and parameter[1][@type='double'] and parameter[2][@type='double']]"
		[Register ("contains", "(DD)Z", "GetContains_DDHandler")]
		public virtual unsafe bool Contains (double p0, double p1)
		{
			const string __id = "contains.(DD)Z";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [2];
				__args [0] = new JniArgumentValue (p0);
				__args [1] = new JniArgumentValue (p1);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
			}
		}

		static Delegate cb_intersects_Lcom_google_maps_android_geometry_Bounds_;
#pragma warning disable 0169
		static Delegate GetIntersects_Lcom_google_maps_android_geometry_Bounds_Handler ()
		{
			if (cb_intersects_Lcom_google_maps_android_geometry_Bounds_ == null)
				cb_intersects_Lcom_google_maps_android_geometry_Bounds_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_Z) n_Intersects_Lcom_google_maps_android_geometry_Bounds_);
			return cb_intersects_Lcom_google_maps_android_geometry_Bounds_;
		}

		static bool n_Intersects_Lcom_google_maps_android_geometry_Bounds_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Geometry.Bounds> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Geometry.Bounds> (native_p0, JniHandleOwnership.DoNotTransfer);
			bool __ret = __this.Intersects (p0);
			return __ret;
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.geometry']/class[@name='Bounds']/method[@name='intersects' and count(parameter)=1 and parameter[1][@type='com.google.maps.android.geometry.Bounds']]"
		[Register ("intersects", "(Lcom/google/maps/android/geometry/Bounds;)Z", "GetIntersects_Lcom_google_maps_android_geometry_Bounds_Handler")]
		public virtual unsafe bool Intersects (global::Android.Gms.Maps.Utils.Geometry.Bounds p0)
		{
			const string __id = "intersects.(Lcom/google/maps/android/geometry/Bounds;)Z";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue ((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object) p0).Handle);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
				global::System.GC.KeepAlive (p0);
			}
		}

		static Delegate cb_intersects_DDDD;
#pragma warning disable 0169
		static Delegate GetIntersects_DDDDHandler ()
		{
			if (cb_intersects_DDDD == null)
				cb_intersects_DDDD = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPDDDD_Z) n_Intersects_DDDD);
			return cb_intersects_DDDD;
		}

		static bool n_Intersects_DDDD (IntPtr jnienv, IntPtr native__this, double p0, double p1, double p2, double p3)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Geometry.Bounds> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.Intersects (p0, p1, p2, p3);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.geometry']/class[@name='Bounds']/method[@name='intersects' and count(parameter)=4 and parameter[1][@type='double'] and parameter[2][@type='double'] and parameter[3][@type='double'] and parameter[4][@type='double']]"
		[Register ("intersects", "(DDDD)Z", "GetIntersects_DDDDHandler")]
		public virtual unsafe bool Intersects (double p0, double p1, double p2, double p3)
		{
			const string __id = "intersects.(DDDD)Z";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [4];
				__args [0] = new JniArgumentValue (p0);
				__args [1] = new JniArgumentValue (p1);
				__args [2] = new JniArgumentValue (p2);
				__args [3] = new JniArgumentValue (p3);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
			}
		}

	}
}
