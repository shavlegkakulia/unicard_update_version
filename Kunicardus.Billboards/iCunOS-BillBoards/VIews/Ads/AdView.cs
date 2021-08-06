using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Billboards.Core.ViewModels;
using iCunOS.BillBoards;
using CoreText;
using Foundation;
using Autofac;
using System.Runtime.InteropServices;
using Kunicardus.Billboards.Core.DbModels;
using System.Security.Policy;
using Google.Maps;
using Newtonsoft.Json.Serialization;
using iCunOS.BillBoards.Plugins.Connectivity;
using Kunicardus.Billboards.Core.Helpers;

namespace iCunOS.BillBoards
{
	public class AdView : UIView
	{
		#region Variables

		public static CGSize DefaultSize;
		private nfloat pointIndicatorHeight = 110;
		private float progressHeight = 80f;
		private AdvertismentViewModel _viewModel;
		private bool _startProgressIsAdded, _downloadAdViewIsAdded, _overlayIsAdded, _seeOrRectViewIsAdded, _notAccumulatedViewIsAdded;
		private nfloat _alertHeight = 100f;
		private nfloat _padding = 30f;

		#endregion

		#region UI

		private UIImageView _image;
		public CircularProgressView _progress;
		private UIView _overlay;
		private UIView _notAccumulatedView;
		private UIView _startProgressView;
		private UIView _seeOrRejectView;
		private UIView _downloadAdView;
		private UILabel _noConnection;
		private UILabel _month;
		private UILabel _hours;
		private UIView _topPointsView;
		private UIView _errorView;
		private UIView _tickView;

		#endregion

		#region Properties

		public AdvertismentViewModel ViewModel {
			get {
				return _viewModel;
			}
			set {
				_viewModel = value;
				if (_downloadAdViewIsAdded) {
					_hours.Text = ViewModel.Advertisment.PassDate.ToString ("HH:mm");
					_month.Text = ViewModel.Advertisment.PassDate.ToGeoString ();
					_month.SizeToFit ();
					_month.Frame = new CGRect (
						(_downloadAdView.Frame.Width - _month.Frame.Width) / 2,
						_month.Frame.Y,
						_month.Frame.Width,
						_month.Frame.Height
					);
					_hours.SizeToFit ();
					_hours.Frame = new CGRect (
						(_downloadAdView.Frame.Width - _hours.Frame.Width) / 2,
						_month.Frame.Bottom,
						_hours.Frame.Width,
						_hours.Frame.Height
					);
				}
			}
		}

		private bool _isActive;

		public bool IsActive {
			get { return _isActive; }
			set { 
				if (value) {
					_isActive = true;
					if (_progress != null) {
						_progress.Reset ();
						_progress.RemoveFromSuperview ();
					}
					if (ViewModel != null) {
						if (ViewModel.Advertisment != null)
						if (ViewModel.Advertisment.Status == Kunicardus.Billboards.Core.Enums.AdvertismentStatus.Loaded) {
							if (_downloadAdView != null && _downloadAdViewIsAdded) {
								_downloadAdView.RemoveFromSuperview ();
								_downloadAdViewIsAdded = false;
							}
							if (!_seeOrRectViewIsAdded) {
								SeeOrRejectAdView ();
							}
						} else if (ViewModel.Advertisment.Status == Kunicardus.Billboards.Core.Enums.AdvertismentStatus.PointsAcumulated) {
							if (_downloadAdView != null && _downloadAdViewIsAdded) {
								_downloadAdView.RemoveFromSuperview ();
								_downloadAdViewIsAdded = false;
							}
							if (!_overlayIsAdded) {
								OverlayView ();
							}
						} else if (ViewModel.Advertisment.Status == Kunicardus.Billboards.Core.Enums.AdvertismentStatus.Seen) {
							if (_downloadAdView != null && _downloadAdViewIsAdded) {
								_downloadAdView.RemoveFromSuperview ();
								_downloadAdViewIsAdded = false;
							}
							if (!_notAccumulatedViewIsAdded) {
								NotAccumulatedView ();
								_notAccumulatedViewIsAdded = true;
							}
						}
					}
				} else {
					_isActive = false;
					if (_progress != null) {
						_progress.Reset ();
						_progress.RemoveFromSuperview ();
					}
					if (ViewModel != null) {
						if (ViewModel.Advertisment != null)
						if (ViewModel.Advertisment.Status == Kunicardus.Billboards.Core.Enums.AdvertismentStatus.Loaded && _startProgressView != null) {
							if (_errorView != null)
								_errorView.RemoveFromSuperview ();
							if (_topPointsView != null)
								_topPointsView.RemoveFromSuperview ();
							AddSubview (_seeOrRejectView);
							AddSubview (_topPointsView);
							_seeOrRectViewIsAdded = true;

						}
					}
				}
			}
		}

		public event EventHandler<bool> OnSkip;

		#endregion

		#region Constructor

		public AdView (CGRect frame) : base (frame)
		{
			BackgroundColor = UIColor.Clear;
			nfloat padding = 20f;
			var heightPadding = 40f;
			if (!Screen.IsTall) {
				heightPadding = 20f;
				padding = 27f;
			}
			_downloadAdView = new UIView (
				new CGRect (padding, 
					pointIndicatorHeight / 2.0f, 
					frame.Width - padding * 2, 
					frame.Height - pointIndicatorHeight / 2.0f - progressHeight / 2.0f - heightPadding));
			
			DownloadAdView ();

		}

		#endregion

		#region Events

		void RejectToWatchAd (object sender, EventArgs e)
		{
			UIAlertView alert = new UIAlertView (new CGRect (_padding, (Frame.Height - _alertHeight) / 2, _downloadAdView.Frame.Width - _padding * 2, _alertHeight));
			alert.AddButton ("დიახ");
			alert.AddButton ("არა");
			alert.Message = ApplicationStrings.RejectToWatchAd;

			alert.CancelButtonIndex = 1;

			alert.Clicked += AlertClicked;
			alert.Show ();
		}

		void RejectMoreAd (object sender, EventArgs e)
		{
			UIAlertView alert = new UIAlertView (new CGRect (_padding, (Frame.Height - _alertHeight) / 2, _downloadAdView.Frame.Width - _padding * 2, _alertHeight));
			alert.AddButton ("დიახ");
			alert.AddButton ("არა");
			alert.Message = ApplicationStrings.SkipMoreAd;

			alert.CancelButtonIndex = 1;

			alert.Clicked += AlertClicked;
			alert.Show ();
		}

		void DontDownload (object sender, EventArgs e)
		{
			UIAlertView alert = new UIAlertView (new CGRect (_padding, (Frame.Height - _alertHeight) / 2, _downloadAdView.Frame.Width - _padding * 2, _alertHeight));
			alert.AddButton ("დიახ");
			alert.AddButton ("არა");
			alert.Message = ApplicationStrings.DontDownloadAd;

			alert.CancelButtonIndex = 1;

			alert.Clicked += AlertClicked;
			alert.Show ();

		}

		void AlertClicked (object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex != 1) {
				ViewModel.SkipTheAd ();
				if (OnSkip != null)
					this.OnSkip (this, true);
			}
		}

		void TimerFinished (object sender, bool e)
		{
			InvokeOnMainThread (() => {
				if (_startProgressView != null && _startProgressIsAdded) {
					_startProgressView.RemoveFromSuperview ();
					_progress.RemoveFromSuperview ();
					_image.RemoveFromSuperview ();
					_topPointsView.RemoveFromSuperview ();
					if (_errorView != null)
						_errorView.RemoveFromSuperview ();
					_startProgressIsAdded = false;
				}

				TouchConnectivityPlugin connectivity = new TouchConnectivityPlugin ();
				if (connectivity.IsNetworkReachable) {
					var response = ViewModel.SavePoints ();
					if (response.Successful) {
						OverlayView ();
					} else {
						NotAccumulatedView ();
					}
				} else {
					DialogPlugin.ShowToast (ApplicationStrings.NoInternetConnection);
					_progress.Reset ();
					NotAccumulatedView ();
				}
			});
		}

		void Load (object sender, EventArgs e)
		{
			TouchConnectivityPlugin connectivity = new TouchConnectivityPlugin ();
			if (connectivity.IsNetworkReachable) {
				if (ViewModel != null) {
					InvokeOnMainThread (() => DialogPlugin.ShowCustomDialog (_downloadAdView.Frame));
					var response = ViewModel.Load ();
					InvokeOnMainThread (() => DialogPlugin.DismissProgressDialog ());
					if (response.Successful) {
						if (_downloadAdView != null) {
							_downloadAdView.RemoveFromSuperview ();
							_downloadAdViewIsAdded = true;
						}
						SeeOrRejectAdView ();
					} else {
						DialogPlugin.ShowToast ("მოხდა შეცდომა სერვერთან კავშირისას");
					}
				}
			} else {
				DialogPlugin.ShowToast (ApplicationStrings.NoInternetConnectionTryLater);
				_downloadAdView.AddSubview (_noConnection);
			}
		}

		void StartProgress (object sender, EventArgs e)
		{
			if (_seeOrRejectView != null && _seeOrRectViewIsAdded) {
				_seeOrRejectView.RemoveFromSuperview ();
				_seeOrRectViewIsAdded = false;
			}
			//StartProgressView ();
			AddSubview (_progress);
			_progress.Start ();
		}

		void SkipTheAd (object sender, EventArgs e)
		{
			ViewModel.SkipTheAd ();
			if (OnSkip != null)
				this.OnSkip (this, true);
		}

		void MorePointsClicked (object sender, EventArgs e)
		{
			UIApplication.SharedApplication.OpenUrl (new NSUrl (ViewModel.Advertisment.ExternalLink));
		}

		void TryAgainAccumulation (object sender, EventArgs e)
		{
			InvokeOnMainThread (() => {
				if (_startProgressView != null && _startProgressIsAdded) {
					_startProgressView.RemoveFromSuperview ();
					_progress.RemoveFromSuperview ();
					_image.RemoveFromSuperview ();
					_topPointsView.RemoveFromSuperview ();
					if (_errorView != null)
						_errorView.RemoveFromSuperview ();
					_seeOrRectViewIsAdded = false;
				}
			
				var response = ViewModel.SavePoints ();
				if (response.Successful) {
					_notAccumulatedView.RemoveFromSuperview ();
					_notAccumulatedViewIsAdded = true;
					OverlayView ();
					if (_errorView != null)
						_errorView.RemoveFromSuperview ();
				} else {
					DialogPlugin.ShowToast (ApplicationStrings.NotAccumulatedNotification);
				}
			});
		}

		#endregion

		#region Methods

		private void SeeOrRejectAdView ()
		{
			InvokeOnMainThread (() => {
				
				StartProgressView (true);

				_seeOrRejectView = new UIView (_downloadAdView.Frame);
				_seeOrRejectView.Layer.BorderWidth = 7;
				_seeOrRejectView.Layer.BorderColor = UIColor.Clear.FromHexString ("#D4D4D4").CGColor;
				_seeOrRejectView.BackgroundColor = UIColor.Clear.FromHexString ("#ffffff", 0.93f);
				_seeOrRejectView.Layer.CornerRadius = 10;

				var seeAdLabel = new UILabel ();
				_seeOrRejectView.AddSubview (seeAdLabel);
				seeAdLabel.LineBreakMode = UILineBreakMode.WordWrap;
				seeAdLabel.Lines = 0;
				seeAdLabel.Text = String.Format ("აღნიშნული რეკლამის ყურების \n შემდეგ თქვენ  დაგერიცხებათ \n{0} ქულა", ViewModel.Advertisment.Points);
				seeAdLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 4f);
				seeAdLabel.SizeToFit ();

				var top = 100f;
				if (!Screen.IsTall)
					top = 60f;
				seeAdLabel.Frame = new CGRect (0, top, _seeOrRejectView.Frame.Width, seeAdLabel.Frame.Height);
				seeAdLabel.TextAlignment = UITextAlignment.Center;
				seeAdLabel.TextColor = UIColor.Black;	

				UIButton seeAdButton = new UIButton (UIButtonType.System);
				seeAdButton.SetTitle (ApplicationStrings.WatchTheAd, UIControlState.Normal);
				seeAdButton.TintColor = UIColor.White;
				seeAdButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize);
				seeAdButton.BackgroundColor = UIColor.Clear.FromHexString ("#C8C54B");
				seeAdButton.Layer.BorderWidth = 2f;
				seeAdButton.Layer.BorderColor = UIColor.Clear.FromHexString ("#E3E061").CGColor;
				seeAdButton.Layer.CornerRadius = 10;
				seeAdButton.Frame = new CGRect (20f, seeAdLabel.Frame.Bottom + 25f, _seeOrRejectView.Frame.Width - 40f, 50f);
				seeAdButton.TouchUpInside += StartProgress;

				_seeOrRejectView.AddSubview (seeAdButton);

				UIButton reject = new UIButton (UIButtonType.System);
				reject.TintColor = UIColor.White;
				reject.SetAttributedTitle (new NSAttributedString (ApplicationStrings.SayReject, underlineStyle: NSUnderlineStyle.Single), UIControlState.Normal);
				reject.TitleLabel.TextColor = UIColor.Clear.FromHexString ("#C8C54B");
				reject.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 4f);
				reject.Frame = new CGRect (30f, seeAdButton.Frame.Bottom + 18f, _seeOrRejectView.Frame.Width - 60f, 50f);
				reject.TintColor = UIColor.White;
				reject.TouchUpInside += RejectToWatchAd;
				_seeOrRejectView.AddSubview (reject);

				if (!_seeOrRectViewIsAdded) {
					AddSubview (_seeOrRejectView);
					_seeOrRectViewIsAdded = true;
				}

				AddSubview (_topPointsView);
			});
		}

		private void StartProgressView (bool asOverlay = false)
		{
			InvokeOnMainThread (() => {
				_startProgressView = new UIView (_downloadAdView.Frame);
				_startProgressView.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.Blue);
				_startProgressView.Layer.BorderWidth = 7;
				_startProgressView.Layer.BorderColor = UIColor.Clear.FromHexString ("#e2e2e2").CGColor;
				_startProgressView.Layer.CornerRadius = 11;
				_startProgressView.Layer.ShadowOffset = new CGSize (10, 10);
				_startProgressView.Layer.ShadowColor = UIColor.Clear.FromHexString (Styles.Colors.DarkGray).CGColor;
				_startProgressView.Layer.ShadowRadius = 10;
				_startProgressView.Layer.ShadowOpacity = 0.8f;

				var topViewWidth = _startProgressView.Frame.Width / 3 - 3f;
				var topViewHeight = 35f;
				_topPointsView = new UIView (new CGRect ((Frame.Width - topViewWidth) / 2,
					_startProgressView.Frame.Y - topViewHeight / 2,
					topViewWidth,
					topViewHeight));
				_topPointsView.BackgroundColor = UIColor.White;
				_topPointsView.Layer.CornerRadius = 7f;
				_topPointsView.Layer.BorderWidth = 3f;
				_topPointsView.Layer.BorderColor = UIColor.Gray.FromHexString ("#ebebeb").CGColor;	

				UILabel points = new UILabel ();
				points.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize + 2f);
				points.Text = ViewModel.Advertisment.Points.ToString (); //ViewModel.Advertisment.Points;
				points.SizeToFit ();
				points.Frame = new CGRect (0f, (topViewHeight - points.Frame.Height) / 2, points.Frame.Width, points.Frame.Height);
				points.TextColor = UIColor.Clear.FromHexString (Styles.Colors.Red);

				UILabel pointText = new UILabel ();
				pointText.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 4f);
				pointText.Text = ApplicationStrings.Point;
				pointText.TextColor = UIColor.Clear.FromHexString (Styles.Colors.Red);
				pointText.SizeToFit ();

				var pointsCenterFrame = new CGRect (
					                        (_topPointsView.Frame.Width - (points.Frame.Width + pointText.Frame.Width)) / 2f,
					                        (topViewHeight - points.Frame.Height) / 2,
					                        points.Frame.Width,
					                        points.Frame.Height
				                        );
				points.Frame = pointsCenterFrame;
				pointText.Frame = new CGRect (points.Frame.Right, 
					points.Frame.Bottom - pointText.Frame.Height - 1f, 
					pointText.Frame.Width, 
					pointText.Frame.Height);
				
				_topPointsView.AddSubview (pointText);

				_topPointsView.AddSubview (points);

				_image = new UIImageView (new CGRect (
					_startProgressView.Frame.X + 6.5f,
					_startProgressView.Frame.Y + 6.5f,
					_startProgressView.Frame.Width - 13f,
					_startProgressView.Frame.Height - 13f));

				if (ViewModel.Advertisment.Image != null) {
					using (var url = new NSUrl (ViewModel.Advertisment.Image))
					using (var data = NSData.FromUrl (url)) {
						_image.Image = UIImage.LoadFromData (new NSData (ViewModel.Advertisment.Image, NSDataBase64DecodingOptions.None));
					}
				}

				_progress = new CircularProgressView (new CGRect ((Frame.Width - progressHeight) / 2.0f, 
					_startProgressView.Frame.Bottom - progressHeight / 2, 
					progressHeight, 
					progressHeight));
				_progress.TimerFinished += TimerFinished;
				_startProgressView.AddSubview (_image);
				if (!_startProgressIsAdded) {
					AddSubview (_startProgressView);
					AddSubview (_image);
					if (!asOverlay) {
						AddSubview (_progress);
						AddSubview (_topPointsView);
					}
					_startProgressIsAdded = true;
				}
				_progress.Seconds = ViewModel.Advertisment.TimeOut;
				if (!asOverlay) {
					_progress.Start ();
				}
			});
		}

		private void DownloadAdView ()
		{
			InvokeOnMainThread (() => {
				_downloadAdView.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.Blue);
				_downloadAdView.Layer.BorderWidth = 7;
				_downloadAdView.Layer.BorderColor = UIColor.Clear.FromHexString ("#e2e2e2").CGColor;
				_downloadAdView.Layer.CornerRadius = 10;
				_downloadAdView.Layer.ShadowOffset = new CGSize (10, 10);
				_downloadAdView.Layer.ShadowColor = UIColor.Clear.FromHexString (Styles.Colors.DarkGray).CGColor;
				_downloadAdView.Layer.ShadowRadius = 10;
				_downloadAdView.Layer.ShadowOpacity = 0.8f;

				var top = 45f;
				if (!Screen.IsTall)
					top = 25f;
				_noConnection = new UILabel (new CGRect (0f, top, _downloadAdView.Frame.Width, 20f));
				_noConnection.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 7f);
				_noConnection.Text = ApplicationStrings.NoInternetConnection;
				_noConnection.TextAlignment = UITextAlignment.Center;
				_noConnection.TextColor = UIColor.White;

				UILabel downloadAd = new UILabel ();
				if (Screen.IsTall)
					downloadAd.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize + 2f);
				else
					downloadAd.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize + 1f);
				downloadAd.Text = ApplicationStrings.DownloadAd;
				downloadAd.TextAlignment = UITextAlignment.Center;
				downloadAd.Lines = 0;
				downloadAd.LineBreakMode = UILineBreakMode.WordWrap;
				downloadAd.SizeToFit ();
				downloadAd.TextColor = UIColor.White;
				downloadAd.Frame = new CGRect ((_downloadAdView.Frame.Width - downloadAd.Frame.Width) / 2,
					_noConnection.Frame.Bottom,
					downloadAd.Frame.Width,
					downloadAd.Frame.Height);
				_downloadAdView.AddSubview (downloadAd);

				nfloat downloadDiameter = 90f;
				if (!Screen.IsTall)
					downloadDiameter = 70f;
				UIButton downloadButton = new UIButton (UIButtonType.System);
				downloadButton.Frame = new CGRect ((_downloadAdView.Frame.Width - downloadDiameter) / 2,
					downloadAd.Frame.Bottom + 7f,
					downloadDiameter,
					downloadDiameter);
				downloadButton.Layer.CornerRadius = downloadDiameter / 2;
				downloadButton.Layer.BorderWidth = 3f;
				downloadButton.Layer.BorderColor = UIColor.Clear.FromHexString ("##E3E061").CGColor;
				downloadButton.BackgroundColor = UIColor.Clear.FromHexString ("#C8C54B");
				downloadButton.SetImage (UIImage.FromBundle ("download.png"), UIControlState.Normal);
				downloadButton.TintColor = UIColor.White;
				downloadButton.ImageEdgeInsets = new UIEdgeInsets (15, 20, 20, 20);
				downloadButton.TouchUpInside += Load;
				_downloadAdView.AddSubview (downloadButton);

				UIButton dontDownload = new UIButton (UIButtonType.System);
				dontDownload.TintColor = UIColor.White;
				dontDownload.SetAttributedTitle (new NSAttributedString (ApplicationStrings.Abolish, underlineStyle: NSUnderlineStyle.Single), UIControlState.Normal);
				dontDownload.TitleLabel.TextColor = UIColor.Clear.FromHexString ("#C8C54B");
				dontDownload.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 7f);
				dontDownload.Frame = new CGRect (30f, downloadButton.Frame.Bottom, _downloadAdView.Frame.Width - 60f, 30f);
				dontDownload.TintColor = UIColor.White;
				dontDownload.TouchUpInside += DontDownload;
				_downloadAdView.AddSubview (dontDownload);

				UILabel billboardDateTitle = new UILabel ();
				billboardDateTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 9f);
				billboardDateTitle.Text = ApplicationStrings.BilboardPassDate;
				billboardDateTitle.SizeToFit ();
				billboardDateTitle.Frame = new CGRect (
					(_downloadAdView.Frame.Width - billboardDateTitle.Frame.Width) / 2,
					dontDownload.Frame.Bottom + 25f,
					billboardDateTitle.Frame.Width,
					billboardDateTitle.Frame.Height
				);
				billboardDateTitle.TextColor = UIColor.White;
				_downloadAdView.AddSubview (billboardDateTitle);

				_month = new UILabel ();
				_month.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 7);
				_month.Frame = new CGRect (
					(_downloadAdView.Frame.Width - _month.Frame.Width) / 2,
					billboardDateTitle.Frame.Bottom + 1f,
					_month.Frame.Width,
					_month.Frame.Height
				);
				_month.TextColor = UIColor.White;
				_downloadAdView.AddSubview (_month);

				_hours = new UILabel ();
				_hours.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 7);
				_hours.TextColor = UIColor.White;
				_downloadAdView.AddSubview (_hours);

				if (!_downloadAdViewIsAdded) {
					AddSubview (_downloadAdView);
					_downloadAdViewIsAdded = true;
				}
			});
		}

		private void OverlayView ()
		{
			InvokeOnMainThread (() => {
				if (!string.IsNullOrWhiteSpace (ViewModel.Advertisment.ExternalLink)) {
					_overlay = new UIView (_downloadAdView.Frame);
					_overlay.Layer.BorderWidth = 7;
					_overlay.Layer.BorderColor = UIColor.Clear.FromHexString ("#D4D4D4").CGColor;
					_overlay.BackgroundColor = UIColor.Clear.FromHexString ("#ffffff", 0.85f);
					_overlay.Layer.CornerRadius = 10;
				
					var morePoints = new UILabel ();
					_overlay.AddSubview (morePoints);
					morePoints.Lines = 0;
					morePoints.LineBreakMode = UILineBreakMode.WordWrap;
					morePoints.Text = String.Format ("გილოცავთ, თვენ დაგერიცხათ \n {0} ქულა!", _viewModel.Advertisment.Points);
					if (Screen.IsTall)
						morePoints.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 3f);
					else
						morePoints.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 4f);
					morePoints.SizeToFit ();
					var top = 80f;
					if (!Screen.IsTall)
						top = 40f;
					morePoints.Frame = new CGRect (0, top, _overlay.Frame.Width, morePoints.Frame.Height);
					morePoints.TextAlignment = UITextAlignment.Center;
					morePoints.TextColor = UIColor.Black;	

					var clickTheButton = new UILabel ();
					_overlay.AddSubview (clickTheButton);
					clickTheButton.Text = ApplicationStrings.ForMorePoints;
					clickTheButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 7f);
					clickTheButton.Lines = 0;
					clickTheButton.LineBreakMode = UILineBreakMode.WordWrap;
					clickTheButton.SizeToFit ();
					clickTheButton.TextAlignment = UITextAlignment.Center;
					clickTheButton.Frame = new CGRect (0f, morePoints.Frame.Bottom + 5f, _overlay.Frame.Width, clickTheButton.Frame.Height);
					clickTheButton.TextColor = UIColor.Black;	

					UIButton moreButton = new UIButton (UIButtonType.System);
					moreButton.SetTitle (ApplicationStrings.MorePoints, UIControlState.Normal);
					moreButton.TintColor = UIColor.White;
					moreButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize);
					moreButton.BackgroundColor = UIColor.Clear.FromHexString ("#C8C54B");
					moreButton.Layer.BorderWidth = 2f;
					moreButton.Layer.BorderColor = UIColor.Clear.FromHexString ("#E3E061").CGColor;
					moreButton.Layer.CornerRadius = 10;
					moreButton.Frame = new CGRect (20f, clickTheButton.Frame.Bottom + 25f, _overlay.Frame.Width - 40f, 50f);
					moreButton.TouchUpInside += MorePointsClicked;
					_overlay.AddSubview (moreButton);

					UIButton skip = new UIButton (UIButtonType.System);
					skip.TintColor = UIColor.White;
					skip.SetAttributedTitle (new NSAttributedString (ApplicationStrings.SayReject, underlineStyle: NSUnderlineStyle.Single), UIControlState.Normal);
					skip.TitleLabel.TextColor = UIColor.Clear.FromHexString ("#C8C54B");
					skip.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 4f);
					skip.Frame = new CGRect (30f, moreButton.Frame.Bottom + 18f, _overlay.Frame.Width - 60f, 50f);
					skip.TintColor = UIColor.White;
					skip.TouchUpInside += RejectMoreAd;
					_overlay.AddSubview (skip);

					nfloat tickViewDiameter = progressHeight;
					_tickView = new UIView (new CGRect ((Frame.Width - tickViewDiameter) / 2,
						_overlay.Frame.Bottom - tickViewDiameter / 2,
						tickViewDiameter,
						tickViewDiameter));
					_tickView.BackgroundColor = UIColor.White;
					_tickView.Layer.BorderWidth = 4f;
					_tickView.Layer.BorderColor = UIColor.Clear.FromHexString ("#C8C54B").CGColor;
					_tickView.Layer.CornerRadius = tickViewDiameter / 2f;


					nfloat imageSize = 40f;
					UIImageView tick = new UIImageView (new CGRect ((tickViewDiameter - imageSize) / 2, (tickViewDiameter - imageSize) / 2, imageSize, imageSize));
					tick.Image = UIImage.FromBundle ("tick.png");
					_tickView.AddSubview (tick);


					if (!_overlayIsAdded) {
						AddSubview (_overlay);
						_overlayIsAdded = true;
						AddSubview (_tickView);
					}
				} else {
					UIAlertView alert = new UIAlertView (new CGRect (_padding, (Frame.Height - _alertHeight) / 2, 150f, 150f));
					alert.AddButton ("გასაგებია");
					alert.Message = String.Format ("გილოცავთ, თქვენ დაგერიცხათ {0} ქულა", ViewModel.Advertisment.Points);
					alert.Clicked += SkipTheAd;
					alert.Show ();
				}
			});
		}

		private void NotAccumulatedView ()
		{
			InvokeOnMainThread (() => {
				_notAccumulatedView = new UIView (_downloadAdView.Frame);
				_notAccumulatedView.Layer.BorderWidth = 7;
				_notAccumulatedView.Layer.BorderColor = UIColor.Clear.FromHexString ("#D4D4D4").CGColor;
				_notAccumulatedView.BackgroundColor = UIColor.Clear.FromHexString ("#ffffff", 0.85f);
				_notAccumulatedView.Layer.CornerRadius = 10;

				var tryAgainLabel = new UILabel ();
				_notAccumulatedView.AddSubview (tryAgainLabel);
				tryAgainLabel.LineBreakMode = UILineBreakMode.WordWrap;
				tryAgainLabel.Lines = 0;
				tryAgainLabel.Text = ApplicationStrings.NotAccumulated;
				if (Screen.IsTall)
					tryAgainLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 5f);
				else
					tryAgainLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize);
				tryAgainLabel.SizeToFit ();
				var top = 100f;
				if (!Screen.IsTall)
					top = 40f;
				tryAgainLabel.Frame = new CGRect (0, top, _notAccumulatedView.Frame.Width, tryAgainLabel.Frame.Height);
				tryAgainLabel.TextAlignment = UITextAlignment.Center;
				tryAgainLabel.TextColor = UIColor.Black;	

				UIButton tryAgainButton = new UIButton (UIButtonType.System);
				tryAgainButton.SetTitle (ApplicationStrings.AccumulatePoints, UIControlState.Normal);
				tryAgainButton.TintColor = UIColor.White;
				tryAgainButton.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize);
				tryAgainButton.BackgroundColor = UIColor.Clear.FromHexString ("#C8C54B");
				tryAgainButton.Layer.BorderWidth = 2f;
				tryAgainButton.Layer.BorderColor = UIColor.Clear.FromHexString ("#E3E061").CGColor;
				tryAgainButton.Layer.CornerRadius = 10;
				tryAgainButton.Frame = new CGRect (20f, tryAgainLabel.Frame.Bottom + 25f, _notAccumulatedView.Frame.Width - 40f, 50f);
				tryAgainButton.TouchUpInside += TryAgainAccumulation;

				_notAccumulatedView.AddSubview (tryAgainButton);

				nfloat errorDiameter = progressHeight;
				_errorView = new UIView (new CGRect ((Frame.Width - progressHeight) / 2.0f, 
					_notAccumulatedView.Frame.Bottom - progressHeight / 2, 
					progressHeight, 
					progressHeight));
				_errorView.BackgroundColor = UIColor.White;
				_errorView.Layer.BorderWidth = 3f;
				_errorView.Layer.BorderColor = UIColor.Clear.FromHexString (Styles.Colors.Red).CGColor;
				_errorView.Layer.CornerRadius = errorDiameter / 2f;
				_notAccumulatedView.AddSubview (_errorView);

				nfloat imageSize = 45f;
				UIImageView error = new UIImageView (new CGRect ((errorDiameter - imageSize) / 2, (errorDiameter - imageSize) / 2, imageSize, imageSize));
				error.Image = UIImage.FromBundle ("error.png");

				_errorView.AddSubview (error);


				UIButton reject = new UIButton (UIButtonType.System);
				reject.SetTitle (ApplicationStrings.SayReject, UIControlState.Normal);
				reject.Frame = new CGRect (0, tryAgainButton.Frame.Bottom + 30f, _notAccumulatedView.Frame.Width, 20f);
				reject.TintColor = UIColor.White;
				reject.SetAttributedTitle (new NSAttributedString (ApplicationStrings.SayReject, underlineStyle: NSUnderlineStyle.Single), UIControlState.Normal);
				reject.TitleLabel.TextColor = UIColor.Clear.FromHexString ("#C8C54B");
				reject.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 4f);
				reject.TouchUpInside += RejectToWatchAd;
				_notAccumulatedView.AddSubview (reject);

				if (!_notAccumulatedViewIsAdded) {
					AddSubview (_notAccumulatedView);
					_notAccumulatedViewIsAdded = true;
				}
				AddSubview (_errorView);
			});
		}

		#endregion
	}
}

