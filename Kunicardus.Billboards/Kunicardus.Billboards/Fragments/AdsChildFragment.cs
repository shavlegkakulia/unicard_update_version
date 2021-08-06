//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Kunicardus.Billboards.Core.ViewModels;
//using Kunicardus.Billboards.Core.Services;
//using System.Threading.Tasks;
//using Android.Graphics;
//using Kunicardus.Billboards.Plugins;
//using System.Timers;

//namespace Kunicardus.Billboards.Fragments
//{
//    public class AdsChildFragment : BaseFragment
//    {

//        AdsViewModel _adsViewModel;

//        public AdsChildFragment(/*int adId*/)
//        {
//            //_adId = adId;
//        }

//        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//        {
//            base.OnCreateView(inflater, container, savedInstanceState);
//            var view = inflater.Inflate(Resource.Layout.AdDetailsLayout, null);

//            _adsViewModel = new AdsViewModel(new BillboardsService(), new AdsService());
            
//            return view;
//        }

//        public void SetId(int adId)
//        {
//            //_adId = adId;
//        }

//        public override void OnActivate()
//        {
//            Task.Run(async () =>
//            {
//                var advertismentLoaded = await _adsViewModel.GetAdvertisment(_adId);
//                if (advertismentLoaded)
//                {
//                    Activity.RunOnUiThread(() =>
//                    {
//                        InitFragmentData(_adId);
//                    });
//                }
//            });
//        }


//        private void InitFragmentData(int index)
//        {
//                _header.Text = string.Format("ქულის ჩარიცხვას დაელოდეთ {0} წამი", _adsViewModel.Advertisment.TimeOut);
//                _points.Text = _adsViewModel.Advertisment.Points.ToString();

//                Task.Run(async () =>
//                {
//                    Bitmap image = await ImageDownloader.GetImageBitmapFromUrl(_adsViewModel.Advertisment.Image);
//                    if (image != null)
//                    {
//                        Activity.RunOnUiThread(() =>
//                        {
//                            _imgAd.SetImageBitmap(image);
//                            InitAd();
//                        });
//                    }
//                });
//        }

//        private void InitAd()
//        {
//            var height = _imgAd.Height;
//            var width = Convert.ToInt32(height / 1.457);
//            _imgAd.LayoutParameters.Height = height;
//            _imgAd.LayoutParameters.Width = width;
//            _imgAd.RequestLayout();

//            _timer = new Timer();
//            _timer.Interval = 100;
//            _timer.Elapsed += timer_Elapsed;
//            _timer.Enabled = true;
//            _timer.Start();
//        }

//        private void timer_Elapsed(object sender, ElapsedEventArgs e)
//        {
//            _progressBar.Progress++;
//            _seconds.Text = (_progressBar.Progress / 10).ToString();

//            this.Activity.RunOnUiThread(() =>
//            {
//                if (_progressBar.Progress == 150)
//                {
//                    _seconds.SetTextColor(Color.ParseColor("#8dc641"));
//                    _progressBar.ProgressDrawable.SetColorFilter(Color.ParseColor("#8dc641"), PorterDuff.Mode.SrcIn);
//                    _waitText.Text = string.Format("თქვენ დაგერიცხათ {0} ქულა", _adsViewModel.Advertisment.Points);
//                    _timer.Enabled = false;
//                }
//            });
//        }

//        public override bool UserVisibleHint
//        {
//            get
//            {
//                return base.UserVisibleHint;
//            }
//            set
//            {
//                base.UserVisibleHint = value;
//                if (!value)
//                {
//                    if (_progressBar != null && _seconds != null && Convert.ToInt32(_seconds.Text) != _adsViewModel.Advertisment.TimeOut)
//                    {
//                        _progressBar.Progress = 0;
//                        _timer.Elapsed -= timer_Elapsed;
//                    }
//                }
//            }
//        }
//    }
//}