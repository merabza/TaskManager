using System;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagerData.Repository
{
  public interface ITaskManagerRepository : IDisposable
  {
    long GetUsersCount();
    int SaveChanges();
    Task<int> SaveChangesAsync();

    void Insert<T>(T entity) where T : class;
    IQueryable<T> GetAll<T>() where T : class;
    void Delete<T>(T entity) where T : class;
    void Update<T>(T entity) where T : class;
    Task<T> FindAsync<T>(int? id) where T : class;
    void SetTaskAsDone(int id);
  }
}