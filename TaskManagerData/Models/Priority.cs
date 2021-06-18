using System.Collections.Generic;

namespace TaskManagerData.Models
{

  public class Priority
  {

    public Priority()
    {
      Tasks = new HashSet<Task>();
    }


    public int PrtId { get; set; }
    public string PrtName { get; set; }

    public ICollection<Task> Tasks { get; set; }

  }


}
