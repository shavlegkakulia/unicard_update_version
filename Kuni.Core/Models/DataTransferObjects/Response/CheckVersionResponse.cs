using System;
using System.Collections.Generic;
using Kuni.Core.Models.DB;
using Kuni.Core.UnicardApiProvider;

namespace Kuni.Core.Models.DataTransferObjects.Response
{
    public class CheckVersionResponse : UnicardApiBaseResponse
    {
        public List<VersionsModel> Versions;
    }
}
