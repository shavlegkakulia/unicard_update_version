
using Android.OS;
using Android.Views;
//using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Kunicardus.Droid.Fragments
{
    public class CardFragment : BaseMvxFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var View = base.GetAndInflateView(Resource.Layout.CardLayout);

            return View;
        }

        #region implemented abstract members of BaseMvxFragment

        public override void OnActivate()
        {
            //
        }

        #endregion
    }
}