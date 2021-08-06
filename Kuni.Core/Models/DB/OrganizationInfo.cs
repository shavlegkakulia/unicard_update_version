using Kuni.Core.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Kuni.Core.Models.DB
{
	public class OrganizationInfo : DBModel
	{
		[PrimaryKey]
		public int OrganizationId { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string Email { get; set; }

		public string FbAddress { get; set; }

		public int PhoneNumber { get; set; }

		public string ServiceDescription { get; set; }

		public string ShortDescription { get; set; }

		public string WorkingHours { get; set; }

		public string PointsDescription { get; set; }

		public int? SectorId { get; set; }

		public int? SubSectorId { get; set; }

		public string Image { get; set; }
	}
}
