using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kunicardus.Billboards.Core.Services.Abstract;
using Kunicardus.Billboards.Core.Models;
using System.Threading.Tasks;
using Kunicardus.Billboards.Core.DbModels;

namespace Kunicardus.Billboards.Core.ViewModels
{
	public class HomePageViewModel
	{
		BillboardsDb _db;
		IBillboardsService _billboardsService;
		IAdsService _adsService;
		IUserService _userService;

		public string  DisplayMessage { get; set; }

		public List<AdsModel> Advertisments { get; set; }

		public UserInfo User { get; set; }

		public event EventHandler SessionTimedOut;

		public HomePageViewModel (IBillboardsService billboardsService, IAdsService adsService, IUserService userService)
		{
			_db = new BillboardsDb (BillboardsDb.path);
			_billboardsService = billboardsService;
			_adsService = adsService;
			_userService = userService;
		}

		public bool GetLoadedAds ()
		{
			Advertisments = _adsService.GetLoadedAdvertisments ();
			if (Advertisments != null) {
				return true;
			}
			return false;
		}

		public void GetLocalUserInfo ()
		{
			string query = "select * from UserInfo";
			User = _db.Query<UserInfo> (query).FirstOrDefault ();
		}

		public bool GetUserInfo ()
		{
			var response = _userService.GetUserInfo ();

			DisplayMessage = response.DisplayMessage;
            
			if (response.ResultCode == "Unauthorized") {
				if (SessionTimedOut != null) {
					SessionTimedOut (this, null);
				}
				return false;
			}

			if (response.Result != null && response.Success) {
				User = response.Result;
			}

			return User != null;
		}
	}
}