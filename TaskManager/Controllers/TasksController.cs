using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerData.Repository;
using Task = TaskManagerData.Models.Task;

namespace TaskManager.Controllers
{
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

      if (searchString != null)
        pageNumber = 1;
      else
        searchString = currentFilter;

      ViewData["CurrentFilter"] = searchString;

      IQueryable<Task> tasks = _repo.GetAll<Task>();
      if (!string.IsNullOrEmpty(searchString))
        tasks = tasks.Where(s => s.TskTitle.Contains(searchString));

      tasks = sortOrder switch
      {
        "Title_desc" => tasks.OrderByDescending(s => s.TskTitle),
        "Description" => tasks = tasks.OrderBy(s => s.TskDescription),
        "Description_desc" => tasks = tasks.OrderByDescending(s => s.TskDescription),
        _ => tasks.OrderBy(s => s.TskTitle)
      };

      int pageSize = 3;
      return View(await PaginatedList<Task>.CreateAsync(tasks.AsNoTracking(), pageNumber ?? 1, pageSize));
    }

    // GET: Tasks/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Tasks/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EnrollmentDate,FirstMidName,LastName")]
      Task task)
    {
      try
      {
        if (ModelState.IsValid)
        {
          _repo.Insert(task);
          await _repo.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
        }
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

      Task task = await _repo.FindAsync<Task>(id);
      if (task == null)
        return NotFound();

      return View(task);
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

      if (!await TryUpdateModelAsync(taskToUpdate, "", s => s.TskTitle))
        return View(taskToUpdate);

      try
      {
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

      return View(taskToUpdate);
    }

    // GET: Tasks/Delete/5
    public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
    {
      if (id == null)
      {
        return NotFound();
      }

      Task task = await _repo.GetAll<Task>().AsNoTracking().FirstOrDefaultAsync(m => m.TskId == id);
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

  }
}
