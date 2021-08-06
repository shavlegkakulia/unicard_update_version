using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Android.Gms.Maps.Utils.Quadtree {

	// Metadata.xml XPath class reference: path="/api/package[@name='com.google.maps.android.quadtree']/class[@name='PointQuadTree']"
	[global::Android.Runtime.Register ("com/google/maps/android/quadtree/PointQuadTree", DoNotGenerateAcw=true)]
	[global::Java.Interop.JavaTypeParameters (new string [] {"T extends com.google.maps.android.quadtree.PointQuadTree.Item"})]
	public partial class PointQuadTree : global::Java.Lang.Object {

		// Metadata.xml XPath interface reference: path="/api/package[@name='com.google.maps.android.quadtree']/interface[@name='PointQuadTree.Item']"
		[Register ("com/google/maps/android/quadtree/PointQuadTree$Item", "", "Android.Gms.Maps.Utils.Quadtree.PointQuadTree/IItemInvoker")]
		public partial interface IItem : IJavaObject, IJavaPeerable {

			global::Android.Gms.Maps.Utils.Geometry.Point Point {
				// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.quadtree']/interface[@name='PointQuadTree.Item']/method[@name='getPoint' and count(parameter)=0]"
				[Register ("getPoint", "()Lcom/google/maps/android/geometry/Point;", "GetGetPointHandler:Android.Gms.Maps.Utils.Quadtree.PointQuadTree/IItemInvoker, GoogleMapsUtility")] get;
			}

		}

		[global::Android.Runtime.Register ("com/google/maps/android/quadtree/PointQuadTree$Item", DoNotGenerateAcw=true)]
		internal partial class IItemInvoker : global::Java.Lang.Object, IItem {

			static readonly JniPeerMembers _members = new XAPeerMembers ("com/google/maps/android/quadtree/PointQuadTree$Item", typeof (IItemInvoker));

			static IntPtr java_class_ref {
				get { return _members.JniPeerType.PeerReference.Handle; }
			}

			public override global::Java.Interop.JniPeerMembers JniPeerMembers {
				get { return _members; }
			}

			protected override IntPtr ThresholdClass {
				get { return class_ref; }
			}

			protected override global::System.Type ThresholdType {
				get { return _members.ManagedPeerType; }
			}

			IntPtr class_ref;

			public static IItem GetObject (IntPtr handle, JniHandleOwnership transfer)
			{
				return global::Java.Lang.Object.GetObject<IItem> (handle, transfer);
			}

			static IntPtr Validate (IntPtr handle)
			{
				if (!JNIEnv.IsInstanceOf (handle, java_class_ref))
					throw new InvalidCastException (string.Format ("Unable to convert instance of type '{0}' to type '{1}'.",
								JNIEnv.GetClassNameFromInstance (handle), "com.google.maps.android.quadtree.PointQuadTree.Item"));
				return handle;
			}

			protected override void Dispose (bool disposing)
			{
				if (this.class_ref != IntPtr.Zero)
					JNIEnv.DeleteGlobalRef (this.class_ref);
				this.class_ref = IntPtr.Zero;
				base.Dispose (disposing);
			}

			public IItemInvoker (IntPtr handle, JniHandleOwnership transfer) : base (Validate (handle), transfer)
			{
				IntPtr local_ref = JNIEnv.GetObjectClass (((global::Java.Lang.Object) this).Handle);
				this.class_ref = JNIEnv.NewGlobalRef (local_ref);
				JNIEnv.DeleteLocalRef (local_ref);
			}

			static Delegate cb_getPoint;
#pragma warning disable 0169
			static Delegate GetGetPointHandler ()
			{
				if (cb_getPoint == null)
					cb_getPoint = JNINativeWrapper.CreateDelegate ((_JniMarshal_PP_L) n_GetPoint);
				return cb_getPoint;
			}

			static IntPtr n_GetPoint (IntPtr jnienv, IntPtr native__this)
			{
				var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Quadtree.PointQuadTree.IItem> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
				return JNIEnv.ToLocalJniHandle (__this.Point);
			}
#pragma warning restore 0169

			IntPtr id_getPoint;
			public unsafe global::Android.Gms.Maps.Utils.Geometry.Point Point {
				get {
					if (id_getPoint == IntPtr.Zero)
						id_getPoint = JNIEnv.GetMethodID (class_ref, "getPoint", "()Lcom/google/maps/android/geometry/Point;");
					return global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Geometry.Point> (JNIEnv.CallObjectMethod (((global::Java.Lang.Object) this).Handle, id_getPoint), JniHandleOwnership.TransferLocalRef);
				}
			}

		}


		static readonly JniPeerMembers _members = new XAPeerMembers ("com/google/maps/android/quadtree/PointQuadTree", typeof (PointQuadTree));
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

		protected PointQuadTree (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='com.google.maps.android.quadtree']/class[@name='PointQuadTree']/constructor[@name='PointQuadTree' and count(parameter)=1 and parameter[1][@type='com.google.maps.android.geometry.Bounds']]"
		[Register (".ctor", "(Lcom/google/maps/android/geometry/Bounds;)V", "")]
		public unsafe PointQuadTree (global::Android.Gms.Maps.Utils.Geometry.Bounds p0)
			: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			const string __id = "(Lcom/google/maps/android/geometry/Bounds;)V";

			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue ((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object) p0).Handle);
				var __r = _members.InstanceMethods.StartCreateInstance (__id, ((object) this).GetType (), __args);
				SetHandle (__r.Handle, JniHandleOwnership.TransferLocalRef);
				_members.InstanceMethods.FinishCreateInstance (__id, this, __args);
			} finally {
				global::System.GC.KeepAlive (p0);
			}
		}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='com.google.maps.android.quadtree']/class[@name='PointQuadTree']/constructor[@name='PointQuadTree' and count(parameter)=4 and parameter[1][@type='double'] and parameter[2][@type='double'] and parameter[3][@type='double'] and parameter[4][@type='double']]"
		[Register (".ctor", "(DDDD)V", "")]
		public unsafe PointQuadTree (double p0, double p1, double p2, double p3)
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

		static Delegate cb_add_Lcom_google_maps_android_quadtree_PointQuadTree_Item_;
#pragma warning disable 0169
		static Delegate GetAdd_Lcom_google_maps_android_quadtree_PointQuadTree_Item_Handler ()
		{
			if (cb_add_Lcom_google_maps_android_quadtree_PointQuadTree_Item_ == null)
				cb_add_Lcom_google_maps_android_quadtree_PointQuadTree_Item_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_V) n_Add_Lcom_google_maps_android_quadtree_PointQuadTree_Item_);
			return cb_add_Lcom_google_maps_android_quadtree_PointQuadTree_Item_;
		}

		static void n_Add_Lcom_google_maps_android_quadtree_PointQuadTree_Item_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Quadtree.PointQuadTree> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (native_p0, JniHandleOwnership.DoNotTransfer);
			__this.Add (p0);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.quadtree']/class[@name='PointQuadTree']/method[@name='add' and count(parameter)=1 and parameter[1][@type='T']]"
		[Register ("add", "(Lcom/google/maps/android/quadtree/PointQuadTree$Item;)V", "GetAdd_Lcom_google_maps_android_quadtree_PointQuadTree_Item_Handler")]
		public virtual unsafe void Add (global::Java.Lang.Object p0)
		{
			const string __id = "add.(Lcom/google/maps/android/quadtree/PointQuadTree$Item;)V";
			IntPtr native_p0 = JNIEnv.ToLocalJniHandle (p0);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (native_p0);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
				JNIEnv.DeleteLocalRef (native_p0);
				global::System.GC.KeepAlive (p0);
			}
		}

		static Delegate cb_clear;
#pragma warning disable 0169
		static Delegate GetClearHandler ()
		{
			if (cb_clear == null)
				cb_clear = JNINativeWrapper.CreateDelegate ((_JniMarshal_PP_V) n_Clear);
			return cb_clear;
		}

		static void n_Clear (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Quadtree.PointQuadTree> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.Clear ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.quadtree']/class[@name='PointQuadTree']/method[@name='clear' and count(parameter)=0]"
		[Register ("clear", "()V", "GetClearHandler")]
		public virtual unsafe void Clear ()
		{
			const string __id = "clear.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_remove_Lcom_google_maps_android_quadtree_PointQuadTree_Item_;
#pragma warning disable 0169
		static Delegate GetRemove_Lcom_google_maps_android_quadtree_PointQuadTree_Item_Handler ()
		{
			if (cb_remove_Lcom_google_maps_android_quadtree_PointQuadTree_Item_ == null)
				cb_remove_Lcom_google_maps_android_quadtree_PointQuadTree_Item_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_Z) n_Remove_Lcom_google_maps_android_quadtree_PointQuadTree_Item_);
			return cb_remove_Lcom_google_maps_android_quadtree_PointQuadTree_Item_;
		}

		static bool n_Remove_Lcom_google_maps_android_quadtree_PointQuadTree_Item_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Quadtree.PointQuadTree> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (native_p0, JniHandleOwnership.DoNotTransfer);
			bool __ret = __this.Remove (p0);
			return __ret;
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.quadtree']/class[@name='PointQuadTree']/method[@name='remove' and count(parameter)=1 and parameter[1][@type='T']]"
		[Register ("remove", "(Lcom/google/maps/android/quadtree/PointQuadTree$Item;)Z", "GetRemove_Lcom_google_maps_android_quadtree_PointQuadTree_Item_Handler")]
		public virtual unsafe bool Remove (global::Java.Lang.Object p0)
		{
			const string __id = "remove.(Lcom/google/maps/android/quadtree/PointQuadTree$Item;)Z";
			IntPtr native_p0 = JNIEnv.ToLocalJniHandle (p0);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (native_p0);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
				JNIEnv.DeleteLocalRef (native_p0);
				global::System.GC.KeepAlive (p0);
			}
		}

		static Delegate cb_search_Lcom_google_maps_android_geometry_Bounds_;
#pragma warning disable 0169
		static Delegate GetSearch_Lcom_google_maps_android_geometry_Bounds_Handler ()
		{
			if (cb_search_Lcom_google_maps_android_geometry_Bounds_ == null)
				cb_search_Lcom_google_maps_android_geometry_Bounds_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_L) n_Search_Lcom_google_maps_android_geometry_Bounds_);
			return cb_search_Lcom_google_maps_android_geometry_Bounds_;
		}

		static IntPtr n_Search_Lcom_google_maps_android_geometry_Bounds_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Quadtree.PointQuadTree> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.Geometry.Bounds> (native_p0, JniHandleOwnership.DoNotTransfer);
			IntPtr __ret = global::Android.Runtime.JavaCollection.ToLocalJniHandle (__this.Search (p0));
			return __ret;
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.quadtree']/class[@name='PointQuadTree']/method[@name='search' and count(parameter)=1 and parameter[1][@type='com.google.maps.android.geometry.Bounds']]"
		[Register ("search", "(Lcom/google/maps/android/geometry/Bounds;)Ljava/util/Collection;", "GetSearch_Lcom_google_maps_android_geometry_Bounds_Handler")]
		public virtual unsafe global::System.Collections.ICollection Search (global::Android.Gms.Maps.Utils.Geometry.Bounds p0)
		{
			const string __id = "search.(Lcom/google/maps/android/geometry/Bounds;)Ljava/util/Collection;";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue ((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object) p0).Handle);
				var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, __args);
				return global::Android.Runtime.JavaCollection.FromJniHandle (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
				global::System.GC.KeepAlive (p0);
			}
		}

	}
}
