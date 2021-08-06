using System;
using Android.Webkit;
using Android.Content;
using Android.Util;
using Android.Runtime;

namespace Kunicardus.Droid
{
	[Register ("Kunicardus.Droid.BindableWebView")]
	public class BindableWebView : WebView
	{
		private string _text;

		public BindableWebView (Context context)
			: base (context)
		{
		}

		public BindableWebView (Context context, IAttributeSet attrs)
			: base (context, attrs)
		{
		}

		public string Text {
			get { return _text; }
			set {
				if (string.IsNullOrEmpty (value))
					return;
				_text = value;
				Settings.SetTextSize (WebSettings.TextSize.Smaller);
				LoadData (_text, "text/html; charset=utf-8", "utf-8");
				UpdatedHtmlContent ();
			}
		}

		public event EventHandler HtmlContentChanged;

		private void UpdatedHtmlContent ()
		{
			var handler = HtmlContentChanged;
			if (handler != null)
				handler (this, EventArgs.Empty);
		}
	}

}

