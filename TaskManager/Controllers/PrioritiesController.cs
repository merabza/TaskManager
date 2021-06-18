using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerData.Models;
using TaskManagerData.Repository;

namespace TaskManager.Controllers
{
  [Authorize(Roles = "Admin")]
  public class PrioritiesController : Controller
  {
    private readonly ILogger _logger;
    private readonly ITaskManagerRepository _repo;

    public PrioritiesController(ILogger logger, ITaskManagerRepository repo)
    {
      _logger = logger;
      _repo = repo;
    }

    // GET: Priorities
    public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
    {
      ViewData["CurrentSort"] = sortOrder;
      ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";

      if (searchString != null)
        pageNumber = 1;
      else
        searchString = currentFilter;

      ViewData["CurrentFilter"] = searchString;

      IQueryable<Priority> priorities = _repo.GetAll<Priority>();
      if (!string.IsNullOrEmpty(searchString))
        priorities = priorities.Where(s => s.PrtName.Contains(searchString));

      priorities = sortOrder switch
      {
        "Name_desc" => priorities.OrderByDescending(s => s.PrtName),
        _ => priorities.OrderBy(s => s.PrtName)
      };

      int pageSize = 3;
      return View(await PaginatedList<Priority>.CreateAsync(priorities.AsNoTracking(), pageNumber ?? 1, pageSize));
    }

    // GET: Priorities/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Priorities/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("PrtName")]
      Priority priority)
    {
      try
      {
        if (ModelState.IsValid)
        {
          _repo.Insert(priority);
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

      return View(priority);
    }

    // GET: Priorities/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
        return NotFound();

      Priority priority = await _repo.FindAsync<Priority>(id);
      if (priority == null)
        return NotFound();

      return View(priority);
    }

    // POST: Priorities/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int? id)
    {
      if (id == null)
        return NotFound();

      Priority priorityToUpdate = await _repo.GetAll<Priority>().FirstOrDefaultAsync(s => s.PrtId == id);
      
      if (!await TryUpdateModelAsync(priorityToUpdate, "", s => s.PrtName)) 
        return View(priorityToUpdate);

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

      return View(priorityToUpdate);
    }

    // GET: Priorities/Delete/5
    public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
    {
      if (id == null)
      {
        return NotFound();
      }

      Priority priority = await _repo.GetAll<Priority>().AsNoTracking().FirstOrDefaultAsync(m => m.PrtId == id);
      if (priority == null)
      {
        return NotFound();
      }

      if (saveChangesError.GetValueOrDefault())
      {
        ViewData["ErrorMessage"] =
            "Delete failed. Try again, and if the problem persists " +
            "see your system administrator.";
      }

      return View(priority);
    }

    // POST: Priorities/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      Priority priority = await _repo.FindAsync<Priority>(id);

      if (priority == null)
        return RedirectToAction(nameof(Index));

      try
      {
        _repo.Delete(priority);
        await _repo.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      catch (DbUpdateException ex)
      {
        _logger.LogError(ex, "Unable to delete Priority. ");
        return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
      }
    }

    private bool PriorityExists(int id)
    {
      return _repo.GetAll<Priority>().Any(e => e.PrtId == id);
    }

  }
}
