using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace IdeaApp.Models.Repo.Base
{
    public interface IRepository<TEntity>
{
    TEntity Add(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);

    TEntity GetById(int id);

    IEnumerable<TEntity> GetByAny(Func<TEntity, bool> filter);

    PagedListResult<TEntity> GetByAnyPaging(Func<TEntity, bool> filter, Expression<Func<TEntity, object>> orderBy, int pageIndex, int pageSize, bool isOrderAsc = true);

    DbContext Context {get;}

}

public class PagedListResult<TEntity>
{
    public int TotalRecords { get; set; }
    public IList<TEntity> Records { get; set; }
}
}