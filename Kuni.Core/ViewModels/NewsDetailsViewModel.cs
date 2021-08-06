using System;
using System.Linq;
using System.Threading.Tasks;

using Kuni.Core.Models;
using Kuni.Core.Models.DB;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Services.Abstract;
using MvvmCross;
//using MvvmCross;

namespace Kuni.Core.ViewModels
{
	public class NewsDetailsViewModel : BaseViewModel
	{
		INewsService _newsService;
		ILocalDbProvider _dbProvider;
		private int _id;

		public NewsDetailsViewModel (INewsService newsService, ILocalDbProvider dbProvider)
		{
			_newsService = newsService;
			_dbProvider = dbProvider;
			GetUserSettings ();
		}

		public void Init (int id)
		{
			_id = id;
			InvokeOnMainThread (() => _dialog.ShowProgressDialog (ApplicationStrings.Loading));
			GetNewsById (_id);
		}

		public SettingsInfo GetUserSettings ()
		{
			try {
				var userId = _dbProvider.Get<UserInfo> ().FirstOrDefault ().UserId;
				UserSettings = _dbProvider.Get<SettingsInfo> ().Where (x => x.UserId == Convert.ToInt32 (userId)).FirstOrDefault ();

				return UserSettings;
			} catch (Exception ex) {
				return null;
			}
		}

		public SettingsInfo UserSettings {
			get;
			set;
		}

		private string _title;

		public string Title {
			get { return _title; }
			set {
				_title = value;
				RaisePropertyChanged (() => Title);
			}
		}

		private string _image;

		public string Image {
			get { return _image; }
			set {
				_image = value;
				RaisePropertyChanged (() => Image);
			}
		}

		private DateTime _date;

		public DateTime Date {
			get { return _date; }
			set {
				_date = value;
				RaisePropertyChanged (() => Date);
			}
		}

		private string _desctiption;

		public string Description {
			get {
				return _desctiption;
			}
			set {
				_desctiption = "<link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\" />" + value;
				RaisePropertyChanged (() => Description);
			}
		}

		public void GetNewsById (int newsId)
		{
			Task.Run (() => {		
				using (ILocalDbProvider dbProvider = Mvx.IoCProvider.Resolve<ILocalDbProvider> ()) {
					var user = dbProvider.Get<UserInfo> ().First ();
					var newsInfo = _newsService.GetNewsById (user.UserId, newsId).Result;

					if (newsInfo.Result != null && newsInfo.Success) {
						Date = newsInfo.Result.CreateDate;
						Description = newsInfo.Result.Description;
						Image = newsInfo.Result.Image;
						Title = newsInfo.Result.Title;
					} else {
						var news = dbProvider.Get<NewsInfo> ().FirstOrDefault (x => x.Id == newsId && x.UserId == user.UserId);
						if (news != null) {
							Date = news.CreateDate;
							Description = news.Description;
							Image = news.Image;
							Title = news.Title;
						}
					}
					InvokeOnMainThread (() => _dialog.DismissProgressDialog ());

					if (!string.IsNullOrEmpty (newsInfo.DisplayMessage)) {
						_dialog.ShowToast (newsInfo.DisplayMessage);
					}
				}
			});
		}
	}
}
