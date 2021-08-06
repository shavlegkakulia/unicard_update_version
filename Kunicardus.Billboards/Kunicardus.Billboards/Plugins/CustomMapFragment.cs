
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;

namespace Kunicardus.Billboards
{
    [Register("customMapFragment")]
    public class CustomMapFragment : SupportMapFragment
    {
        public View mOriginalContentView;
        public TouchableWrapper mTouchView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        { 
            mOriginalContentView = base.OnCreateView(inflater, container, savedInstanceState);    

            if (mTouchView != null)
            {                
                mTouchView.AddView(mOriginalContentView);
            }
            return mTouchView;
        }


        public override View View
        {
            get
            {
                return mOriginalContentView;
            }
        }
    }

    public class TouchableWrapper: FrameLayout
    {

        public event EventHandler<MotionEvent> Touched;

        public TouchableWrapper(Context context)
            : base(context)
        {
        }

        public TouchableWrapper(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public TouchableWrapper(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            if (this.Touched != null)
            {
                this.Touched(this, e);
            }

            return base.DispatchTouchEvent(e);
        }
    }

    public interface IOnMapDragEventListener
    {
        void OnMapDragged();
    }
}

