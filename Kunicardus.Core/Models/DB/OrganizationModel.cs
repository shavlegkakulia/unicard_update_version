using System;
using Kunicardus.Core.Models;
using Kunicardus.Core.Models.DB;
using SQLite;

namespace Kunicardus.Core.Models
{
    public class OrganizationModel : DBModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int OrganizationId { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string PointDescription { get; set; }

        public int? SectorId { get; set; }

        public int? SubSectorId { get; set; }

        public string Unit { get; set; }

        public string UnitScore { get; set; }

        public string UnitDescription { get; set; }
    }
}

