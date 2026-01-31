using GymSystemDAL.Data.Contexts;
using GymSystemDAL.Entities;
using GymSystemDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymSystemDAL.Repositories.Classes
{
    public class BookingRepository : GenericRepository<MemberSession>, IBookingRepository
    {
        private readonly GymSystemDbContext _context;

        public BookingRepository(GymSystemDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<MemberSession> GetSessionsById(int sessionId)
            => _context.MemberSessions
                .Where(ms => ms.SessionId == sessionId)
                .Include(ms => ms.Member)
                .ToList();
    }
}
