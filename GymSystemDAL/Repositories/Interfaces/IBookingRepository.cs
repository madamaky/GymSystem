using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemDAL.Entities;

namespace GymSystemDAL.Repositories.Interfaces
{
    public interface IBookingRepository : IGenericRepository<MemberSession>
    {
        IEnumerable<MemberSession> GetSessionsById(int sessionId);
    }
}
