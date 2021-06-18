using System.Collections.Generic;

namespace TaskManagerData.Models
{

  public class Status
  {

    public Status()
    {
      Tasks = new HashSet<Task>();
    }

    public int SttId { get; set; }
    public string SttName { get; set; }

    public ICollection<Task> Tasks { get; set; }
  }
}
