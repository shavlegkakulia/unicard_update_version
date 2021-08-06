using Kuni.Core.Models;
using Kuni.Core.Models.BusinessModels;
using Kuni.Core.Models.DB;
using Kuni.Core.Plugins;
using Kuni.Core.Plugins.UIDialogPlugin;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Services.Abstract;
using MvvmCross;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using MvvmCross;

namespace Kuni.Core.ViewModels
{
    public class MerchantsViewModel : BaseViewModel
    {
        ILocalDbProvider _dbProvider;
        IUIDialogPlugin _dialogPlugin;
        IOrganizationService _organizationService;

        public MerchantsViewModel(IUIDialogPlugin dialogPlugin, IOrganizationService organizationService, ILocalDbProvider dbProvider)
        {
            _dialogPlugin = dialogPlugin;
            _dbProvider = dbProvider;
            _organizationService = organizationService;
        }

        public SettingsInfo GetUserSettings()
        {
            try
            {
                var userId = _dbProvider.Get<UserInfo>().FirstOrDefault().UserId;
                UserSettings = _dbProvider.Get<SettingsInfo>().Where(x => x.UserId == Convert.ToInt32(userId)).FirstOrDefault();

                return UserSettings;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

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

        public SettingsInfo UserSettings
        {
            get;
            set;
        }

        #endregion

        #region Check Local Location Service

        public bool LocationEnabled()
        {
            try
            {
                using (ILocalDbProvider dbProvider = Mvx.IoCProvider.Resolve<ILocalDbProvider>())
                {
                    var settings = dbProvider.Get<SettingsInfo>().FirstOrDefault();
                    if (settings != null && settings.LocationEnabled.HasValue)
                    {
                        return settings.LocationEnabled.Value;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
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

        private bool _shouldUpdateMap;

        public bool ShouldUpdateMap
        {
            get { return _shouldUpdateMap; }
            set
            {
                _shouldUpdateMap = value;
                RaisePropertyChanged(() => ShouldUpdateMap);
            }
        }

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

        public bool isFiltered;
        public int _orgId;

        #region Filtering

        public void FilterMerchants(string filter)
        {
            Task.Run(() =>
            {
                try
                {
                    using (ILocalDbProvider dbProvider = Mvx.IoCProvider.Resolve<ILocalDbProvider>())
                    {
                        var addresses = dbProvider.Get<MerchantInfo>()
                                            .Where(x => x.Address.Contains(filter, StringComparison.OrdinalIgnoreCase))
                                            .ToList();
                        var names = dbProvider.Get<MerchantInfo>()
                                        .Where(x => x.MerchantName.Contains(filter, StringComparison.OrdinalIgnoreCase))
                                        .ToList();
                        //var merchants = _dbProvider.Get<MerchantInfo>().Where(x => x.Address.Contains(filter) || x.MerchantName.Contains(filter));
                        var merchants = new List<MerchantInfo>();
                        merchants.AddRange(addresses);
                        merchants.AddRange(names);
                        Merchants = GetMerchantsWithDistance(merchants.ToList());
                        ShouldUpdateMap = true;
                        isFiltered = true;
                    }
                }
                catch (Exception ex)
                {
                    InvokeOnMainThread(() =>
                    {
                        _dialogPlugin.ShowToast("Error occured");
                    });
                }
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

        public async void UpdateMerchants()
        {
            using (ILocalDbProvider dbProvider = Mvx.IoCProvider.Resolve<ILocalDbProvider>())
            {
                var count = dbProvider.GetCount<MerchantInfo>();
                if (count == 0)
                {
                    return;
                }
            }
            Task.Run(async () =>
            {
                BaseActionResult<List<MerchantModel>> response = new BaseActionResult<List<MerchantModel>>();
                response = await _organizationService.GetMerchants(null, null, null);
                using (ILocalDbProvider dbProvider = Mvx.IoCProvider.Resolve<ILocalDbProvider>())
                {
                    var merchants = new List<MerchantInfo>();
                    if (response.Success && response.Result != null)
                    {
                        foreach (var item in response.Result)
                        {
                            var organization = new MerchantInfo
                            {
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
                                UnitScore = item.UnitScore,
                                UnitDescription = item.UnitDescription
                            };
                            merchants.Add(organization);
                        }
                        dbProvider.Execute("Delete from MerchantInfo");
                        dbProvider.Insert<MerchantInfo>(merchants);
                    }
                }
            });
        }

        #endregion

        public void GetMerchants(int orgId = -1)
        {
            _orgId = orgId;
            ShouldUpdateMap = (orgId != -1 || isFiltered);
            Task.Run(async () =>
            {
                try
                {
                    using (ILocalDbProvider dbProvider = Mvx.IoCProvider.Resolve<ILocalDbProvider>())
                    {
                        var existingDataCount = dbProvider.GetCount<MerchantInfo>();
                        if (existingDataCount > 0)
                        {

                            if (orgId > 0)
                            {
                                isFiltered = true;
                                string query = string.Format("select * from MerchantInfo as m where m.OrganizationId = {0}", orgId);
                                Merchants = GetMerchantsWithDistance(dbProvider.Query<MerchantInfo>(query));
                            }
                            else {
                                isFiltered = false;
                                Merchants = GetMerchantsWithDistance(dbProvider.Query<MerchantInfo>("select * from MerchantInfo"));
                            }
                        }
                        else {
                            var response = new BaseActionResult<List<MerchantModel>>();

                            response = await _organizationService.GetMerchants(null, null, null);

                            var merchants = new List<MerchantInfo>();
                            if (response.Success && response.Result != null)
                            {
                                foreach (var item in response.Result)
                                {
                                    var organization = new MerchantInfo
                                    {
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
                                        UnitScore = item.UnitScore,
                                        UnitDescription = item.UnitDescription
                                    };
                                    merchants.Add(organization);
                                }
                            }

                            dbProvider.Insert<MerchantInfo>(merchants);

                            if (orgId > 0)
                            {
                                isFiltered = true;
                                Merchants = GetMerchantsWithDistance(merchants.Where(x => x.OrganizationId == orgId).ToList());
                            }
                            else {
                                isFiltered = false;
                                Merchants = GetMerchantsWithDistance(merchants);
                            }

                            if (!string.IsNullOrEmpty(response.DisplayMessage))
                            {
                                _dialogPlugin.ShowToast(response.DisplayMessage);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    InvokeOnMainThread(() => _dialogPlugin.ShowToast("Error Occured"));
                }
            });
        }

        #region Distance Calculation

        public List<MerchantInfo> GetMerchantsWithDistance(List<MerchantInfo> merchants)
        {
            if (merchants != null && merchants.Count > 0)
            {
                var data = merchants.Select(x =>
                   new MerchantInfo
                   {
                       Address = x.Address,
                       Distance = (x.Latitude.HasValue && x.Longitude.HasValue && Latitude.HasValue && Longitude.HasValue) ?
                           GetDistanceInMetres(Latitude.Value, Longitude.Value, x.Latitude.Value, x.Longitude.Value) : null,
                       DistanceUnit = x.Distance != null ? x.Distance < 1000 ? "მეტრი" : "კმ" : "",
                       MerchantId = x.MerchantId,
                       MerchantName = x.MerchantName,
                       LocalId = x.LocalId,
                       Latitude = x.Latitude,
                       Longitude = x.Longitude,
                       Image = x.Image,
                       Unit = x.Unit,
                       OrganizationId = x.OrganizationId,
                       OrgnizationPointsDesc = x.OrgnizationPointsDesc,
                       UnitScore = x.UnitScore,
                       UnitDescription = x.UnitDescription
                   });
                return data.OrderByDescending(x => x.Distance.HasValue).ThenBy(x => x.Distance).Distinct().ToList();
            }
            return new List<MerchantInfo>();
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
