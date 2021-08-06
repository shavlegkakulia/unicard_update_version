using Android.OS;
using Android.Views;
using MvvmCross.Droid.Support.V4;
//using MvvmCross.Droid.Views.Fragments;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Kunicardus.Droid
{
    public abstract class BaseMvxFragment : MvxFragment
    {
        public virtual void OnActivate()
        {
        }

        protected View GetAndInflateView(int layoutRes)
        {
            return this.BindingInflate(layoutRes, null);
        }
    }
}

