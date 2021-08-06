using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuni.Core.Models.BusinessModels
{
    public class MerchantModel
    {
        public int LocalId { get; set; }
        public string MerchantId { get; set; }
        public string MerchantName { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public int OrganizationId { get; set; }
        public string OrgnizationPointsDesc { get; set; }
        public string Unit { get; set; }
        public string UnitDescription { get; set; }
        public string UnitScore { get; set; }
        public string Image { get; set; }
    }
}
