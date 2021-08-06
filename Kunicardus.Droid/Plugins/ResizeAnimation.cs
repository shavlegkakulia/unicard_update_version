using Android.Views;
using Android.Widget;
using Android.Views.Animations;

namespace Kunicardus.Droid.Plugins
{
	public class ResizeAnimation : Animation
	{
		int startHeight;
		int targetHeight;
		View View;
		LayoutRules _rules;
		int? _View;
		int[] _margins;

		public ResizeAnimation (View View, int targetHeight, LayoutRules rules, int? Views, int[] margins)
		{
			this.View = View;
			this.targetHeight = targetHeight;
			startHeight = View.Height;
			_rules = rules;
			_View = Views;
			_margins = margins;
		}

		override protected void ApplyTransformation (float interpolatedTime, Transformation t)
		{
			int newHeight = (int)(startHeight + (targetHeight - startHeight) * interpolatedTime);

			RelativeLayout.LayoutParams parameters = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.WrapContent, targetHeight < 0 ? targetHeight : newHeight);
			if (_View.HasValue) {
				parameters.AddRule (_rules, _View.Value);
			} else {
				parameters.AddRule (_rules);
			}

			if (_margins != null) {
				parameters.SetMargins (_margins [0], _margins [1], _margins [2], _margins [3]);
			}
			View.LayoutParameters = parameters;
			//View.LayoutParameters.Height = newHeight;
			View.RequestLayout ();
		}

		override public void Initialize (int width, int height, int parentWidth, int parentHeight)
		{
			base.Initialize (width, height, parentWidth, parentHeight);
		}

		public override bool WillChangeBounds ()
		{
			return true;
		}
	}
}