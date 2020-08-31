
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace IdeaApp.Models.Repo.Base{

    public class MainRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {

        private DbContext _context;
        public MainRepository(DbContext context)
        {
            _context= context;
            
        }
        public TEntity Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
            return entity;
            // return this.GetById(entity);
            
        }

        public virtual void Delete(TEntity entity)
        {
           _context.Set<TEntity>().Remove(entity);
           _context.SaveChanges();
            
        }


        public virtual IEnumerable<TEntity> GetByAny(Func<TEntity, bool> filter)
        {
            return _context.Set<TEntity>().Where(filter);
        }

        public virtual PagedListResult<TEntity> GetByAnyPaging(Func<TEntity, bool> filter, Expression<Func<TEntity, object>> orderBy, int pageIndex, int pageSize, bool isOrderAsc = true)
        {
            var query = _context.Set<TEntity>();

            var totalRecords = this.GetByAny(filter).Count();

            IList<TEntity> result;
            
            if (isOrderAsc)
                result = query.OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            else
                result = query.OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PagedListResult<TEntity> { Records = result, TotalRecords = totalRecords };
        }

        public virtual TEntity GetById(int id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public void Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();

        }
    }

}