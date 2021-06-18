using TaskManagerData.Models;
using TaskManagerData.Repository;

namespace TaskManagerData
{
  public static class DataInitializer
  {

    public static void SeedData(ITaskManagerRepository repository)
    {
      SeedStatuses(repository);
      SeedPriorities(repository);
      SeedTasks(repository);
    }

    private static void SeedStatuses(ITaskManagerRepository repository)
    {
      repository.Insert(new Status {SttId = 1, SttName = "New"});
      repository.Insert(new Status {SttId = 2, SttName = "InProgress"});
      repository.Insert(new Status {SttId = 3, SttName = "Done"});
      repository.SaveChanges();
    }

    private static void SeedPriorities(ITaskManagerRepository repository)
    {
      repository.Insert(new Priority {PrtId = 1, PrtName = "Blocker"});
      repository.Insert(new Priority {PrtId = 2, PrtName = "Critical"});
      repository.Insert(new Priority {PrtId = 3, PrtName = "High"});
      repository.Insert(new Priority {PrtId = 4, PrtName = "Low"});
      repository.Insert(new Priority {PrtId = 5, PrtName = "Trivial"});
      repository.SaveChanges();
    }

    private static void SeedTasks(ITaskManagerRepository repository)
    {
      repository.Insert(new Task
        {TskId = 1, TskTitle = "ამოცანა 1", TskDescription = "ამოცანა 1-ის აღწერა", PriorityId = 1, StatusId = 2});
      repository.Insert(new Task
        {TskId = 2, TskTitle = "ამოცანა 2", TskDescription = "ამოცანა 2-ის აღწერა", PriorityId = 3, StatusId = 1});
      repository.Insert(new Task
        {TskId = 3, TskTitle = "ამოცანა 3", TskDescription = "ამოცანა 3-ის აღწერა", PriorityId = 4, StatusId = 3});
      repository.SaveChanges();
    }


  }
}
