using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerData.Models;
using TaskManagerData.Repository;

namespace TaskManager.Controllers
{
  public class StatusesController : Controller
  {
    private readonly ILogger _logger;
    private readonly ITaskManagerRepository _repo;

    public StatusesController(ILogger logger, ITaskManagerRepository repo)
    {
      _logger = logger;
      _repo = repo;
    }

    // GET: Statuses
    public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
    {
      ViewData["CurrentSort"] = sortOrder;
      ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";

      if (searchString != null)
        pageNumber = 1;
      else
        searchString = currentFilter;

      ViewData["CurrentFilter"] = searchString;

      IQueryable<Status> statuses = _repo.GetAll<Status>();
      if (!string.IsNullOrEmpty(searchString))
        statuses = statuses.Where(s => s.SttName.Contains(searchString));

      statuses = sortOrder switch
      {
        "Name_desc" => statuses.OrderByDescending(s => s.SttName),
        _ => statuses.OrderBy(s => s.SttName)
      };

      int pageSize = 3;
      return View(await PaginatedList<Status>.CreateAsync(statuses.AsNoTracking(), pageNumber ?? 1, pageSize));
    }

    // GET: Statuses/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Statuses/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EnrollmentDate,FirstMidName,LastName")]
      Status status)
    {
      try
      {
        if (ModelState.IsValid)
        {
          _repo.Insert(status);
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

      return View(status);
    }

    // GET: Statuses/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
        return NotFound();

      Status status = await _repo.FindAsync<Status>(id);
      if (status == null)
        return NotFound();

      return View(status);
    }

    // POST: Statuses/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int? id)
    {
      if (id == null)
        return NotFound();

      Status statusToUpdate = await _repo.GetAll<Status>().FirstOrDefaultAsync(s => s.SttId == id);
      
      if (!await TryUpdateModelAsync(statusToUpdate, "", s => s.SttName)) 
        return View(statusToUpdate);

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

      return View(statusToUpdate);
    }

    // GET: Statuses/Delete/5
    public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
    {
      if (id == null)
      {
        return NotFound();
      }

      Status status = await _repo.GetAll<Status>().AsNoTracking().FirstOrDefaultAsync(m => m.SttId == id);
      if (status == null)
      {
        return NotFound();
      }

      if (saveChangesError.GetValueOrDefault())
      {
        ViewData["ErrorMessage"] =
            "Delete failed. Try again, and if the problem persists " +
            "see your system administrator.";
      }

      return View(status);
    }

    // POST: Statuses/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      Status status = await _repo.FindAsync<Status>(id);

      if (status == null)
        return RedirectToAction(nameof(Index));

      try
      {
        _repo.Delete(status);
        await _repo.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      catch (DbUpdateException ex)
      {
        _logger.LogError(ex, "Unable to delete status. ");
        return RedirectToAction(nameof(Delete), new {id, saveChangesError = true });
      }
    }

  }
}
