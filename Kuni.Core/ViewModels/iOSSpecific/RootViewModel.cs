using System;
using MvvmCross.ViewModels;
using Kuni.Core.Services.Abstract;
using System.Threading.Tasks;
using Kuni.Core.Models;
using System.Collections.Generic;
using Kuni.Core.Models.DB;
using Kuni.Core.Providers.LocalDBProvider;
using System.Linq;
using Kuni.Core.Models.BusinessModels;
using MvvmCross;
//using MvvmCross;

namespace Kuni.Core.ViewModels.iOSSpecific
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
				response = await Mvx.IoCProvider.Resolve<IOrganizationService> ().GetMerchants (null, null, null);
				using (ILocalDbProvider dbProvider = Mvx.IoCProvider.Resolve<ILocalDbProvider> ()) {
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

