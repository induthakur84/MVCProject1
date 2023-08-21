using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MVCProject1.DataAccess.Repository.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T Get(int id); // for find id and return class
        IEnumerable<T> GetAll(                         //this is for display
            Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null);
        public T FirstOrDefult    /// it helps to find id for multiple tables
        (
            Expression<Func<T,bool>> filter = null,
            string includeProperties = null  //Category, CoverType
        );
        void Add(T entity);
        void Remove(T entity);
        void Remove(int id);
        void RemoveRange(IEnumerable<T> entity); 
        // if we want remove multiple record.
    }
}
//we can implement more than one interface in single class
// in interface we just declaration of member not defining member.