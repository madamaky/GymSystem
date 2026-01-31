using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemDAL.Entities;

namespace GymSystemDAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        // Call IUnitOfWork For Asking Specific Repository
        // Method to Return Repository
        // Method SaveChanges()

        public ISessionRepository SessionRepository { get; }
        public IMembershipRepository MembershipRepository { get; }
        public IBookingRepository BookingRepository { get; }

        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();
        int SaveChanges();
    }
}
