using Kunicardus.Core.Helpers.AppSettings;
using Kunicardus.Core.Models;
using Kunicardus.Core.Models.BusinessModels;
using Kunicardus.Core.Models.DataTransferObjects.Request;
using Kunicardus.Core.Models.DataTransferObjects.Response;
using Kunicardus.Core.Services.Abstract;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kunicardus.Core.Models.DB;

namespace Kunicardus.Core.Services.Concrete
{
	public class NewsService : INewsService
	{

		private IUnicardApiProvider _apiProvider;
		private IAppSettings _appSettings;

		public NewsService (IUnicardApiProvider unicardApiProvider, IAppSettings appSettings)
		{
			_apiProvider = unicardApiProvider;
			_appSettings = appSettings;
		}

		public async Task<BaseActionResult<List<NewsInfo>>> GetNews (string userId)
		{

			BaseActionResult<List<NewsInfo>> result = new BaseActionResult<List<NewsInfo>> ();

			var request = new GetNewsListRequest {
				UserId = userId
			};

			var url = string.Format ("{0}GetNewsList", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request,
				           Formatting.None,
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = _apiProvider.Post<GetNewsListResponse> (url, null, json).Result;
			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
			result.Result = new List<NewsInfo> ();

			if (response.News != null && response.News.Count > 0) {
				foreach (var item in response.News) {
					var news = new NewsInfo {
						CreateDate = item.CreateDate,
						Description = item.Description,
						Id = item.Id,
						Image = item.Image,
						Title = item.Title,
						IsRead = false
					};
					result.Result.Add (news);
				}

			}
			return result;
		}


		public async Task<BaseActionResult<NewsModel>> GetNewsById (string userId, int newsId)
		{
			BaseActionResult<NewsModel> result = new BaseActionResult<NewsModel> ();

			var request = new GetNewsRequest { UserId = userId, NewsId = newsId };

			var url = string.Format ("{0}GetNewsById", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request,
				           Formatting.None,
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = _apiProvider.Post<GetNewsDetailsResponse> (url, null, json).Result;
			if (response != null) {
				result.Success = response.Successful;
				result.DisplayMessage = response.DisplayMessage;
				result.Result = new NewsModel ();

				result.Result.CreateDate = response.News.CreateDate;
				result.Result.Description = response.News.Description;
				result.Result.Id = response.News.Id;
				result.Result.Image = response.News.Image;
				result.Result.Title = response.News.Title;
			}
			return result;
		}
	}
}
