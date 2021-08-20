using System;
using System.Threading.Tasks;
using WebApplicationapi.Data;

namespace WebApplicationapi.IRepository
{
  public interface IUnitOfWork : IDisposable
  {
    public IGenericRepository<Country> Countries { get; }
    public IGenericRepository<Hotel> Hotels { get; }

    public Task Save();
  }
}
