using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
      //_context.Add(entity);
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

    public void SetTaskAsDone(int id)
    {
      const string doneStatusName = "Done";
      var doneStatus = _context.Statuses.SingleOrDefault(s => s.SttName == doneStatusName);
      if ( doneStatus == null )
        return;
      var task = _context.Tasks.SingleOrDefault(s => s.TskId == id);
      if ( task == null )
        return;
      task.StatusId = doneStatus.SttId;
      _context.SaveChanges();
    }
  }
}
