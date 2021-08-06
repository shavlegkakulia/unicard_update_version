using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;







using System.Threading.Tasks;
using Kunicardus.Billboards.Core.DbModels;

namespace Kunicardus.Billboards.Core.Services.Abstract
{
	public interface IBillboardsService
	{
		List<Billboard> GetBillboards ();

		int GetLastPassedBillboardId ();

		bool MarkBillboardAsSeen (int billboardId, int advertisementId);

		void InsertDummyDataForIOS ();
	}
}