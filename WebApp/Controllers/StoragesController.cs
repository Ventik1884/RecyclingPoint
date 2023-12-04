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
    public class StoragesController : Controller
    {

        private readonly RecPointContext _context;
        private int _pageSize = 50;
        private string _currentPage = "pageStorages";
        private string _currentSortOrder = "sortOrderStorages";
        private string _currentFilterStorageType = "searchStorageTypeStorages";
        private string _currentFilterName = "searchNameStorages";

        public StoragesController(RecPointContext context)
        {
            _context = context;
        }

        // GET: Storages
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 294)]
        public IActionResult Index(SortStateStorage? sortOrder, string searchStorageType, string searchNameStorages, int? page, bool resetFilter = false)
        {
            IQueryable<Storage> storages = _context.Storages.Include(s => s.StorageType);
            sortOrder ??= GetSortStateFromSessionOrSetDefault();
            page ??= GetCurrentPageFromSessionOrSetDefault();
            if (resetFilter)
            {
                HttpContext.Session.Remove(_currentFilterStorageType);
                HttpContext.Session.Remove(_currentFilterName);
            }
            searchStorageType ??= GetCurrentFilterStorageTypeOrSetDefault();
            searchNameStorages ??= GetCurrentFilterNameOrSetDefault();
            storages = Search(storages, (SortStateStorage)sortOrder, searchStorageType, searchNameStorages);
            var count = storages.Count();
            PageViewModel pageViewModel = new PageViewModel(count, (int)page, _pageSize);
            storages = storages.Skip(((int)pageViewModel.PageNumber - 1) * _pageSize).Take(_pageSize);
            SaveValuesInSession((SortStateStorage)sortOrder, (int)page, searchStorageType, searchNameStorages);
            IndexViewModel<Storage> storagesView = new IndexViewModel<Storage>()
            {
                Items = storages,
                PageViewModel = pageViewModel
            };
            return View(storagesView);
        }

        // GET: Storages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Storages == null)
            {
                return NotFound();
            }

            var storage = await _context.Storages
                .Include(s => s.StorageType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storage == null)
            {
                return NotFound();
            }

            return View(storage);
        }

        // GET: Storages/Create
        public IActionResult Create()
        {
            ViewData["StorageTypeId"] = new SelectList(_context.StorageTypes, "Id", "Name");
            return View();
        }

        // POST: Storages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StorageTypeId,Name,Number,Square,Capacity,Occupancy,Depreciation,CheckDate")] Storage storage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StorageTypeId"] = new SelectList(_context.StorageTypes, "Id", "Name", storage.StorageTypeId);
            return View(storage);
        }

        // GET: Storages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Storages == null)
            {
                return NotFound();
            }

            var storage = await _context.Storages.FindAsync(id);
            if (storage == null)
            {
                return NotFound();
            }
            ViewData["StorageTypeId"] = new SelectList(_context.StorageTypes, "Id", "Name", storage.StorageTypeId);
            return View(storage);
        }

        // POST: Storages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StorageTypeId,Name,Number,Square,Capacity,Occupancy,Depreciation,CheckDate")] Storage storage)
        {
            if (id != storage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StorageExists(storage.Id))
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
            ViewData["StorageTypeId"] = new SelectList(_context.StorageTypes, "Id", "Name", storage.StorageTypeId);
            return View(storage);
        }

        // GET: Storages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Storages == null)
            {
                return NotFound();
            }

            var storage = await _context.Storages
                .Include(s => s.StorageType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storage == null)
            {
                return NotFound();
            }

            return View(storage);
        }

        // POST: Storages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Storages == null)
            {
                return Problem("Entity set 'RecPointContext.Storages'  is null.");
            }
            var storage = await _context.Storages.FindAsync(id);
            if (storage != null)
            {
                _context.Storages.Remove(storage);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StorageExists(int id)
        {
          return _context.Storages.Any(e => e.Id == id);
        }
        private IQueryable<Storage> Search(IQueryable<Storage> storages,
            SortStateStorage sortOrder, string searchStorageType, string searchNameStorages)
        {
            ViewData["searchNameStorages"] = searchNameStorages;
            ViewData["searchStorageType"] = searchStorageType;
            storages = storages.Where(e => e.Name.Contains(searchNameStorages ?? "")
            & e.StorageType.Name.Contains(searchStorageType ?? ""));

            ViewData["Name"] = sortOrder == SortStateStorage.NameAsc ? SortStateStorage.NameDesc : SortStateStorage.NameAsc;
            ViewData["Capacity"] = sortOrder == SortStateStorage.CapacityAsc ? SortStateStorage.CapacityDesc : SortStateStorage.CapacityAsc;
            ViewData["CheckDate"] = sortOrder == SortStateStorage.CheckDateAsc ? SortStateStorage.CheckDateDesc : SortStateStorage.CheckDateAsc;
            ViewData["Depreciation"] = sortOrder == SortStateStorage.DepreciationAsc ? SortStateStorage.DepreciationDesc : SortStateStorage.DepreciationAsc;
            ViewData["Number"] = sortOrder == SortStateStorage.NumberAsc ? SortStateStorage.NumberDesc : SortStateStorage.NumberAsc;
            ViewData["Square"] = sortOrder == SortStateStorage.SquareAsc ? SortStateStorage.SquareDesc : SortStateStorage.SquareAsc;
            ViewData["Occupancy"] = sortOrder == SortStateStorage.OccupancyAsc ? SortStateStorage.OccupancyDesc : SortStateStorage.OccupancyAsc;
            ViewData["StorageType"] = sortOrder == SortStateStorage.StorageTypeAsc ? SortStateStorage.StorageTypeDesc : SortStateStorage.StorageTypeAsc;

            storages = sortOrder switch
            {
                SortStateStorage.NameAsc => storages.OrderBy(s => s.Name),
                SortStateStorage.NameDesc => storages.OrderByDescending(s => s.Name),
                SortStateStorage.CapacityAsc => storages.OrderBy(s => s.Capacity),
                SortStateStorage.CapacityDesc => storages.OrderByDescending(s => s.Capacity),
                SortStateStorage.CheckDateAsc => storages.OrderBy(s => s.CheckDate),
                SortStateStorage.CheckDateDesc => storages.OrderByDescending(s => s.CheckDate),
                SortStateStorage.DepreciationAsc => storages.OrderBy(s => s.Depreciation),
                SortStateStorage.DepreciationDesc => storages.OrderByDescending(s => s.Depreciation),
                SortStateStorage.NumberAsc => storages.OrderBy(s => s.Number),
                SortStateStorage.NumberDesc => storages.OrderByDescending(s => s.Number),
                SortStateStorage.StorageTypeAsc => storages.OrderBy(s => s.StorageType),
                SortStateStorage.StorageTypeDesc => storages.OrderByDescending(s => s.StorageType),
                SortStateStorage.OccupancyAsc => storages.OrderBy(s => s.Occupancy),
                SortStateStorage.OccupancyDesc => storages.OrderByDescending(s => s.Occupancy),
                SortStateStorage.SquareAsc => storages.OrderBy(s => s.Square),
                SortStateStorage.SquareDesc => storages.OrderByDescending(s => s.Square),
                SortStateStorage.No => storages.OrderBy(s => s.Id),
                _ => storages.OrderBy(s => s.Id)
            };

            return storages;
        }
        private void SaveValuesInSession(SortStateStorage sortOrder, int page, string searchStorageType, string searchNameStorages)
        {
            HttpContext.Session.Remove(_currentSortOrder);
            HttpContext.Session.Remove(_currentPage);
            HttpContext.Session.Remove(_currentFilterName);
            HttpContext.Session.Remove(_currentFilterStorageType);
            HttpContext.Session.SetString(_currentSortOrder, sortOrder.ToString());
            HttpContext.Session.SetString(_currentPage, page.ToString());
            HttpContext.Session.SetString(_currentFilterName, searchNameStorages);
            HttpContext.Session.SetString(_currentFilterStorageType, searchStorageType);
        }
        private SortStateStorage GetSortStateFromSessionOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentSortOrder) ?
                (SortStateStorage)Enum.Parse(typeof(SortStateStorage),
                HttpContext.Session.GetString(_currentSortOrder)) : SortStateStorage.No;
        }
        private int GetCurrentPageFromSessionOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentPage) ?
                HttpContext.Session.Get<int>(_currentPage) : 1;
        }
        private string GetCurrentFilterStorageTypeOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentFilterStorageType) ?
                HttpContext.Session.GetString(_currentFilterStorageType) : string.Empty;
        }

        private string GetCurrentFilterNameOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentFilterName) ?
                HttpContext.Session.GetString(_currentFilterName) : string.Empty;
        }
    }
}
