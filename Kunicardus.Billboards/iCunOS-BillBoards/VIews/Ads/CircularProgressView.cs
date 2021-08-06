using System;
using UIKit;
using CoreGraphics;
using CoreAnimation;
using Foundation;
using System.Timers;

namespace iCunOS.BillBoards
{
	public class CircularProgressView : UIView
	{
		#region Variables

		private Timer _timer;

		#endregion

		#region UI

		private CAShapeLayer _circle;
		private UILabel _secondsLabel;

		#endregion

		#region Properties

		private nfloat _lineWidth = 2;

		public nfloat LineWidh {
			get {
				return _lineWidth;
			}
			set {
				_lineWidth = value;
			}
		}

		public event EventHandler<bool> TimerFinished;

		private int _seconds;

		public int Seconds {
			get {
				return _seconds;
			}
			set {
				_seconds = value;
				_secondsLeft = value;
			}
		}

		private int _secondsLeft;

		#endregion

		#region Ctors

		public CircularProgressView (CGRect frame) : base (frame)
		{
			this.BackgroundColor = UIColor.Clear;

			UIView borderView = new UIView ();

			borderView.Frame = new CGRect (0, 0, frame.Width, frame.Height);
			borderView.BackgroundColor = UIColor.White;

			borderView.Layer.CornerRadius = frame.Width / 2.0f;
			borderView.Layer.BorderColor = UIColor.Clear.FromHexString ("#eeeeee").CGColor;
			borderView.Layer.BorderWidth = _lineWidth;
			this.AddSubview (borderView);

			_secondsLabel = new UILabel ();
			_secondsLabel.TextColor = UIColor.Clear.FromHexString (Styles.Colors.Red);
			_secondsLabel.Font = UIFont.SystemFontOfSize (32);
			_secondsLabel.Text = _secondsLeft.ToString ();
			_secondsLabel.TextAlignment = UITextAlignment.Center;
			_secondsLabel.SizeToFit ();
			_secondsLabel.Frame = new CGRect (0, 
				(frame.Height - _secondsLabel.Frame.Height) / 2.0f - 5,
				Frame.Width, _secondsLabel.Frame.Height);
			this.AddSubview (_secondsLabel);

			UILabel secondsTitle = new UILabel ();
			secondsTitle.TextColor = UIColor.Clear.FromHexString (Styles.Colors.Red);
			secondsTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 10);
			secondsTitle.Text = ApplicationStrings.Second;
			secondsTitle.TextAlignment = UITextAlignment.Center;
			secondsTitle.SizeToFit ();
			secondsTitle.Frame = new CGRect (0, _secondsLabel.Frame.Bottom - 2,
				Frame.Width, secondsTitle.Frame.Height);
			this.AddSubview (secondsTitle);


			_timer = new Timer (1000);
			_timer.Elapsed += TimerElapsed;
		}

		void TimerElapsed (object sender, ElapsedEventArgs e)
		{
			_secondsLeft--;
			UpdateSeconds (_secondsLeft);
			if (_secondsLeft <= 0) {
				_timer.Enabled = false;
				if (TimerFinished != null)
					TimerFinished (this, true);
			}
		}

		#endregion

		#region Methods

		public void UpdateSeconds (int value)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (() => {
				_secondsLabel.Text = value.ToString ();
			});
		}


		public void Reset ()
		{
			_timer.Enabled = false;
			if (_circle != null) {
				_circle.RemoveAllAnimations ();
				_circle.RemoveFromSuperLayer ();
				this.Layer.RemoveAllAnimations ();
			}
			_secondsLeft = _seconds;
			UpdateSeconds (_secondsLeft);
		}

		public void Start ()
		{
			_secondsLeft = _seconds;
			UpdateSeconds (_secondsLeft);

			if (_circle == null) {
				_circle = new CAShapeLayer ();
				_circle.Path = UIBezierPath.FromArc (
					new CGPoint (Frame.Width / 2.0f, Frame.Width / 2.0f),
					this.Frame.Width / 2.0f - _lineWidth / 2.0f, 
					2.0f * (nfloat)Math.PI * 0 - (nfloat)(Math.PI / 2.0f),
					2f * (nfloat)Math.PI * 1f - (nfloat)(Math.PI / 2.0f), true).CGPath;
				_circle.FillColor = UIColor.Clear.CGColor;
				_circle.StrokeColor = UIColor.Clear.FromHexString (Styles.Colors.Red).CGColor;
				_circle.LineWidth = _lineWidth;
			}

			CABasicAnimation animation = CABasicAnimation.FromKeyPath ("strokeEnd");
			animation.Duration = _seconds;
			animation.RemovedOnCompletion = false;
			animation.From = NSNumber.FromInt32 (0);
			animation.To = NSNumber.FromInt32 (1);
			animation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.Linear);
			_circle.AddAnimation (animation, "drawCircleAnimation");

			this.Layer.AddSublayer (_circle);

			_timer.Enabled = true;
		}

		#endregion

	}
}

