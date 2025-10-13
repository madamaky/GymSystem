using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemDAL.Entities;

namespace GymSystemDAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        TEntity? GetById(int id);
        IEnumerable<TEntity> GetAll(Func<TEntity, bool>? condition = null);

        int Add(TEntity entity);
        int Delete(TEntity entity);
        int Update(TEntity entity);

        // If there is some entity that has extra operation
        // We create an Interface for it and makes it inherite this interface
    }
}
