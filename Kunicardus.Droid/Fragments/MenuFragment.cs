using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Kunicardus.Droid;
using Kuni.Core;
using MvvmCross;
using MvvmCross.Droid.Support.V4;

namespace Kunicardus.Droid
{
    public class MenuFragment : MvxFragment
    {
        ListView _menuList;
        MenuAdapter _adapter;

        public MenuFragment()
        {
            ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Mvx.IoCProvider.IoCConstruct<MenuViewModel>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var View = this.BindingInflate(Resource.Layout.MenuView, null);
            var activity = this.Activity as MainView;

            var logo = View.FindViewById<ImageView>(Resource.Id.logoImg);
            logo.Click += delegate
            {
                activity.MenuClick(0);
            };

            var card = View.FindViewById<RelativeLayout>(Resource.Id.tabCard);
            card.Click += (o, e) =>
            {
                if (!activity.IsAnimInProgress)
                {
                    //activity.IsAnimInProgress = true;
                    //GAService.GetGASInstance().Track_App_Event("Card Clicked", "from menu");
                    activity.CardAnimationClicked(null, null);
                }
            };

            _menuList = View.FindViewById<ListView>(Resource.Id.menuList);
            if (_menuList != null)
            {
                //comes from mvvmcross
                //MenuViewModel vm = this.ViewModel as MenuViewModel;
                _adapter = new MenuAdapter(this.Activity, ((MenuViewModel)ViewModel).Items);
                _menuList.Adapter = _adapter;
                _menuList.ItemClick += (o, e) =>
                {
                    activity.MenuClick(e.Position);
                };
            }
            return View;
        }
    }
}

