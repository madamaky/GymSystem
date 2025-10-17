using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystemDAL.Entities
{
    public class Session : BaseEntity
    {
        public string Description { get; set; } = null!;
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        #region Category 1 : M Session

        public int CategoryId { get; set; }
        public Category SessionCategory { get; set; }

        #endregion

        #region Trainer 1 : M Session

        public int TrainerId { get; set; }
        public Trainer SessionTrainer { get; set; }

        #endregion

        #region Member M : M Session

        public ICollection<MemberSession> SessionMembers { get; set; }

        #endregion
    }
}
