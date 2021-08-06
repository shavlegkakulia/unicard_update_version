using System;
using Kunicardus.Billboards.Core.DbModels;

namespace Kunicardus.Billboards.Core
{
    public class BillboardHistoryModel : BillboardHistory
    {
        public string OrganizationLogo{ get; set; }

        public string OrganizationName{ get; set; }
    }
}

