using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystemDAL.Entities
{
    internal class Membership : BaseEntity
    {
        public int MemberId { get; set; } // FK From Member
        public Member Member { get; set; }

        public int PlanId { get; set; }
        public Plan Plan { get; set; }

        // StartDate == CteatedAt => BaseEntity

        public DateTime EndDate { get; set; }

        public string Status
        {
            get
            {
                if (EndDate <= DateTime.Now)
                    return "Expired !";
                else
                    return "Active";
            }
        }
    }
}
