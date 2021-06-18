using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerData.Models;
using TaskManagerData.Repository;
using Task = TaskManagerData.Models.Task;

namespace TaskManager.Controllers
{
  [Authorize]
  public class TasksController : Controller
  {
    private readonly ILogger _logger;
    private readonly ITaskManagerRepository _repo;

    public TasksController(ILogger logger, ITaskManagerRepository repo)
    {
      _logger = logger;
      _repo = repo;
    }

    // GET: Tasks
    public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
    {
      ViewData["CurrentSort"] = sortOrder;
      ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Title_desc" : "";
      ViewData["DescriptionSortParam"] = sortOrder == "Description" ? "Description_desc" : "Description";
      ViewData["PrioritySortParam"] = sortOrder == "Priority" ? "Priority_desc" : "Priority";
      ViewData["StatusSortParam"] = sortOrder == "Status" ? "Status_desc" : "Status";

      if (searchString != null)
        pageNumber = 1;
      else
        searchString = currentFilter;

      ViewData["CurrentFilter"] = searchString;

      IQueryable<Task> tasks = _repo.GetAll<Task>().Include(i => i.PriorityNavigation).Include(i => i.StatusNavigation);
      if (!string.IsNullOrEmpty(searchString))
        tasks = tasks.Where(s => s.TskTitle.Contains(searchString));

      tasks = sortOrder switch
      {
        "Title_desc" => tasks.OrderByDescending(s => s.TskTitle),
        "Description" => tasks = tasks.OrderBy(s => s.TskDescription),
        "Description_desc" => tasks = tasks.OrderByDescending(s => s.TskDescription),
        "Priority" => tasks = tasks.OrderBy(s => s.PriorityNavigation.PrtName),
        "Priority_desc" => tasks = tasks.OrderByDescending(s => s.PriorityNavigation.PrtName),
        "Status" => tasks = tasks.OrderBy(s => s.StatusNavigation.SttName),
        "Status_desc" => tasks = tasks.OrderByDescending(s => s.StatusNavigation.SttName),
        _ => tasks.OrderBy(s => s.TskTitle)
      };

      int pageSize = 3;
      return View(await PaginatedList<Task>.CreateAsync(tasks.AsNoTracking(), pageNumber ?? 1, pageSize));
    }

    // GET: Tasks/Create
    public IActionResult Create()
    {
      PopulatePrioritiesDropDownList();
      PopulateStatusesDropDownList();
      return View();
    }

    // POST: Tasks/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost, ActionName("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TskTitle,TskDescription,PriorityId,StatusId")] Task task)
    {
      try
      {
        if (ModelState.IsValid)
        {
          //თუ შექმნას ცდილობს ნაკლები უფლებების მქონე მომხმარებელი,
          //ან თუ სტატუსი მითითებული არ არის მიეთითოს New
          if (!HttpContext.User.IsInRole("Admin") && !HttpContext.User.IsInRole("Support") || task.StatusId == 0)
            task.StatusId = 1;
          _repo.Insert(task);
          await _repo.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
        }
        PopulatePrioritiesDropDownList(task.PriorityId);
        PopulateStatusesDropDownList(task.StatusId);
        return View(task);
      }
      catch (DbUpdateException ex)
      {
        //Log the error (uncomment ex variable name and write a log.
        ModelState.AddModelError("", "Unable to save changes. " +
                                     "Try again, and if the problem persists " +
                                     "see your system administrator.");
        _logger.LogError(ex, "Unable to save changes. ");
      }

      return View(task);
    }

    // GET: Tasks/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
        return NotFound();

      Task task = await _repo.GetAll<Task>().Include(i=>i.StatusNavigation).SingleOrDefaultAsync(m => m.TskId == id);
      if (task == null)
        return NotFound();
      PopulatePrioritiesDropDownList(task.PriorityId);
      PopulateStatusesDropDownList(task.StatusId);
      return View(task);
    }

    private void PopulateStatusesDropDownList(object selectedStatus = null)
    {
      IOrderedQueryable<Status> departmentsQuery = _repo.GetAll<Status>().OrderBy(o=>o.SttName);
      ViewBag.StatusId = new SelectList(departmentsQuery.AsNoTracking(), nameof(Status.SttId), nameof(Status.SttName),
        selectedStatus);
    }

    private void PopulatePrioritiesDropDownList(object selectedPriority = null)
    {
      IOrderedQueryable<Priority> departmentsQuery = _repo.GetAll<Priority>().OrderBy(o=>o.PrtName);
      ViewBag.PriorityId = new SelectList(departmentsQuery.AsNoTracking(), nameof(Priority.PrtId),
        nameof(Priority.PrtName), selectedPriority);
    }

    // POST: Tasks/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int? id)
    {
      if (id == null)
        return NotFound();

      Task taskToUpdate = await _repo.GetAll<Task>().FirstOrDefaultAsync(s => s.TskId == id);
      
      if ( HttpContext.User.IsInRole("Admin") || HttpContext.User.IsInRole("Support") )
      {
        if (!await TryUpdateModelAsync(taskToUpdate, "", s => s.TskTitle, s => s.TskDescription, s => s.PriorityId,
          s => s.StatusId))
          return View(taskToUpdate);
      }
      else
      {//ჩვეულებრივი მომხმარებელი სტატუსს ვერ არედაქტირებს
        if (!await TryUpdateModelAsync(taskToUpdate, "", s => s.TskTitle, s => s.TskDescription, s => s.PriorityId))
          return View(taskToUpdate);
      }


      try
      {
        //_repo.Update(taskToUpdate);
        await _repo.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      catch (DbUpdateException ex)
      {
        //Log the error (uncomment ex variable name and write a log.)
        ModelState.AddModelError("", "Unable to save changes. " +
                                     "Try again, and if the problem persists, " +
                                     "see your system administrator.");
        _logger.LogError(ex, "Unable to save changes. ");
      }
      PopulatePrioritiesDropDownList(taskToUpdate.PriorityId);
      PopulateStatusesDropDownList(taskToUpdate.StatusId);
      return View(taskToUpdate);
    }

    // GET: Tasks/Delete/5
    public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
    {
      if (id == null)
      {
        return NotFound();
      }

      if (!HttpContext.User.IsInRole("Admin"))
      {
        _logger.LogError("User does not have Task Delete Right");
        return RedirectToAction(nameof(Index));
      }


      Task task = await _repo.GetAll<Task>().AsNoTracking().Include(i => i.StatusNavigation)
        .Include(i => i.PriorityNavigation).SingleOrDefaultAsync(m => m.TskId == id);
      if (task == null)
      {
        return NotFound();
      }

      if (saveChangesError.GetValueOrDefault())
      {
        ViewData["ErrorMessage"] =
            "Delete failed. Try again, and if the problem persists " +
            "see your system administrator.";
      }

      return View(task);
    }

    // POST: Tasks/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      if (!HttpContext.User.IsInRole("Admin"))
      {
        _logger.LogError("User does not have Task Delete Right");
        return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
      }

      Task task = await _repo.FindAsync<Task>(id);

      if (task == null)
        return RedirectToAction(nameof(Index));

      try
      {
        _repo.Delete(task);
        await _repo.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      catch (DbUpdateException ex)
      {
        _logger.LogError(ex, "Unable to delete tsk. ");
        return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
      }
    }



    // Mark the task as a done
    public ActionResult Done(int id)
    {

      _repo.SetTaskAsDone(id);

      return RedirectToAction("Index");
    }


  }
}
