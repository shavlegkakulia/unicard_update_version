using System;
using Kunicardus.Billboards.Core.DbModels;
using System.Linq;

namespace Kunicardus.Billboards.Core
{
	public class MenuViewModel
	{
		public MenuViewModel ()
		{			
		}

		public string GetUserName ()
		{
			using (BillboardsDb db = new BillboardsDb (BillboardsDb.path)) {
				var users = db.Query<UserInfo> ("select * from UserInfo");
				if (users != null && users.Count > 0) {
					UserInfo user = users.FirstOrDefault ();
					return string.Format ("{0} {1}", user.FirstName, user.LastName);
				} else {
					return string.Empty;
				}
			}
		}

		public bool Logout ()
		{
			try {
				using (BillboardsDb db = new BillboardsDb (BillboardsDb.path)) {
					db.Execute ("delete from UserInfo");
					return true;
				}
			} catch (Exception ex) {

				return false;
			}
		}
	}
}

