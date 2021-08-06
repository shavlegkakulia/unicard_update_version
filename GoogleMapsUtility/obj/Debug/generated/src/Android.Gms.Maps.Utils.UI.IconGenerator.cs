using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Android.Gms.Maps.Utils.UI {

	// Metadata.xml XPath class reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']"
	[global::Android.Runtime.Register ("com/google/maps/android/ui/IconGenerator", DoNotGenerateAcw=true)]
	public partial class IconGenerator : global::Java.Lang.Object {


		// Metadata.xml XPath field reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/field[@name='STYLE_BLUE']"
		[Register ("STYLE_BLUE")]
		public const int StyleBlue = (int) 4;

		// Metadata.xml XPath field reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/field[@name='STYLE_DEFAULT']"
		[Register ("STYLE_DEFAULT")]
		public const int StyleDefault = (int) 1;

		// Metadata.xml XPath field reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/field[@name='STYLE_GREEN']"
		[Register ("STYLE_GREEN")]
		public const int StyleGreen = (int) 5;

		// Metadata.xml XPath field reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/field[@name='STYLE_ORANGE']"
		[Register ("STYLE_ORANGE")]
		public const int StyleOrange = (int) 7;

		// Metadata.xml XPath field reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/field[@name='STYLE_PURPLE']"
		[Register ("STYLE_PURPLE")]
		public const int StylePurple = (int) 6;

		// Metadata.xml XPath field reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/field[@name='STYLE_RED']"
		[Register ("STYLE_RED")]
		public const int StyleRed = (int) 3;

		// Metadata.xml XPath field reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/field[@name='STYLE_WHITE']"
		[Register ("STYLE_WHITE")]
		public const int StyleWhite = (int) 2;
		static readonly JniPeerMembers _members = new XAPeerMembers ("com/google/maps/android/ui/IconGenerator", typeof (IconGenerator));
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

		protected IconGenerator (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/constructor[@name='IconGenerator' and count(parameter)=1 and parameter[1][@type='android.content.Context']]"
		[Register (".ctor", "(Landroid/content/Context;)V", "")]
		public unsafe IconGenerator (global::Android.Content.Context p0)
			: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			const string __id = "(Landroid/content/Context;)V";

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

		static Delegate cb_getAnchorU;
#pragma warning disable 0169
		static Delegate GetGetAnchorUHandler ()
		{
			if (cb_getAnchorU == null)
				cb_getAnchorU = JNINativeWrapper.CreateDelegate ((_JniMarshal_PP_F) n_GetAnchorU);
			return cb_getAnchorU;
		}

		static float n_GetAnchorU (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.UI.IconGenerator> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.AnchorU;
		}
#pragma warning restore 0169

		public virtual unsafe float AnchorU {
			// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/method[@name='getAnchorU' and count(parameter)=0]"
			[Register ("getAnchorU", "()F", "GetGetAnchorUHandler")]
			get {
				const string __id = "getAnchorU.()F";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualSingleMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_getAnchorV;
#pragma warning disable 0169
		static Delegate GetGetAnchorVHandler ()
		{
			if (cb_getAnchorV == null)
				cb_getAnchorV = JNINativeWrapper.CreateDelegate ((_JniMarshal_PP_F) n_GetAnchorV);
			return cb_getAnchorV;
		}

		static float n_GetAnchorV (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.UI.IconGenerator> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.AnchorV;
		}
#pragma warning restore 0169

		public virtual unsafe float AnchorV {
			// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/method[@name='getAnchorV' and count(parameter)=0]"
			[Register ("getAnchorV", "()F", "GetGetAnchorVHandler")]
			get {
				const string __id = "getAnchorV.()F";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualSingleMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_makeIcon;
#pragma warning disable 0169
		static Delegate GetMakeIconHandler ()
		{
			if (cb_makeIcon == null)
				cb_makeIcon = JNINativeWrapper.CreateDelegate ((_JniMarshal_PP_L) n_MakeIcon);
			return cb_makeIcon;
		}

		static IntPtr n_MakeIcon (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.UI.IconGenerator> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.ToLocalJniHandle (__this.MakeIcon ());
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/method[@name='makeIcon' and count(parameter)=0]"
		[Register ("makeIcon", "()Landroid/graphics/Bitmap;", "GetMakeIconHandler")]
		public virtual unsafe global::Android.Graphics.Bitmap MakeIcon ()
		{
			const string __id = "makeIcon.()Landroid/graphics/Bitmap;";
			try {
				var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, null);
				return global::Java.Lang.Object.GetObject<global::Android.Graphics.Bitmap> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
			}
		}

		static Delegate cb_makeIcon_Ljava_lang_String_;
#pragma warning disable 0169
		static Delegate GetMakeIcon_Ljava_lang_String_Handler ()
		{
			if (cb_makeIcon_Ljava_lang_String_ == null)
				cb_makeIcon_Ljava_lang_String_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_L) n_MakeIcon_Ljava_lang_String_);
			return cb_makeIcon_Ljava_lang_String_;
		}

		static IntPtr n_MakeIcon_Ljava_lang_String_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.UI.IconGenerator> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = JNIEnv.GetString (native_p0, JniHandleOwnership.DoNotTransfer);
			IntPtr __ret = JNIEnv.ToLocalJniHandle (__this.MakeIcon (p0));
			return __ret;
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/method[@name='makeIcon' and count(parameter)=1 and parameter[1][@type='java.lang.String']]"
		[Register ("makeIcon", "(Ljava/lang/String;)Landroid/graphics/Bitmap;", "GetMakeIcon_Ljava_lang_String_Handler")]
		public virtual unsafe global::Android.Graphics.Bitmap MakeIcon (string p0)
		{
			const string __id = "makeIcon.(Ljava/lang/String;)Landroid/graphics/Bitmap;";
			IntPtr native_p0 = JNIEnv.NewString (p0);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (native_p0);
				var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, __args);
				return global::Java.Lang.Object.GetObject<global::Android.Graphics.Bitmap> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
				JNIEnv.DeleteLocalRef (native_p0);
			}
		}

		static Delegate cb_setBackground_Landroid_graphics_drawable_Drawable_;
#pragma warning disable 0169
		static Delegate GetSetBackground_Landroid_graphics_drawable_Drawable_Handler ()
		{
			if (cb_setBackground_Landroid_graphics_drawable_Drawable_ == null)
				cb_setBackground_Landroid_graphics_drawable_Drawable_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_V) n_SetBackground_Landroid_graphics_drawable_Drawable_);
			return cb_setBackground_Landroid_graphics_drawable_Drawable_;
		}

		static void n_SetBackground_Landroid_graphics_drawable_Drawable_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.UI.IconGenerator> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = global::Java.Lang.Object.GetObject<global::Android.Graphics.Drawables.Drawable> (native_p0, JniHandleOwnership.DoNotTransfer);
			__this.SetBackground (p0);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/method[@name='setBackground' and count(parameter)=1 and parameter[1][@type='android.graphics.drawable.Drawable']]"
		[Register ("setBackground", "(Landroid/graphics/drawable/Drawable;)V", "GetSetBackground_Landroid_graphics_drawable_Drawable_Handler")]
		public virtual unsafe void SetBackground (global::Android.Graphics.Drawables.Drawable p0)
		{
			const string __id = "setBackground.(Landroid/graphics/drawable/Drawable;)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue ((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object) p0).Handle);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
				global::System.GC.KeepAlive (p0);
			}
		}

		static Delegate cb_setColor_I;
#pragma warning disable 0169
		static Delegate GetSetColor_IHandler ()
		{
			if (cb_setColor_I == null)
				cb_setColor_I = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPI_V) n_SetColor_I);
			return cb_setColor_I;
		}

		static void n_SetColor_I (IntPtr jnienv, IntPtr native__this, int p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.UI.IconGenerator> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetColor (p0);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/method[@name='setColor' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("setColor", "(I)V", "GetSetColor_IHandler")]
		public virtual unsafe void SetColor (int p0)
		{
			const string __id = "setColor.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (p0);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_setContentPadding_IIII;
#pragma warning disable 0169
		static Delegate GetSetContentPadding_IIIIHandler ()
		{
			if (cb_setContentPadding_IIII == null)
				cb_setContentPadding_IIII = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPIIII_V) n_SetContentPadding_IIII);
			return cb_setContentPadding_IIII;
		}

		static void n_SetContentPadding_IIII (IntPtr jnienv, IntPtr native__this, int p0, int p1, int p2, int p3)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.UI.IconGenerator> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetContentPadding (p0, p1, p2, p3);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/method[@name='setContentPadding' and count(parameter)=4 and parameter[1][@type='int'] and parameter[2][@type='int'] and parameter[3][@type='int'] and parameter[4][@type='int']]"
		[Register ("setContentPadding", "(IIII)V", "GetSetContentPadding_IIIIHandler")]
		public virtual unsafe void SetContentPadding (int p0, int p1, int p2, int p3)
		{
			const string __id = "setContentPadding.(IIII)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [4];
				__args [0] = new JniArgumentValue (p0);
				__args [1] = new JniArgumentValue (p1);
				__args [2] = new JniArgumentValue (p2);
				__args [3] = new JniArgumentValue (p3);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_setContentRotation_I;
#pragma warning disable 0169
		static Delegate GetSetContentRotation_IHandler ()
		{
			if (cb_setContentRotation_I == null)
				cb_setContentRotation_I = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPI_V) n_SetContentRotation_I);
			return cb_setContentRotation_I;
		}

		static void n_SetContentRotation_I (IntPtr jnienv, IntPtr native__this, int p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.UI.IconGenerator> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetContentRotation (p0);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/method[@name='setContentRotation' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("setContentRotation", "(I)V", "GetSetContentRotation_IHandler")]
		public virtual unsafe void SetContentRotation (int p0)
		{
			const string __id = "setContentRotation.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (p0);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_setContentView_Landroid_view_View_;
#pragma warning disable 0169
		static Delegate GetSetContentView_Landroid_view_View_Handler ()
		{
			if (cb_setContentView_Landroid_view_View_ == null)
				cb_setContentView_Landroid_view_View_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_V) n_SetContentView_Landroid_view_View_);
			return cb_setContentView_Landroid_view_View_;
		}

		static void n_SetContentView_Landroid_view_View_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.UI.IconGenerator> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = global::Java.Lang.Object.GetObject<global::Android.Views.View> (native_p0, JniHandleOwnership.DoNotTransfer);
			__this.SetContentView (p0);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/method[@name='setContentView' and count(parameter)=1 and parameter[1][@type='android.view.View']]"
		[Register ("setContentView", "(Landroid/view/View;)V", "GetSetContentView_Landroid_view_View_Handler")]
		public virtual unsafe void SetContentView (global::Android.Views.View p0)
		{
			const string __id = "setContentView.(Landroid/view/View;)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue ((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object) p0).Handle);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
				global::System.GC.KeepAlive (p0);
			}
		}

		static Delegate cb_setRotation_I;
#pragma warning disable 0169
		static Delegate GetSetRotation_IHandler ()
		{
			if (cb_setRotation_I == null)
				cb_setRotation_I = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPI_V) n_SetRotation_I);
			return cb_setRotation_I;
		}

		static void n_SetRotation_I (IntPtr jnienv, IntPtr native__this, int p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.UI.IconGenerator> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetRotation (p0);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/method[@name='setRotation' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("setRotation", "(I)V", "GetSetRotation_IHandler")]
		public virtual unsafe void SetRotation (int p0)
		{
			const string __id = "setRotation.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (p0);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_setStyle_I;
#pragma warning disable 0169
		static Delegate GetSetStyle_IHandler ()
		{
			if (cb_setStyle_I == null)
				cb_setStyle_I = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPI_V) n_SetStyle_I);
			return cb_setStyle_I;
		}

		static void n_SetStyle_I (IntPtr jnienv, IntPtr native__this, int p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.UI.IconGenerator> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetStyle (p0);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/method[@name='setStyle' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("setStyle", "(I)V", "GetSetStyle_IHandler")]
		public virtual unsafe void SetStyle (int p0)
		{
			const string __id = "setStyle.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (p0);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_setTextAppearance_Landroid_content_Context_I;
#pragma warning disable 0169
		static Delegate GetSetTextAppearance_Landroid_content_Context_IHandler ()
		{
			if (cb_setTextAppearance_Landroid_content_Context_I == null)
				cb_setTextAppearance_Landroid_content_Context_I = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPLI_V) n_SetTextAppearance_Landroid_content_Context_I);
			return cb_setTextAppearance_Landroid_content_Context_I;
		}

		static void n_SetTextAppearance_Landroid_content_Context_I (IntPtr jnienv, IntPtr native__this, IntPtr native_p0, int p1)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.UI.IconGenerator> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = global::Java.Lang.Object.GetObject<global::Android.Content.Context> (native_p0, JniHandleOwnership.DoNotTransfer);
			__this.SetTextAppearance (p0, p1);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/method[@name='setTextAppearance' and count(parameter)=2 and parameter[1][@type='android.content.Context'] and parameter[2][@type='int']]"
		[Register ("setTextAppearance", "(Landroid/content/Context;I)V", "GetSetTextAppearance_Landroid_content_Context_IHandler")]
		public virtual unsafe void SetTextAppearance (global::Android.Content.Context p0, int p1)
		{
			const string __id = "setTextAppearance.(Landroid/content/Context;I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [2];
				__args [0] = new JniArgumentValue ((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object) p0).Handle);
				__args [1] = new JniArgumentValue (p1);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
				global::System.GC.KeepAlive (p0);
			}
		}

		static Delegate cb_setTextAppearance_I;
#pragma warning disable 0169
		static Delegate GetSetTextAppearance_IHandler ()
		{
			if (cb_setTextAppearance_I == null)
				cb_setTextAppearance_I = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPI_V) n_SetTextAppearance_I);
			return cb_setTextAppearance_I;
		}

		static void n_SetTextAppearance_I (IntPtr jnienv, IntPtr native__this, int p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Gms.Maps.Utils.UI.IconGenerator> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetTextAppearance (p0);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.maps.android.ui']/class[@name='IconGenerator']/method[@name='setTextAppearance' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("setTextAppearance", "(I)V", "GetSetTextAppearance_IHandler")]
		public virtual unsafe void SetTextAppearance (int p0)
		{
			const string __id = "setTextAppearance.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (p0);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

	}
}
