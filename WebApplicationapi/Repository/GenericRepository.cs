using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplicationapi.Data;
using WebApplicationapi.IRepository;
using WebApplicationapi.Models;
using X.PagedList;

namespace WebApplicationapi.Repository
{
  public class GenericRepository<T> : IGenericRepository<T> where T : class
  {
    private readonly DatabaseContext _context;
    private readonly DbSet<T> _db;

    public GenericRepository(DatabaseContext context)
    {
      _context = context;
      _db = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression = null,
      Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
    {
      IQueryable<T> query = _db;

      if (expression != null)
      {
        query = query.Where(expression);
      }

      if (includes != null)
      {
        includes.ForEach(includeProperty => query = query.Include(includeProperty));
      }

      if (orderBy != null)
      {
        query = orderBy(query);
      }

      return await query.AsNoTracking().ToListAsync();
    }


    public async Task<IPagedList<T>> GetPagedList(
      PagingRequestParams pagingRequestParams, List<string> includes = null)
    {
      IQueryable<T> query = _db;

      if (includes != null)
      {
        includes.ForEach(includeProperty => query = query.Include(includeProperty));
      }

      return await query.AsNoTracking().ToPagedListAsync(pagingRequestParams.PageNumber, pagingRequestParams.PageSize);
    }

    public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null)
    {
      IQueryable<T> query = _db;

      if (includes != null)
      {
        includes.ForEach(includeProperty => query = query.Include(includeProperty));
      }

      return await query.AsNoTracking().FirstOrDefaultAsync(expression);
    }

    public async Task Insert(T entity)
    {
      await _db.AddAsync(entity);
    }

    public async Task InsertRange(IEnumerable<T> entities)
    {
      await _db.AddRangeAsync(entities);
    }

    public async Task Delete(int id)
    {
      var entity = await _db.FindAsync(id);
      _db.Remove(entity);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
      _db.RemoveRange(entities);
    }

    public void Update(T entity)
    {
      _db.Attach(entity);
      _context.Entry(entity).State = EntityState.Modified;
    }
  }
}
