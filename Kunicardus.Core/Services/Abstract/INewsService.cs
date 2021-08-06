using Kunicardus.Core.Models;
using Kunicardus.Core.Models.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kunicardus.Core.Models.DB;

namespace Kunicardus.Core.Services.Abstract
{
	public interface INewsService
	{
		Task<BaseActionResult<List<NewsInfo>>> GetNews (string userId);

		Task<BaseActionResult<NewsModel>> GetNewsById (string userId, int newsId);
	}
}
