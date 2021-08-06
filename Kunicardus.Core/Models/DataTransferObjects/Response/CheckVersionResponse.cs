using System;
using System.Collections.Generic;
using Kunicardus.Core.Models.DB;
using Kunicardus.Core.UnicardApiProvider;

namespace Kunicardus.Core.Models.DataTransferObjects.Response
{
    public class CheckVersionResponse : UnicardApiBaseResponse
    {
        public List<VersionsModel> Versions;
    }
}
