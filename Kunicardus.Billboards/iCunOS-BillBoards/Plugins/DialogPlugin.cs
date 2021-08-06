using System;
using UIKit;
using System.Threading;
using CoreGraphics;

namespace iCunOS.BillBoards
{
	public static class DialogPlugin
	{
		public static void ShowToast (string message)
		{						
			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
			if (app.Toast == null) {
				app.Toast = new Toast ();
			}
			if (!string.IsNullOrWhiteSpace (app.Toast.Text)) {
				return;
			}
			app.Toast.Text = message;
			app.Toast.Alpha = 0;
			app.Toast.RemoveFromSuperview ();
			app.Window.AddSubview (app.Toast);
			UIView.Animate (0.3f, 
				() => {
					app.Toast.Alpha = 1;
				}, () => {
				System.Threading.Tasks.Task.Factory.StartNew (() => {
					Thread.Sleep (1800); 
					UIApplication.SharedApplication.InvokeOnMainThread (() => {
						UIView.Animate (1, () => {
							app.Toast.Alpha = 0;								
						}, () => {
							app.Toast.Text = string.Empty;
							app.Toast.RemoveFromSuperview ();
						});
					});
				});
			});

		}

		public static void ShowProgressDialog (string message)
		{
			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
			app.ActivityIndicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.WhiteLarge);//[[UIActivityIndicatorView alloc]initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
			app.ActivityIndicator.BackgroundColor = UIColor.Gray;
			app.ActivityIndicator.Alpha = 0.5f;
			app.ActivityIndicator.Frame = app.Window.Frame;
			app.ActivityIndicator.Center = app.Window.Center;   
			app.Window.AddSubview (app.ActivityIndicator);
			app.ActivityIndicator.StartAnimating ();
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
		}

		public static void ShowCustomDialog (CGRect frame)
		{
			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
			app.ActivityIndicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.WhiteLarge);//[[UIActivityIndicatorView alloc]initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
			app.ActivityIndicator.BackgroundColor = UIColor.Gray;
			app.ActivityIndicator.Alpha = 0.5f;
			app.ActivityIndicator.Frame = frame;
			app.ActivityIndicator.Layer.CornerRadius = 10;
			app.ActivityIndicator.Center = app.Window.Center;   
			app.Window.AddSubview (app.ActivityIndicator);
			app.ActivityIndicator.StartAnimating ();
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
		}

		public static void DismissProgressDialog ()
		{
			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
			if (app.ActivityIndicator != null) {
				app.ActivityIndicator.StopAnimating ();
				app.ActivityIndicator.RemoveFromSuperview ();
				app.ActivityIndicator.Dispose ();
				app.ActivityIndicator = null;
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			}
		}
	}
}

