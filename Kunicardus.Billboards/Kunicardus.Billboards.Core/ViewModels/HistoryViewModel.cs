using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kunicardus.Billboards.Core.Services.Abstract;
using Kunicardus.Billboards.Core.DbModels;
using System.Threading.Tasks;
using Kunicardus.Billboards.Core.Models;

namespace Kunicardus.Billboards.Core.ViewModels
{
	public class HistoryViewModel
	{
		private IAdsService _adsService;
		private IUserService _userService;
		private string _userId;

		public List<HistoryModel> Advertisments { get; set; }


		public HistoryViewModel (IAdsService adsService, IUserService userService)
		{
			_adsService = adsService;
			_userService = userService;
			BillboardsDb db = new BillboardsDb (BillboardsDb.path);
			_userId = db.Query<UserInfo> (@"select b.UserId from UserInfo as b").Select (x => x.UserId).FirstOrDefault ().ToString ();
		}

		public bool GetAdvertisments ()
		{
			Advertisments = _adsService.GetSeenAdvertisments (_userId);
			if (Advertisments != null) {
				return true;
			}
			return false;
		}
	}
}