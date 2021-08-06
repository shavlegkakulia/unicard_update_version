using System;
using Cirrious.MvvmCross.Touch.Views;
using UIKit;
using Kunicardus.Core;
using Cirrious.MvvmCross.Binding.BindingContext;
using Kunicardus.Touch.Helpers.UI;
using CoreGraphics;
using Foundation;
using Facebook.LoginKit;
using Cirrious.CrossCore;
using Kunicardus.Core.Plugins.UIDialogPlugin;
using Facebook.CoreKit;
using Kunicardus.Touch.Plugins.UIDialogPlugin;

namespace Kunicardus.Touch
{
    public class LoginViewController : BaseMvxViewController
    {

        #region Properties

        public new LoginViewModel ViewModel
        {
            get { return (LoginViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        #endregion

        #region Constructors

        public LoginViewController()
        {
            HideMenuIcon = true;
        }

        #endregion

        #region Overrides

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _manager = new LoginManager();
            InitUI();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.NavigationBarHidden = true;
        }

        #endregion

        #region Methods

        private void InitUI()
        {

            View.BackgroundColor = UIColor.Clear.FromHexString(Styles.Colors.LoginScreenBackGroundGreen);

            UIImageView headerBackLine = new UIImageView(ImageHelper.MaxResizeImage(UIImage.FromBundle("back_line_green"), View.Frame.Width, 0));
            headerBackLine.SizeToFit();
            headerBackLine.Frame = new CoreGraphics.CGRect(0, 22, headerBackLine.Frame.Width, headerBackLine.Frame.Height);
            View.AddSubview(headerBackLine);

            UILabel greetingLabel = new UILabel();
            greetingLabel.TextColor = UIColor.White;
            greetingLabel.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, 26);
            greetingLabel.Text = ApplicationStrings.Greeting;
            greetingLabel.SizeToFit();
            greetingLabel.Frame =
                new CGRect((View.Frame.Width - greetingLabel.Frame.Width) / 2.0f, 80, greetingLabel.Frame.Width, greetingLabel.Frame.Height);
            View.AddSubview(greetingLabel);

            UIButton facebookLogIn = new UIButton(UIButtonType.RoundedRect);
            facebookLogIn.Frame = new CGRect((View.Frame.Width - 280.0f) / 2.0f, 140, 280, 50);
            facebookLogIn.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 2);
            facebookLogIn.SetImage(
                ImageHelper.MaxResizeImage(UIImage.FromBundle("fb"), 0, 36).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal),
                UIControlState.Normal);
            facebookLogIn.BackgroundColor = UIColor.Clear.FromHexString("#425d9e");
            facebookLogIn.SetTitle(ApplicationStrings.FacebookLogin, UIControlState.Normal);
            facebookLogIn.ImageEdgeInsets = new UIEdgeInsets(7.0f, 17.0f, 7.0f, 0.0f);
            facebookLogIn.TitleEdgeInsets = new UIEdgeInsets(0.0f, 25.0f, 0.0f, 0.0f);
            facebookLogIn.Layer.CornerRadius = 25;
            facebookLogIn.TintColor = UIColor.White;
            facebookLogIn.SetTitleColor(UIColor.White, UIControlState.Normal);
            facebookLogIn.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            facebookLogIn.DropShadowDependingOnBGColor();
            View.AddSubview(facebookLogIn);

            UIView orLeftLine = new UIView(new CGRect(facebookLogIn.Frame.Left, facebookLogIn.Frame.Bottom + 25, facebookLogIn.Frame.Width / 2.0f - 20.0f, 1.5f));
            orLeftLine.BackgroundColor = UIColor.Clear.FromHexString("#8eb931");
            View.AddSubview(orLeftLine);
            UILabel orLabel = new UILabel();
            orLabel.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, 16);
            orLabel.TextColor = UIColor.White;
            orLabel.Text = ApplicationStrings.Or;
            orLabel.SizeToFit();
            orLabel.Frame = new CGRect((View.Frame.Width - orLabel.Frame.Width) / 2.0f,
                facebookLogIn.Frame.Bottom + 25 - (orLabel.Frame.Height / 2.0f), orLabel.Frame.Width, orLabel.Frame.Height);
            View.AddSubview(orLabel);

            UIView orRightLine = new UIView(new CGRect(orLabel.Frame.Right + 20f, facebookLogIn.Frame.Bottom + 25,
                                     facebookLogIn.Frame.Width / 2.0f - 20.0f - (orLabel.Frame.Width / 2.0f), 1.5f));
            orRightLine.BackgroundColor = UIColor.Clear.FromHexString("#8eb931");
            View.AddSubview(orRightLine);

            UIButton justAuthorize = new UIButton(UIButtonType.RoundedRect);
            justAuthorize.Frame = new CGRect((View.Frame.Width - 280.0f) / 2.0f, orLeftLine.Frame.Bottom + 25, 280, 50);
            justAuthorize.BackgroundColor = UIColor.Clear.FromHexString("#f2bb2c");
            justAuthorize.SetTitle(ApplicationStrings.Authorize, UIControlState.Normal);
            justAuthorize.Layer.CornerRadius = 25;
            justAuthorize.TintColor = UIColor.White;
            justAuthorize.SetTitleColor(UIColor.White, UIControlState.Normal);
            justAuthorize.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 2);
            justAuthorize.DropShadowDependingOnBGColor();
            View.AddSubview(justAuthorize);

            UIView registrationView = new UIView(new CGRect(0, View.Frame.Height - 62, View.Frame.Width, 62));
            registrationView.BackgroundColor = UIColor.Clear.FromHexString("#97c840");

            UIButton registration = new UIButton(UIButtonType.System);
            registration.Font = UIFont.FromName(Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 2);
            var underlineregistration = new NSAttributedString(
                                            ApplicationStrings.Registration,
                                            underlineStyle: NSUnderlineStyle.Single, foregroundColor: UIColor.Clear.FromHexString(Styles.Colors.Yellow));
            registration.SetAttributedTitle(underlineregistration, UIControlState.Normal);
            registration.SizeToFit();
            registration.Frame =
                new CGRect((registrationView.Frame.Width - registration.Frame.Width) / 2.0f,
                (registrationView.Frame.Height - registration.Frame.Height) / 2.0f,
                registration.Frame.Width,
                registration.Frame.Height);
            registrationView.AddSubview(registration);
            View.AddSubview(registrationView);


            facebookLogIn.TouchUpInside += FBLogin_Clicked;


            // Bindings
            this.CreateBinding(registration).To((LoginViewModel vm) => vm.RegisterCommand).Apply();
            this.CreateBinding(justAuthorize).To((LoginViewModel vm) => vm.AuthCommand).Apply();
        }

        #endregion

        #region Facebook

        LoginManager _manager;

        private readonly string[] EXTENDED_PERMISSIONS = { "email" };
        void FBLogin_Clicked(object sender, EventArgs e)
        {
            _manager.LogInWithReadPermissions(EXTENDED_PERMISSIONS, (res, err) => FbLoginHandler(res, err));
        }

        void FbLoginHandler(LoginManagerLoginResult result, Foundation.NSError error)
        {
            var dialog = new TouchUIDialogPlugin();

            if (error != null && error.Code > 0)
            {
                dialog.ShowToast(error?.ToString());
            }
            else if (result.IsCancelled)
            {

            }
            else if (!result.GrantedPermissions.Contains("email"))
            {
                ShowErrorPermissionDialog();
            }
            else
            {
                var request = new GraphRequest("me", new NSDictionary(new NSString("fields"), new NSString("email,first_name,last_name")), AccessToken.CurrentAccessToken.TokenString, null, "GET");
                var requestConnection = new GraphRequestConnection();
                requestConnection.AddRequest(request, (c, lres, lerr) =>
                {
                    try
                    {
                        if (lres != null)
                        {
                            var res_parameters = (lres as NSDictionary);
                            if (res_parameters != null)
                            {
                                string fb_email = res_parameters["email"].ToString();
                                //string fb_id = res_parameters ["id"].ToString ();
                                string fb_firstname = res_parameters["first_name"].ToString();
                                string fb_lastname = res_parameters["last_name"].ToString();

                                ((LoginViewModel)ViewModel).FacebookConnect(fb_firstname, fb_lastname, fb_email, result.Token.TokenString);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        dialog.ShowToast(ex.Message?.ToString());
                    }
                });
                requestConnection.Start();
            }
        }

        private void ShowErrorPermissionDialog()
        {
            new UIAlertView(ApplicationStrings.Error,
                ApplicationStrings.WeNeedAccessToYourEmailToConnectWithFB,
                null,
                ApplicationStrings.GotIt).Show();
        }

        #endregion
    }
}

