using Kuni.Core.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuni.Core.Services.Abstract
{
	public interface IDataService
	{
		List<Transfer> PopulateShops ();
	}
}
