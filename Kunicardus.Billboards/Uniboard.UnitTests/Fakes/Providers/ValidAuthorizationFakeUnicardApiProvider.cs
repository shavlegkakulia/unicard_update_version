using Kunicardus.Billboards.Core.Models.DataTransferObjects;
using Kunicardus.Billboards.Core.Services.UnicardApiProvider;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uniboard.UnitTests.Fakes
{
    public class ValidAuthorizationFakeUnicardApiProvider : IUnicardApiProvider
    {
        public TResultObject UnsecuredPost<TResultObject>(string url, Dictionary<string, string> headers, string body) where TResultObject : UnicardApiBaseResponse
        {
            throw new NotImplementedException();
        }

        public TResultObject Post<TResultObject>(string url, Dictionary<string, string> headers, string body) where TResultObject : UnicardApiBaseResponse
        {
            return new AuthResponse
            {
                DisplayMessage = "",
                ErrorMessage = "",
                ResultCode = "200",
                SessionID = "123456789",
                UserId = "8491"
            } as TResultObject;
        }

        public BillboardsBaseResponse<TResultObject> PostToApi<TResultObject>(string url, Dictionary<string, string> headers, string body) where TResultObject : class, new()
        {
            throw new NotImplementedException();
        }

        public BillboardsBaseResponse<TResultObject> GetFromApi<TResultObject>(string url, Dictionary<string, string> headers) where TResultObject : class, new()
        {
            throw new NotImplementedException();
        }

        public Kunicardus.Billboards.Core.Models.UserModel UpdateSession()
        {
            throw new NotImplementedException();
        }
    }
}
