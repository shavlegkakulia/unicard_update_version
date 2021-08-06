using System.Collections.Generic;
using System.Windows.Input;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Models.DB;
using System.Linq;
using System.Threading.Tasks;
using Kuni.Core.ViewModels;
using MvvmCross.ViewModels;
using MvvmCross;
using MvvmCross.Commands;
//using MvvmCross;

namespace Kuni.Core
{
	public class MenuViewModel : BaseViewModel
	{
		ILocalDbProvider _dbProvider;

		private string _welcomeMessage;

		public string WelcomeMessage {
			get{ return _welcomeMessage; }
			set {
				_welcomeMessage = value;
				RaisePropertyChanged (() => WelcomeMessage);
			}
		}

		private string _cardNumber;

		public string CardNumber {
			get{ return _cardNumber; }
			set {
				_cardNumber = value;
			}
		}

		public MenuViewModel (ILocalDbProvider dbProvider)
		{
			_dbProvider = dbProvider;

			Items.Add (new MenuModel {
				IconName = "home",
				Name = "მთავარი"
			});
			Items.Add (new MenuModel {
				IconName = "mypage",
				Name = "ჩემი გვერდი"
			});
			Items.Add (new MenuModel {
				IconName = "catalog",
				Name = "რაში დავხარჯოტესტ"
			});
			Items.Add (new MenuModel {
				IconName = "merchants",
				Name = "ჩემ გარშემო"
			});
			Items.Add (new MenuModel {
				IconName = "partners",
				Name = ApplicationStrings.Partners
			});
			Items.Add (new MenuModel {
				IconName = "news",
				Name = ApplicationStrings.News
			});
			Items.Add (new MenuModel {
				IconName = "unnamed",
				Name = ApplicationStrings.AboutUs
			});
			Items.Add (new MenuModel {
				IconName = "settings",
				Name = ApplicationStrings.Settings
			});
			Items.Add (new MenuModel {
				IconName = "logout",
				Name = "გამოსვლა"
			});

			Task.Run (() => {
				UpdateUserWelcomeMessage ();
			});
		}

		private void UpdateUserWelcomeMessage ()
		{
			var user = _dbProvider.Get<UserInfo> ().FirstOrDefault ();
			if (user != null
			    && !string.IsNullOrWhiteSpace (user.FirstName)
			    && !string.IsNullOrWhiteSpace (user.LastName)
			    && user.FirstName != "-"
			    && user.LastName != "-") {
//				InvokeOnMainThread (() => {
				WelcomeMessage = string.Format ("{0} {1}", user.FirstName, user.LastName);
//				});
				CardNumber = user.VirtualCardNumber;
			} else {
				InvokeOnMainThread (() => {
					WelcomeMessage = ApplicationStrings.Hello;
				});
			}
		}

		private List<MenuModel> _items = new List<MenuModel> ();

		public List<MenuModel> Items {
			get { return _items; }
			set {
				_items = value;
				RaisePropertyChanged (() => Items);
			}
		}

		private MvxCommand<MenuModel> _itemSelectedCommand;

		public ICommand ItemSelectedCommand {
			get {
				_itemSelectedCommand = _itemSelectedCommand ?? new MvvmCross.Commands.MvxCommand<MenuModel> (DoSelectItem);
				return _itemSelectedCommand;
			}
		}

		private void DoSelectItem (MenuModel item)
		{
			switch (item.MenuIndex) {
			case 0:
				NavigationCommand<HomePageViewModel> ();
				break;
			case 1:
                NavigationCommand<OrganisationListViewModel> ();
				break;
			case 2:
                NavigationCommand<NewsListViewModel> ();
				break;
			}
		}


		public MvxViewModel GetSelectedViewModel (int index)
		{
			switch (index) {
			case 0:
				return Mvx.IoCConstruct<HomePageViewModel> ();
			case 1:
				return Mvx.IoCConstruct<OrganisationListViewModel> ();
			case 2:
				return Mvx.IoCConstruct<NewsListViewModel> ();
			}
			return null;
		}
	}

	public class MenuModel
	{
		public int MenuIndex { get; set; }

		public string Name { get; set; }

		public string IconName { get; set; }

	}
}

