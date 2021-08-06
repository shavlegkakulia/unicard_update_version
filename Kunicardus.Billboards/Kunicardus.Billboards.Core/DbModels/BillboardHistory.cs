using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using Kunicardus.Billboards.Core.Enums;

namespace Kunicardus.Billboards.Core.DbModels
{
    public class BillboardHistory
    {
        [PrimaryKey, AutoIncrement]
        public int HistoryId { get; set; }

        [Indexed]
        public int BillboardId { get; set; }

        [Indexed]
        public int AdvertismentId { get; set; }

        public DateTime PassDate { get; set; }

        public string Image { get; set; }

        public double Points { get; set; }

        public string ExternalLink { get; set; }

        public int TimeOut { get; set; }

        public AdvertismentStatus Status { get; set; }

        public DateTime ViewDate { get; set; }

        public string UserId { get; set; }
    }
}