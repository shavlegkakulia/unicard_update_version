using Kunicardus.Billboards.Core.Models.DTOs;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uniboard.UnitTests.Fakes.Providers
{
    public  class InValidUserBalanceFakeUnicardApiProvider : IUnicardApiProvider
    {
        public TResultObject UnsecuredPost<TResultObject>(string url, Dictionary<string, string> headers, string body) where TResultObject : UnicardApiBaseResponse
        {
            throw new NotImplementedException();
        }

        public TResultObject Post<TResultObject>(string url, Dictionary<string, string> headers, string body) where TResultObject : UnicardApiBaseResponse
        {
            return new GetUserBalanceResponse
            {
                AvailablePoints = 50.5m,
                DisplayMessage = "adasdadsadas",
                ErrorMessage = "",
                ResultCode = "115",
            } as TResultObject;
        }

        public Kunicardus.Billboards.Core.Services.UnicardApiProvider.BillboardsBaseResponse<TResultObject> PostToApi<TResultObject>(string url, Dictionary<string, string> headers, string body) where TResultObject : class, new()
        {
            throw new NotImplementedException();
        }

        public Kunicardus.Billboards.Core.Services.UnicardApiProvider.BillboardsBaseResponse<TResultObject> GetFromApi<TResultObject>(string url, Dictionary<string, string> headers) where TResultObject : class, new()
        {
            throw new NotImplementedException();
        }

        public Kunicardus.Billboards.Core.Models.UserModel UpdateSession()
        {
            throw new NotImplementedException();
        }
    }
}
