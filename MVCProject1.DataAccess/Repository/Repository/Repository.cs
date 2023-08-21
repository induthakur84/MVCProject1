using Microsoft.EntityFrameworkCore;
using MVCProject1.DataAccess.Data;
using MVCProject1.DataAccess.Repository.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MVCProject1.DataAccess.Repository.Repository
{
    public class Repository<T> : IRepository<T> where T : class  /// in which we use inheritance we 
        //we inherit IRepository
    {
        private readonly ApplicationDbContext _context; //it helps to communicate with database
        internal DbSet<T>  dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context= context;
            this.dbSet= context.Set<T>();
            // this constuctor is to call another constructor from constructor in the same class.
        }


        public void Add(T entity) // it is used to save entity
        {
            dbSet.Add(entity);
        }

        public T FirstOrDefult(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public T Get(int id)   // it is used to find Id and this is used for multiple Tables.
        {
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
           IQueryable<T> query = dbSet;
            if(filter!=null)
                query = query.Where(filter);
            if(includeProperties!=null) //it only works when foreign key is maded.
            {
                foreach(var includeProp in includeProperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query=query.Include(includeProp);
                }
            }
            if(orderBy!=null)
                return orderBy(query).ToList();
            return query.ToList();  // for display
        }

        public void Remove(T entity) // it is used to remove entity.
        {
            dbSet.Remove(entity);
        }

        public void Remove(int id)
        {

            T entity = dbSet.Find(id);
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
