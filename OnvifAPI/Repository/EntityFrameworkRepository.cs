using Microsoft.EntityFrameworkCore;
using OnvifAPI.Interfaces;

namespace OnvifAPI.Repository
{
    public class EntityFrameworkRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public EntityFrameworkRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public T GetById(int id)
        {
            try
            {
                return _dbSet.Find(id);
            }
            catch (Exception)
            {

                return null;
            }
        }

        public IEnumerable<T> GetAll()
        {
            try
            {
                return _dbSet.ToList();
            }
            catch (Exception)
            {

                return null;
            }
        }

        public T Add(T entity)
        {
            try
            {
                _dbSet.Add(entity);
                _context.SaveChanges();
                return entity;
            }
            catch (Exception )
            {

                return null;
            }
           
        }


        public IEnumerable<T> AddList(IEnumerable<T> entitys)
        {
            try
            {
                _dbSet.AddRange(entitys);
                _context.SaveChanges();
                return entitys;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public T Update(T entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();
                return entity;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public bool Delete(int id)
        {
            try
            {

                T entity= _dbSet.Find(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
