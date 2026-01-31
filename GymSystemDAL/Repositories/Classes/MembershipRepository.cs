using GymSystemDAL.Data.Contexts;
using GymSystemDAL.Entities;
using GymSystemDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymSystemDAL.Repositories.Classes
{
    public class MembershipRepository : GenericRepository<Membership>, IMembershipRepository
    {
        private readonly GymSystemDbContext _context;

        public MembershipRepository(GymSystemDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Membership> GetAllMembershipsWithMembersAndPlans(Func<Membership, bool>? filter = null)
        {
            var memberships = _context.Memberships
                .Include(m => m.Member)
                .Include(m => m.Plan)
                .Where(filter ?? (_ => true));

            return memberships;
        }

        public Membership? GetFirstOrDefault(Func<Membership, bool>? filter = null)
            => _context.Memberships.FirstOrDefault(filter ?? (_ => true));
    }
}
