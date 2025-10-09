using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystemDAL.Entities
{
    internal class Member : GymUser
    {
        // CreatedAt Column Exited In BaseEntity
        // I Will Use This Column As JoinDate For Member => Configurations

        public string? Photo { get; set; }

        #region Member 1 : 1 HealthRecord

        // Nav Prop
        public HealthRecord HealthRecord { get; set; }

        #endregion

        #region Member M : M Plan

        public ICollection<Membership> Memberships { get; set; }

        #endregion

        #region Member M : M Session

        public ICollection<MemberSession> MemberSessions { get; set; }

        #endregion
    }
}
