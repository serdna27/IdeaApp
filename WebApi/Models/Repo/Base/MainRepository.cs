
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Logging;

namespace IdeaApp.Models.Repo.Base{

    public class MainRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {

        private DbContext _context;

        private ILogger _logger;

        
        public MainRepository(DbContext context,ILogger logger)
        {
            _context= context;
            _logger = logger;
            
            
        }


        public DbContext Context => _context;

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

        public virtual PagedListResult<TEntity> GetByAnyPaging(Func<TEntity, bool> filter, Func<TEntity, object> orderBy, int pageIndex, int pageSize, bool isOrderAsc = true)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            var totalRecords = this.GetByAny(filter).Count();

            IList<TEntity> result;
            if (isOrderAsc)
                result = query.Where(filter).OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            else
                result = query.Where(filter).OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            // var c = query.ToSql<TEntity>();
            
            // _logger.LogDebug($"query mmg==>{this.ToSql(query)}");
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