using System;
using Kunicardus.Billboards.Core.DbModels;
using System.Linq;
using Newtonsoft.Json.Schema;

namespace Kunicardus.Billboards.Core.ViewModels
{
	public class SettingsViewModel
	{
		BillboardsDb _db;

		public SettingsViewModel ()
		{
			_db = new BillboardsDb (BillboardsDb.path); 
			var query = "select * from userInfo";
			UserInfoFromDB = _db.Query<UserInfo> (query).FirstOrDefault ();
		}

		private UserInfo _userInfo;

		public UserInfo UserInfoFromDB {
			get{ return _userInfo; }
			set{ _userInfo = value; }
		}
	}
}

