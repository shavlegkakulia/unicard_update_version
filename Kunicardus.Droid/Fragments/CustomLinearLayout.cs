using System;
using Android.Widget;
using Android.Graphics;
using Android.Content;
using Android.Views;
using Android.Runtime;
using Android.Util;

namespace Kunicardus.Droid
{
	[Register ("Kunicardus.Droid.CustomLinearLayout")]
	public class CustomLinearLayout: LinearLayout
	{
		private bool isDisable;
		Path path;
		RectF r;
		Paint paint;

		private void init ()
		{            
			r = new RectF (0, 0, Width, Height);
			paint = new Paint (PaintFlags.AntiAlias);
			paint.SetShadowLayer (1000.0f, 0.0f, 2.0f, Color.DarkSlateGray);
		}

		public CustomFrameLayout (Context context)
			: base (context)
		{
			init ();
		}

		public CustomFrameLayout (Context context, IAttributeSet attrs)
			: base (context, attrs)
		{
			init ();
		}

		override public bool OnInterceptTouchEvent (MotionEvent evt)
		{
			if (evt.Action == MotionEventActions.Down && (evt.RawX <= 50 && !isDisable))
				return true;
			return isDisable;
		}

		public void DisableChildTouch (bool isDisable)
		{
			this.isDisable = isDisable;
		}

		public void setRounded (bool flag)
		{            
			r.Top = 0;
			r.Left = 0;
			r.Right = Width;
			r.Bottom = Height;
			if (flag) {
				path = new Path ();
				path.AddRoundRect (r, 30, 30, Path.Direction.Cw);
			} else {
				path = new Path ();
				path.AddRoundRect (r, 0, 0, Path.Direction.Cw);
			}
			Invalidate ();
		}

		public override void Draw (Canvas canvas)
		{
			if (path != null) {
				canvas.ClipPath (path);
				//canvas.DrawPath(path, paint);
			}
			base.Draw (canvas);
		}
	}
}

