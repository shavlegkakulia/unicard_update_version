using MvvmCross.ViewModels;
using Kuni.Core.Models;
using Kuni.Core.Models.BusinessModels;
using Kuni.Core.Models.DB;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kuni.Core.ViewModels
{
	public class NewsListViewModel : BaseViewModel
	{
		INewsService _newsService;
		ILocalDbProvider _dbProvider;

		public NewsListViewModel (INewsService newsService, ILocalDbProvider dbProvider)
		{
			_dbProvider = dbProvider;
			_newsService = newsService;
		}

		private bool _dataPopulated;

		public bool DataPopulated {
			get { return _dataPopulated; }
			set {
				_dataPopulated = value;
				RaisePropertyChanged (() => DataPopulated);
			}
		}

		private List<NewsInfo> _news;

		public List<NewsInfo> News {
			get { return _news; }
			set { 
				_news = value;
				RaisePropertyChanged (() => News);
			}
		}

		private  ICommand _itemClick;

		public ICommand ItemClickCommand {
			get {
				_itemClick = _itemClick ?? new MvvmCross.Commands.MvxCommand<NewsInfo> (ItemClick);
				return _itemClick;
			}
		}

		private void ItemClick (NewsInfo newsInfo)
		{
			if (_device.Platform == "ios") {
				MarkAsRead (newsInfo.Id);
				NavigationCommand<NewsDetailsViewModel> (new {id = newsInfo.Id});
			} else { 
				// A N D R O I D
				NavigationCommand<NewsDetailsViewModel> (new {id = newsInfo.Id});
				Task.Run (() => {
					MarkAsRead (newsInfo.Id);
				});
			}
		}

		public void MarkAsRead (int newsId)
		{
			var user = _dbProvider.Get<UserInfo> ().First ();
			_dbProvider.Execute (string.Format ("update NewsInfo set IsRead = 1 where IsRead = 0 and UserId = {0} and Id = {1}", user.UserId, newsId));
			if (_device.Platform != "ios") {
				GetNewsList ();
			}
		
		}

		public int GetNewsId (int position)
		{
			return News [position].Id;
		}

		public void GetNewsList ()
		{
			InvokeOnMainThread (() => {
				_dialog.ShowProgressDialog (ApplicationStrings.Loading);
			});
			Task.Run (() => {
				var user = _dbProvider.Get<UserInfo> ().First ();
				var newsList = _dbProvider.Get<NewsInfo> ().Where (x => x.UserId == user.UserId).OrderByDescending (x => x.CreateDate).ToList ();
				if (newsList != null && newsList.Count > 0) {
					News = newsList.ToList ();
					DataPopulated = true;
				} 
				InvokeOnMainThread (() => _dialog.DismissProgressDialog ());
			});
		}

		public void RefreshNewsList ()
		{
			Task.Run (() => {
				RefreshASync ();
			});
		}

		private async void RefreshASync ()
		{
			var user = _dbProvider.Get<UserInfo> ().First ();
			BaseActionResult<List<NewsInfo>> response = new BaseActionResult<List<NewsInfo>> ();
			response = await _newsService.GetNews (user.UserId);
			if (response.Success && response.Result.Count > 0) {
				var tmpNews = response.Result.OrderByDescending (x => x.CreateDate).ToList ();
				var oldReadNewsId = _dbProvider.Query<NewsInfo> ("Select * from NewsInfo where IsRead=1 and UserId=" + user.UserId);
				for (int i = 0; i < tmpNews.Count; i++) {
					tmpNews [i].UserId = user.UserId;
					for (int j = 0; j < oldReadNewsId.Count; j++) {
						if (tmpNews [i].Id == oldReadNewsId [j].Id) {
							tmpNews [i].IsRead = true;
							break;
						}
					}
				}
				News = tmpNews.OrderByDescending (x => x.CreateDate).ToList ();
				_dbProvider.Execute ("Delete from NewsInfo");
				_dbProvider.Insert<NewsInfo> (News);
			}
			DataPopulated = true;
		}
	}
}
