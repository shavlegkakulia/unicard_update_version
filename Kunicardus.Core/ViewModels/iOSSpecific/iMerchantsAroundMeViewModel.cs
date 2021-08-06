using Kunicardus.Core.Models;
using Kunicardus.Core.Models.BusinessModels;
using Kunicardus.Core.Models.DB;
using Kunicardus.Core.Plugins;
using Kunicardus.Core.Plugins.UIDialogPlugin;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross;

namespace Kunicardus.Core.ViewModels.iOSSpecific
{
    public class iMerchantsAroundMeViewModel : BaseViewModel
    {
        ILocalDbProvider _dbProvider;
        IUIDialogPlugin _dialogPlugin;
        IOrganizationService _organizationService;
        IGoogleAnalyticsService _gaService;

        public iMerchantsAroundMeViewModel(ILocalDbProvider dbProvider,
                                            IUIDialogPlugin dialogPlugin,
                                            IOrganizationService organizationService,
                                            IGoogleAnalyticsService gaService)
        {
            _dbProvider = dbProvider;
            _dialogPlugin = dialogPlugin;
            _organizationService = organizationService;
            _gaService = gaService;
            //			UpdateMerchants ();
        }

        public void Init(int orgId)
        {
            _orgId = orgId;
        }

        public int? _orgId { get; set; }


        #region My Location

        double? _longitude;

        public double? Longitude
        {
            get { return _longitude; }
            set
            {
                _longitude = value;
                RaisePropertyChanged(() => Longitude);
            }
        }

        double? _latitude;

        public double? Latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = value;
                RaisePropertyChanged(() => Latitude);
            }
        }

        #endregion

        #region Check Local Location Service

        public void UpdateDistances()
        {
            if (_merchants != null)
            {
                Merchants = GetMerchantsWithDistance(_merchants);
            }
        }

        public bool LocationEnabled()
        {
            //			var settings = _dbProvider.Get<SettingsInfo> ().FirstOrDefault ();
            //			if (settings != null && settings.LocationEnabled.HasValue) {
            //				return settings.LocationEnabled.Value;
            //			}
            return true;
        }

        #endregion

        private bool _dataPopulated;

        public bool DataPopulated
        {
            get { return _dataPopulated; }
            set
            {
                _dataPopulated = value;
                RaisePropertyChanged(() => DataPopulated);
            }
        }

        public bool ShouldRedrawMap { get; set; }

        List<MerchantInfo> _merchants;

        public List<MerchantInfo> Merchants
        {
            get { return _merchants; }
            set
            {
                _merchants = value;
                RaisePropertyChanged(() => Merchants);
                DataPopulated = true;
            }
        }

        #region Filtering

        public void FilterMerchants(string filter)
        {
            Task.Run(() =>
            {
                var addresses = _dbProvider.Get<MerchantInfo>()
                    .Where(x => x.Address.Contains(filter, StringComparison.OrdinalIgnoreCase)) //  .IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                var names = _dbProvider.Get<MerchantInfo>()
                    .Where(x => x.MerchantName.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                //var merchants = _dbProvider.Get<MerchantInfo>().Where(x => x.Address.Contains(filter) || x.MerchantName.Contains(filter));
                var merchants = new List<MerchantInfo>();
                merchants.AddRange(addresses);
                merchants.AddRange(names);
                ShouldRedrawMap = true;
                Merchants = GetMerchantsWithDistance(merchants.ToList());
            });
        }

        public List<string> GetFilterSuggestions(string filter)
        {
            List<string> suggestions = new List<string>();
            if (Merchants != null)
            {
                var adresses = Merchants.Where(x => x.Address.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.Address.Split(' '));
                var names = Merchants.Where(x => x.MerchantName.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.MerchantName)
                    .Distinct();



                foreach (var item in adresses)
                {
                    suggestions.AddRange(item);
                }
                suggestions.AddRange(names);
            }
            return suggestions.Distinct().ToList();
        }

        #endregion

        public void OpenOrgDetils(int orgId)
        {
            _gaService.TrackEvent(GAServiceHelper.From.FromPartnersList, GAServiceHelper.Events.PartnersDetailClicked);
            _gaService.TrackScreen(GAServiceHelper.Page.PartnersDetails);
            ShowViewModel<OrganizationDetailsViewModel>(new { organisationId = orgId });
        }

        public void GetMerchants(int? orgId = null)
        {
            Task.Run(async () =>
            {
                orgId = _orgId;
                List<MerchantInfo> merchants = null;
                if (orgId > 0)
                {
                    string query = string.Format("select * from MerchantInfo as m where m.OrganizationId = {0}", orgId);
                    merchants = _dbProvider.Query<MerchantInfo>(query);
                }
                else
                {
                    merchants = _dbProvider.Query<MerchantInfo>("select * from MerchantInfo");
                }
                if (merchants != null && merchants.Count > 0)
                {
                    ShouldRedrawMap = true;
                    Merchants = GetMerchantsWithDistance(merchants);
                }
                var response = new BaseActionResult<List<MerchantModel>>();
                if (orgId > 0)
                {
                    response = await _organizationService.GetMerchants(null, null, orgId);
                }
                else {
                    response = await _organizationService.GetMerchants(null, null, null);
                }
                merchants = new List<MerchantInfo>();
                if (response.Success && response.Result != null)
                {
                    foreach (var item in response.Result)
                    {
                        var organization = new MerchantInfo
                        {
                            MerchantId = item.MerchantId,
                            OrganizationId = item.OrganizationId,
                            Image = (item.Image ?? "").Replace(@"\", "/"),
                            MerchantName = item.MerchantName,
                            Address = item.Address,
                            CityId = item.CityId,
                            DistrictId = item.DistrictId,
                            Latitude = item.Latitude,
                            Longitude = item.Longitude,
                            OrgnizationPointsDesc = item.OrgnizationPointsDesc,
                            Unit = item.Unit,
                            UnitScore = item.UnitScore,
                            UnitDescription = item.UnitDescription
                        };
                        merchants.Add(organization);
                    }
                }
                if (orgId == null)
                {
                    _dbProvider.Insert<MerchantInfo>(merchants);
                }
                InvokeOnMainThread(() =>
                {

                    if (merchants != null && merchants.Count > 0)
                    {
                        ShouldRedrawMap = true;
                        Merchants = GetMerchantsWithDistance(merchants);
                    }

                    if (!string.IsNullOrEmpty(response.DisplayMessage))
                    {
                        _dialogPlugin.ShowToast(response.DisplayMessage);
                    }
                });
                //}

            });
        }

        #region Distance Calculation

        public List<MerchantInfo> GetMerchantsWithDistance(List<MerchantInfo> merchants)
        {
            if (merchants == null)
                return merchants;
            foreach (var item in merchants)
            {
                if (item.Latitude.HasValue && item.Longitude.HasValue && Latitude.HasValue && Longitude.HasValue)
                {
                    item.Distance = GetDistanceInMetres(Latitude.Value, Longitude.Value, item.Latitude.Value, item.Longitude.Value);
                    if (item.Distance != null)
                    {

                        if (item.Distance < 1000)
                        {
                            item.DistanceText = string.Format("{0} {1}", item.Distance, "მ");
                            item.DistanceUnit = "მ";
                            continue;
                        }
                        else
                            item.DistanceUnit = "კმ";

                        if (item.Distance >= 1000 && item.Distance < 5000)
                        {
                            item.DistanceText = string.Format(">1 {0}", item.DistanceUnit);
                        }
                        else if (item.Distance >= 5000 && item.Distance < 10000)
                        {
                            item.DistanceText = string.Format(">5 {0}", item.DistanceUnit);
                        }
                        else if (item.Distance >= 10000 && item.Distance < 25000)
                        {
                            item.DistanceText = string.Format(">10 {0}", item.DistanceUnit);
                        }
                        else if (item.Distance >= 25000 && item.Distance < 50000)
                        {
                            item.DistanceText = string.Format(">25 {0}", item.DistanceUnit);
                        }
                        else if (item.Distance >= 50000 && item.Distance < 100000)
                        {
                            item.DistanceText = string.Format(">50 {0}", item.DistanceUnit);
                        }
                        else {
                            item.DistanceText = string.Format(">{0} {1}", item.Distance / 100000 * 100, item.DistanceUnit);
                        }
                    }
                }
            }
            return merchants.OrderByDescending(x => x.Distance.HasValue).ThenBy(x => x.Distance).Distinct().ToList();
        }

        public static int? GetDistanceInMetres(double lat1, double lon1, double lat2, double lon2)
        {

            if (lat1 == lat2 && lon1 == lon2)
                return 0;

            var theta = lon1 - lon2;

            var distance = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) +
                           Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
                           Math.Cos(deg2rad(theta));

            distance = Math.Acos(distance);
            if (double.IsNaN(distance))
                return null;

            distance = rad2deg(distance);
            distance = distance * 60.0 * 1.1515 * 1609.344;

            return (Convert.ToInt32(distance));
        }

        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        #endregion
    }
}

