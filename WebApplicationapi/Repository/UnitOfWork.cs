using System;
using System.Threading.Tasks;
using WebApplicationapi.Data;
using WebApplicationapi.IRepository;

namespace WebApplicationapi.Repository
{
  public class UnitOfWork : IUnitOfWork
  {
    private readonly DatabaseContext _context;

    private IGenericRepository<Hotel> _Hotels;
    private IGenericRepository<Country> _Countries;


    public UnitOfWork(DatabaseContext context)
    {
      _context = context;
    }

    public IGenericRepository<Country> Countries => _Countries ??= new GenericRepository<Country>(_context);
    public IGenericRepository<Hotel> Hotels => _Hotels ??= new GenericRepository<Hotel>(_context);

    public void Dispose()
    {
      _context.Dispose();
      GC.SuppressFinalize(true);
    }

    public async Task Save()
    {
      await _context.SaveChangesAsync();
    }
  }
}
