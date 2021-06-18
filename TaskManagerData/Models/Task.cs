namespace TaskManagerData.Models
{

  public class Task
  {

    public int TskId { get; set; }
    public string TskTitle { get; set; }
    public string TskDescription { get; set; }
    public int PriorityId { get; set; }
    public int StatusId { get; set; }

    public Priority PriorityNavigation { get; set; }
    public Status StatusNavigation { get; set; }

  }

}
