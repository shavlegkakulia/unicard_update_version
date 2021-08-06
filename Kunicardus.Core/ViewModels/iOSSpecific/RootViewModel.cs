using System;
using MvvmCross.Core.ViewModels;
using Kunicardus.Core.Services.Abstract;
using System.Threading.Tasks;
using Kunicardus.Core.Models;
using System.Collections.Generic;
using Kunicardus.Core.Models.DB;
using Kunicardus.Core.Providers.LocalDBProvider;
using System.Linq;
using Kunicardus.Core.Models.BusinessModels;
using MvvmCross;
using MvvmCross.Platform;

namespace Kunicardus.Core.ViewModels.iOSSpecific
{
	public class RootViewModel : MvxViewModel
	{
		public RootViewModel ()
		{
			UpdateMerchants ();
		}

		public bool UserAuthed { get; set; }

		public void Init (bool auth)
		{
			UserAuthed = auth;
		}

		private  void  UpdateMerchants ()
		{
			Task.Run (async () => {
				BaseActionResult<List<MerchantModel>> response = new BaseActionResult<List<MerchantModel>> ();
				response = await Mvx.Resolve<IOrganizationService> ().GetMerchants (null, null, null);
				using (ILocalDbProvider dbProvider = Mvx.Resolve<ILocalDbProvider> ()) {
					var merchants = new List<MerchantInfo> ();
					if (response.Success && response.Result != null) {
						foreach (var item in response.Result) {
							var organization = new MerchantInfo {
								MerchantId = item.MerchantId,
								OrganizationId = item.OrganizationId,
								Image = item.Image,
								MerchantName = item.MerchantName,
								Address = item.Address,
								CityId = item.CityId,
								DistrictId = item.DistrictId,
								Latitude = item.Latitude,
								Longitude = item.Longitude,
								OrgnizationPointsDesc = item.OrgnizationPointsDesc,
								Unit = item.Unit,
								UnitScore = item.UnitScore
							};
							merchants.Add (organization);
						}
						dbProvider.Execute ("Delete from MerchantInfo");
						dbProvider.Insert<MerchantInfo> (merchants);
					}
				}
			});
		}
	}
}

