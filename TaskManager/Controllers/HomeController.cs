using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using TaskManager.Models;
using TaskManagerData.Models;
using TaskManagerData.Repository;

namespace TaskManager.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly ITaskManagerRepository _taskManagerRepository;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public HomeController(ILogger<HomeController> logger, ITaskManagerRepository taskManagerRepository,
      UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
      _logger = logger;
      _taskManagerRepository = taskManagerRepository;
      _userManager = userManager;
      _roleManager = roleManager;
    }

    public IActionResult Index()
    {
      HomeViewModel homeView = new HomeViewModel
      {
        Users = _userManager.Users.ToList(), Roles = _roleManager.Roles.ToList(),
        Priorities = _taskManagerRepository.GetAll<Priority>().ToList(),
        Statuses = _taskManagerRepository.GetAll<Status>().ToList(),
        Tasks = _taskManagerRepository.GetAll<Task>().ToList()
      };

      return View("Index", homeView);
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
