using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemDAL.Entities.Enums;

namespace GymSystemDAL.Entities
{
    internal class Trainer : GymUser
    {
        // CreatedAt Column Exited In BaseEntity
        // I Will Use This Column As HireDate For Member => Configurations

        public Specialties Specialties { get; set; }

        #region Trainer 1 : M Session

        public ICollection<Session> TrainerSessions { get; set; }

        #endregion
    }
}
