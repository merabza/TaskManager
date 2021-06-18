using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerData.Models;
using Task = TaskManagerData.Models.Task;

namespace TaskManagerData.Repository
{
  public class TaskManagerRepository : ITaskManagerRepository
  {
    private readonly TaskManagerDbContext _context;
    private readonly ILogger<TaskManagerRepository> _logger;


    public TaskManagerRepository(TaskManagerDbContext context, ILogger<TaskManagerRepository> logger)
    {
      _context = context;
      _logger = logger;
    }


    public long GetUsersCount()
    {
      return _context.Users.Count();
    }

    public int SaveChanges()
    {
      return _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
      return await _context.SaveChangesAsync();
    }

    public void Insert<T>(T entity) where T : class
    {
      if (entity == null) 
        return;
      _context.Entry(entity).State = EntityState.Added;
      _context.SaveChanges();
    }

    public IQueryable<T> GetAll<T>() where T : class
    {
      return _context.Set<T>();
    }

    public void Delete<T>(T entity) where T : class
    {
      if (entity == null) 
        return;
      _context.Entry(entity).State = EntityState.Deleted;
      _context.SaveChanges();

    }

    public void Update<T>(T entity) where T : class
    {
      if (entity == null) 
        return;
      _context.Set<T>().Attach(entity);
      _context.Entry(entity).State=EntityState.Modified;
      _context.SaveChanges();
    }

    public void Dispose()
    {
      _context.Dispose();
    }

    public async Task<T> FindAsync<T>(int? id) where T : class
    {
      return await _context.Set<T>().FindAsync(id);
    }

    


  }
}
