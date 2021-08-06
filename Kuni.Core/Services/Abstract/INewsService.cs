using Kuni.Core.Models;
using Kuni.Core.Models.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kuni.Core.Models.DB;

namespace Kuni.Core.Services.Abstract
{
	public interface INewsService
	{
		Task<BaseActionResult<List<NewsInfo>>> GetNews (string userId);

		Task<BaseActionResult<NewsModel>> GetNewsById (string userId, int newsId);
	}
}
