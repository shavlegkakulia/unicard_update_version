using System;
using Kunicardus.Core.Plugins.UIDialogPlugin;
using UIKit;
using System.Threading;

namespace Kunicardus.Touch.Plugins.UIDialogPlugin
{
	public class TouchUIDialogPlugin : IUIDialogPlugin
	{
		
		public TouchUIDialogPlugin ()
		{
			
		}

		#region IUIDialogPlugin implementation

		public void ShowToast (string message)
		{						
			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
			if (app.Toast == null) {
				app.Toast = new Toast ();
				app.Toast.Tapped += delegate {
					UIView.Animate (0.3f, () => {
						app.Toast.Alpha = 0;								
					}, () => {
						app.Toast.Text = string.Empty;
						app.Toast.RemoveFromSuperview ();
					});
				};
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

					Thread.Sleep (5200); 
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

		public void ShowProgressDialog (string message)
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

		public void DismissProgressDialog ()
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

		#endregion
		
	}
}

