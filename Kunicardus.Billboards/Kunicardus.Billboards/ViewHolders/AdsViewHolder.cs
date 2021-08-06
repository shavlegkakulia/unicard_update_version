using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Timers;

namespace Kunicardus.Billboards.ViewHolders
{

    public class AdsViewHolder : RecyclerView.ViewHolder
    {
        public View MainView { get; set; }

        public ImageView Image { get; set; }

        public ImageView ImgSuccess { get; set; }

        public ImageView ImgDownload { get; set; }

        public ImageView ImgArrow { get; set; }

        public ImageView ImgOrganizationLogo{ get; set; }

        public ProgressBar ProgressBar { get; set; }

        public Button BtnRetry { get; set; }

        public TextView BtnSkip { get; set; }

        public TextView Seconds { get; set; }

        public TextView TxtInfo { get; set; }

        public TextView TxtInfo2 { get; set; }

        public TextView Points { get; set; }

        public TextView TxtIgnore { get; set; }

        public TextView TxtPassTime { get; set; }

        public TextView TxtPassDate { get; set; }

        public TextView TxtOrganization{ get; set; }

        public RelativeLayout OverlayLayout { get; set; }

        public RelativeLayout DownloadLayout { get; set; }

        public RelativeLayout PointsLayout { get; set; }

        public RelativeLayout TimerLayout { get; set; }

        public string Tag { get; set; }

        public AdsViewHolder(View view)
            : base(view)
        {
            MainView = view;
        }
    }
}