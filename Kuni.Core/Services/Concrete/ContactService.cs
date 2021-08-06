﻿using Kuni.Core.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kuni.Core.Models;
using Kuni.Core.Models.BusinessModels;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;
using Kuni.Core.Helpers.AppSettings;
using Kuni.Core.Models.DataTransferObjects.Response;

namespace Kuni.Core.Services.Concrete
{
    public class ContactService : IContactService
    {
        private IUnicardApiProvider _apiProvider;
        private IAppSettings _appSettings;

        public ContactService (IUnicardApiProvider apiProvider, IAppSettings appSettings)
        {
            _apiProvider = apiProvider;
            _appSettings = appSettings;
        }
        #region IContactService implementation

        public async Task<BaseActionResult<ContactInfoModel>> GetContactInfo()
        {
            BaseActionResult<ContactInfoModel> result = new BaseActionResult<ContactInfoModel> ();

            var request = new UnicardApiBaseRequest();

            var url = string.Format ("{0}GetContactInfo", _appSettings.UnicardServiceUrl);
            var json = JsonConvert.SerializeObject (request,
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = _apiProvider.Post<GetContactInfoResponse> (url, null, json).Result;
            result.Success = response.Successful;
            result.DisplayMessage = response.DisplayMessage;
            result.Result = new ContactInfoModel();
            if (response != null) {
                result.Result.Email = response.Email;
                result.Result.Facebook = response.Facebook;
                result.Result.PhoneNumber = response.PhoneNumber;
                result.Result.WorkHours = response.WorkHours;
                result.Result.WebPage = response.WebPage;
            }
            return result;
        }

        #endregion
    }
}
