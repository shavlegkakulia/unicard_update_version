using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;







using SQLite;

namespace Kunicardus.Billboards.Core.DbModels
{
    public class UserInfo
    {
        [PrimaryKey]
        public string UserId { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string FullAddress { get; set; }

        public string Phone { get; set; }

        public string PersonalId { get; set; }
        
        public decimal Balance_AvailablePoints { get; set; }

        public bool IsFacebookUser { get; set; }
    }
}