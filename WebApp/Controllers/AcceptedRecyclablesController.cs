using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Infrastructure;
using WebApp.Models;
using WebApp.Models.SortStates;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class AcceptedRecyclablesController : Controller
    {
        private readonly RecPointContext _context;
        private int _pageSize = 50;
        private string _currentPage = "pageAcceptedRec";
        private string _currentSortOrder = "sortOrderAcceptedRec";
        private string _currentFilterRecType = "searchRecTypeAccRec";
        private string _currentFilterStorage = "searchPositionAccRec";
        private string _currentFilterEmployee = "searchEmployeeAccRec";

        public AcceptedRecyclablesController(RecPointContext context)
        {
            _context = context;
        }

        // GET: AcceptedRecyclables
        public IActionResult Index(SortStateAcceptedRec? sortOrder, string searchRecTypeAccRec, string searchStorageAccRec, int? page, bool resetFilter = false)
        {
            IQueryable<AcceptedRecyclable> acceptedRecyclables =
                _context.AcceptedRecyclables.Include(ar => ar.Employee)
                .Include(ar => ar.Storage).Include(ar => ar.RecyclableType);
            sortOrder ??= GetSortStateFromSessionOrSetDefault();
            page ??= GetCurrentPageFromSessionOrSetDefault();
            if (resetFilter)
            {
                HttpContext.Session.Remove(_currentFilterRecType);
                HttpContext.Session.Remove(_currentFilterStorage);
            }
            searchRecTypeAccRec ??= GetCurrentFilterRecTypeOrSetDefault();
            searchStorageAccRec ??= GetCurrentFilterStorageOrSetDefault();
            acceptedRecyclables = Search(acceptedRecyclables, (SortStateAcceptedRec)sortOrder, searchRecTypeAccRec, searchStorageAccRec);
            var count = acceptedRecyclables.Count();
            PageViewModel pageViewModel = new PageViewModel(count, (int)page, _pageSize);
            acceptedRecyclables = acceptedRecyclables.Skip(((int)pageViewModel.PageNumber - 1) * _pageSize).Take(_pageSize);
            SaveValuesInSession((SortStateAcceptedRec)sortOrder, (int)page, searchRecTypeAccRec, searchStorageAccRec);
            IndexViewModel<AcceptedRecyclable> acceptedRecyclablesView = new IndexViewModel<AcceptedRecyclable>()
            {
                Items = acceptedRecyclables,
                PageViewModel = pageViewModel
            };
            return View(acceptedRecyclablesView);
        }

        // GET: AcceptedRecyclables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AcceptedRecyclables == null)
            {
                return NotFound();
            }

            var acceptedRecyclable = await _context.AcceptedRecyclables
                .Include(a => a.Employee)
                .Include(a => a.RecyclableType)
                .Include(a => a.Storage)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (acceptedRecyclable == null)
            {
                return NotFound();
            }

            return View(acceptedRecyclable);
        }

        // GET: AcceptedRecyclables/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(
                _context.Employees.Select(e => new
                {
                    Id = e.Id,
                    FullName = e.Surname + " " + e.Name + " " + e.Patronymic
                }),
                "Id", "FullName");
            ViewData["RecyclableTypeId"] = new SelectList(_context.RecyclableTypes, "Id", "Name");
            ViewData["StorageId"] = new SelectList(_context.Storages, "Id", "Name");
            return View();
        }

        // POST: AcceptedRecyclables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RecyclableTypeId,EmployeeId,StorageId,Quantity,Date")] AcceptedRecyclable acceptedRecyclable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(acceptedRecyclable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(
                _context.Employees.Select(e => new
                {
                    Id = e.Id,
                    FullName = e.Surname + " " + e.Name + " " + e.Patronymic
                }),
                "Id", "FullName", acceptedRecyclable.EmployeeId);
            ViewData["RecyclableTypeId"] = new SelectList(_context.RecyclableTypes, "Id", "Name", acceptedRecyclable.RecyclableTypeId);
            ViewData["StorageId"] = new SelectList(_context.Storages, "Id", "Name", acceptedRecyclable.StorageId);
            return View(acceptedRecyclable);
        }

        // GET: AcceptedRecyclables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AcceptedRecyclables == null)
            {
                return NotFound();
            }

            var acceptedRecyclable = await _context.AcceptedRecyclables.FindAsync(id);
            if (acceptedRecyclable == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(
                _context.Employees.Select(e => new
                {
                    Id = e.Id,
                    FullName = e.Surname + " " + e.Name + " " + e.Patronymic
                }),
                "Id", "FullName", acceptedRecyclable.EmployeeId);
            ViewData["RecyclableTypeId"] = new SelectList(_context.RecyclableTypes, "Id", "Name", acceptedRecyclable.RecyclableTypeId);
            ViewData["StorageId"] = new SelectList(_context.Storages, "Id", "Name", acceptedRecyclable.StorageId);
            return View(acceptedRecyclable);
        }

        // POST: AcceptedRecyclables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RecyclableTypeId,EmployeeId,StorageId,Quantity,Date")] AcceptedRecyclable acceptedRecyclable)
        {
            if (id != acceptedRecyclable.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(acceptedRecyclable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcceptedRecyclableExists(acceptedRecyclable.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(
                _context.Employees.Select(e => new 
                { 
                    Id = e.Id,
                    FullName = e.Surname + " " + e.Name + " " + e.Patronymic
                }), 
                "Id", "FullName", acceptedRecyclable.EmployeeId);
            ViewData["RecyclableTypeId"] = new SelectList(_context.RecyclableTypes, "Id", "Name", acceptedRecyclable.RecyclableTypeId);
            ViewData["StorageId"] = new SelectList(_context.Storages, "Id", "Name", acceptedRecyclable.StorageId);
            return View(acceptedRecyclable);
        }

        // GET: AcceptedRecyclables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AcceptedRecyclables == null)
            {
                return NotFound();
            }

            var acceptedRecyclable = await _context.AcceptedRecyclables
                .Include(a => a.Employee)
                .Include(a => a.RecyclableType)
                .Include(a => a.Storage)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (acceptedRecyclable == null)
            {
                return NotFound();
            }

            return View(acceptedRecyclable);
        }

        // POST: AcceptedRecyclables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AcceptedRecyclables == null)
            {
                return Problem("Entity set 'RecPointContext.AcceptedRecyclables'  is null.");
            }
            var acceptedRecyclable = await _context.AcceptedRecyclables.FindAsync(id);
            if (acceptedRecyclable != null)
            {
                _context.AcceptedRecyclables.Remove(acceptedRecyclable);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcceptedRecyclableExists(int id)
        {
          return _context.AcceptedRecyclables.Any(e => e.Id == id);
        }
        private IQueryable<AcceptedRecyclable> Search(IQueryable<AcceptedRecyclable> acceptedRecyclables,
            SortStateAcceptedRec sortOrder, string searchRecTypeAccRec, string searchStorageAccRec)
        {
            ViewData["searchRecTypeAccRec"] = searchRecTypeAccRec;
            ViewData["searchStorageAccRec"] = searchStorageAccRec;
            acceptedRecyclables = acceptedRecyclables.Where(e => e.RecyclableType.Name.Contains(searchRecTypeAccRec ?? "")
            & e.Storage.Name.Contains(searchStorageAccRec ?? ""));

            ViewData["Employee"] = sortOrder == SortStateAcceptedRec.EmployeeAsc ? SortStateAcceptedRec.EmployeeDesc : SortStateAcceptedRec.EmployeeAsc;
            ViewData["Storage"] = sortOrder == SortStateAcceptedRec.StorageAsc ? SortStateAcceptedRec.StorageDesc : SortStateAcceptedRec.StorageAsc;
            ViewData["Date"] = sortOrder == SortStateAcceptedRec.DateAsc ? SortStateAcceptedRec.DateDesc : SortStateAcceptedRec.DateAsc;
            ViewData["Quantity"] = sortOrder == SortStateAcceptedRec.QuantityAsc ? SortStateAcceptedRec.QuantityDesc : SortStateAcceptedRec.QuantityAsc;
            ViewData["RecyclableType"] = sortOrder == SortStateAcceptedRec.RecyclableTypeAsc ? SortStateAcceptedRec.RecyclableTypeDesc : SortStateAcceptedRec.RecyclableTypeAsc;

            acceptedRecyclables = sortOrder switch
            {
                SortStateAcceptedRec.EmployeeAsc => acceptedRecyclables
                .OrderBy(ar => ar.Employee.Surname + " " + ar.Employee.Name + " " + ar.Employee.Patronymic),
                SortStateAcceptedRec.EmployeeDesc => acceptedRecyclables
                .OrderByDescending(ar => ar.Employee.Surname + " " + ar.Employee.Name + " " + ar.Employee.Patronymic),
                SortStateAcceptedRec.StorageAsc => acceptedRecyclables.OrderBy(ar => ar.Storage.Name),
                SortStateAcceptedRec.StorageDesc => acceptedRecyclables.OrderByDescending(ar => ar.Storage.Name),
                SortStateAcceptedRec.DateAsc => acceptedRecyclables.OrderBy(ar => ar.Date),
                SortStateAcceptedRec.DateDesc => acceptedRecyclables.OrderByDescending(ar => ar.Date),
                SortStateAcceptedRec.QuantityAsc => acceptedRecyclables.OrderBy(e => e.Quantity),
                SortStateAcceptedRec.QuantityDesc => acceptedRecyclables.OrderByDescending(e => e.Quantity),
                SortStateAcceptedRec.RecyclableTypeAsc => acceptedRecyclables.OrderBy(e => e.RecyclableType.Name),
                SortStateAcceptedRec.RecyclableTypeDesc => acceptedRecyclables.OrderByDescending(e => e.RecyclableType.Name),
                SortStateAcceptedRec.No => acceptedRecyclables.OrderBy(e => e.Id),
                _ => acceptedRecyclables.OrderBy(e => e.Id)
            };

            return acceptedRecyclables;
        }
        private void SaveValuesInSession(SortStateAcceptedRec sortOrder, int page, string searchRecTypeAccRec, string searchStorageAccRec)
        {
            HttpContext.Session.Remove(_currentSortOrder);
            HttpContext.Session.Remove(_currentPage);
            HttpContext.Session.Remove(_currentFilterRecType);
            HttpContext.Session.Remove(_currentFilterStorage);
            HttpContext.Session.SetString(_currentSortOrder, sortOrder.ToString());
            HttpContext.Session.SetString(_currentPage, page.ToString());
            HttpContext.Session.SetString(_currentFilterRecType, searchRecTypeAccRec);
            HttpContext.Session.SetString(_currentFilterStorage, searchStorageAccRec);
        }
        private SortStateAcceptedRec GetSortStateFromSessionOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentSortOrder) ?
                (SortStateAcceptedRec)Enum.Parse(typeof(SortStateAcceptedRec),
                HttpContext.Session.GetString(_currentSortOrder)) : SortStateAcceptedRec.No;
        }
        private int GetCurrentPageFromSessionOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentPage) ?
                HttpContext.Session.Get<int>(_currentPage) : 1;
        }
        private string GetCurrentFilterRecTypeOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentFilterRecType) ?
                HttpContext.Session.GetString(_currentFilterRecType) : string.Empty;
        }

        private string GetCurrentFilterStorageOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentFilterStorage) ?
                HttpContext.Session.GetString(_currentFilterStorage) : string.Empty;
        }
    }
}
