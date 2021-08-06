using System;
using UIKit;
using Kunicardus.Core.Models.DB;
using CoreGraphics;

namespace Kunicardus.Touch
{
	public class BindableUIWebView : UIWebView
	{
		public BindableUIWebView (CGRect frame) : base (frame)
		{
			this.Frame = frame;
		}

		private string _text;

		public string Text {
			get{ return _text; }
			set { 
				if (string.IsNullOrEmpty (value))
					return;

				_text = value;

				LoadHtmlString (_text, null);
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

