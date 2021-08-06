using System;
using System.IO;
using UIKit;
using Kunicardus.Core.ViewModels;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.BindingContext;
using System.Xml;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Kunicardus.Touch.Helpers.UI;
using System.Collections.Generic;

namespace Kunicardus.Touch
{
	public class NewsDetailsViewController  : BaseMvxViewController
	{
		#region Properties

		public new NewsDetailsViewModel ViewModel {
			get { return (NewsDetailsViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		#endregion

		#region Constructor Implementation

		public NewsDetailsViewController ()
		{
			HideMenuIcon = true;
		}

		#endregion

		#region Private Variables

		private BindableUIWebView _webView;
		private UILabel _date;
		private UIScrollView _scrollView;

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.NavigationBarHidden = false;
			View.BackgroundColor = UIColor.White;
			Title = ApplicationStrings.News;
			InitUI ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			NavigationController.NavigationBarHidden = true;
			base.ViewWillDisappear (animated);
		}

		#endregion

		#region Methods

		private void InitUI ()
		{
			var topPadding = 5f;
			var padding = 30f;
			_scrollView = new UIScrollView (new CGRect (0, 0, View.Frame.Width, View.Frame.Height));
			UILabel title = new UILabel (new CGRect (padding / 2, topPadding, View.Frame.Width - padding, 40f));
			title.TextColor = UIColor.Black;
			title.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 2);
			title.LineBreakMode = UILineBreakMode.WordWrap;
			title.Lines = 0;

			_date = new UILabel (new CGRect (padding / 2, title.Frame.Bottom + 5f, View.Frame.Width - padding, 15f));
			_date.TextColor = UIColor.Clear.FromHexString ("#707070");
			_date.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 5f);

			_webView = new BindableUIWebView (new CGRect (0, _date.Frame.Bottom + 5,
				View.Frame.Width, View.Frame.Height - _date.Frame.Bottom));
			_webView.LoadFinished += HTMLContentLoaded;
			_webView.ScrollView.AlwaysBounceVertical = false;

			_webView.BackgroundColor = UIColor.White;
			//_webView.ContentScaleFactor = 1.5f;
			_scrollView.AddSubview (title);
			_scrollView.AddSubview (_date);
			_scrollView.AddSubview (_webView);

			var set = this.CreateBindingSet<NewsDetailsViewController, NewsDetailsViewModel> ();
			set.Bind (title).To (vm => vm.Title);
			set.Bind (_date).To (vm => vm.Date).WithConversion ("NewsDate", "HH:MM:ss");
			set.Apply ();
			View.AddSubview (_scrollView);
			this.CreateBinding (_webView).For ("Text").To ((NewsDetailsViewModel vm) => vm.Description).Apply ();
			this.AddBindings (new Dictionary<object,string> (){ { _webView, "Text Description" } });
		}

		void HTMLContentLoaded (object sender, EventArgs e)
		{
			var height = _webView.EvaluateJavascript ("document.body.scrollHeight");
			var frame = _webView.Frame;
			frame.Height = (float)Convert.ToDouble (height);
			_webView.Frame = frame;
			var scFrame = _scrollView.Frame;
			scFrame.Height = View.Frame.Height;
			_scrollView.Frame = scFrame;
			_scrollView.ContentSize = new CGSize (View.Frame.Width, (float)Convert.ToDouble (height) + GetStatusBarHeight ());

			CGSize contentSize = _webView.ScrollView.ContentSize;
			CGSize viewSize = View.Bounds.Size;
			nfloat rw = viewSize.Width / contentSize.Width;

			_webView.ScrollView.MinimumZoomScale = rw;
			_webView.ScrollView.MaximumZoomScale = rw;
			_webView.ScrollView.ZoomScale = rw;
		}

		#endregion
	}
}

