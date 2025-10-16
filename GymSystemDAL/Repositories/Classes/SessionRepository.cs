using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemDAL.Data.Contexts;
using GymSystemDAL.Entities;
using GymSystemDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymSystemDAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymSystemDbContext _dbContext;

        // Chain CTOR Parent
        public SessionRepository(GymSystemDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Session> GetAllSessionsWithTrainerAndCategory()
        {
            return _dbContext.Sessions
                .Include(X => X.SessionTrainer)
                .Include(X => X.SessionCategory)
                .ToList();
        }

        public int GetCountOfBookedSlot(int sessionId)
        {
            return _dbContext.MemberSessions.Count(X => X.SessionId == sessionId);
        }

        public Session? GetSessionWithTrainerAndCategory(int sessionId)
        {
            return _dbContext.Sessions
                .Include(X => X.SessionTrainer)
                .Include(X => X.SessionCategory)
                .FirstOrDefault(X => X.Id == sessionId);
        }
    }
}
