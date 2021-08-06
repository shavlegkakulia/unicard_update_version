using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kunicardus.Billboards.Core.Models;
using Kunicardus.Billboards.Core.DbModels;
using Kunicardus.Billboards.Core.UnicardApiProvider;

namespace Kunicardus.Billboards.Core.ViewModels
{
	public class MainViewModel
	{
		public List<MenuModel> MenuItems { get; set; }

        public MainViewModel()
        {
            LoadMenuItems();
        }

		public void LoadMenuItems ()
		{
			MenuItems = new List<MenuModel> ();
			MenuItems.Add (new MenuModel {
				IconName = "home",
				Name = "მთავარი გვერდი"
			});
			MenuItems.Add (new MenuModel {
				IconName = "map",
				Name = "რუკა"
			});
			MenuItems.Add (new MenuModel {
				IconName = "ads",
				Name = "რეკლამები"
			});
			MenuItems.Add (new MenuModel {
				IconName = "history",
				Name = "ისტორია"
			});
			MenuItems.Add (new MenuModel {
				IconName = "settings",
				Name = "პარამეტრები"
			});
			MenuItems.Add (new MenuModel {
				IconName = "logout",
				Name = "გამოსვლა"
			});
		}
        
		public bool Logout ()
		{
			try {
				using (BillboardsDb db = new BillboardsDb (BillboardsDb.path)) {
					var users = db.Query<UserInfo> ("select * from UserInfo");
					//if (users != null && users.Count > 0)
					//{
					//    var user = users.FirstOrDefault();
					//    db.Execute("delete from AutoCompleteFields");
					//    db.Insert(new AutoCompleteFields()
					//    {
					//        Id = 0,
					//        UserEmail = user.Username
					//    });
					//}
					db.Execute ("delete from UserInfo");
					return true;
				}
			} catch (Exception ex) {
				
				return false;
			}
		}
	}
}