using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using TaskManagerData.Models;

namespace TaskManager.Models
{
  public class HomeViewModel
  {
    public List<IdentityUser> Users { get; set; }
    public List<IdentityRole> Roles { get; set; }
    public virtual List<Status> Statuses { get; set; }
    public virtual List<Priority> Priorities { get; set; }
    public virtual List<Task> Tasks { get; set; }


  }
}
