using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApplicationapi.Models;
using X.PagedList;

namespace WebApplicationapi.IRepository
{
  public interface IGenericRepository<T> where T : class
  {
    Task<IEnumerable<T>> GetAll(
      Expression<Func<T, bool>> expression = null,
      Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
      List<string> includes = null
    );

    Task<IPagedList<T>> GetPagedList(
      PagingRequestParams pagingRequestParams,
      List<string> includes = null
    );

    Task<T> Get(
      Expression<Func<T, bool>> expression,
      List<string> includes = null
    );

    Task Insert(T entity);
    Task InsertRange(IEnumerable<T> entities);

    Task Delete(int id);
    void DeleteRange(IEnumerable<T> entities);

    void Update(T entity);
  }
}
