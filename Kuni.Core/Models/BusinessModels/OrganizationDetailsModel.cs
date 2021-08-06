using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuni.Core.Models
{
    public class OrganizationDetailsModel
    {
        public int OrganizationId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Email { get; set; }

        public string FbAddress { get; set; }

        public string PhoneNumber { get; set; }

        public List<string> PhoneNumbers { get; set; }

        public string ServiceDescription { get; set; }

        public string ShortDescription { get; set; }

        public string WorkingHours { get; set; }

        public string Website { get; set; }

        public string ImageUrl { get; set; }

        public string Unit { get; set; }

        public string UnitScore { get; set; }

        public string UnitDescription { get; set; }
    }
}
