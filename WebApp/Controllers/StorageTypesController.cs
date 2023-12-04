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
    public class StorageTypesController : Controller
    {
        private readonly RecPointContext _context;
        private int _pageSize = 20;
        private string _currentPage = "pageStorageTypes";
        private string _currentSortOrder = "sortOrderStorageTypes";
        private string _currentFilterStorageType = "searchStorageTypeStorageTypes";

        public StorageTypesController(RecPointContext context)
        {
            _context = context;
        }

        // GET: StorageTypes

        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 294)]
        public IActionResult Index(SortStateStorageType? sortOrder, string searchStorageType, int? page, bool resetFilter = false)
        {
            IQueryable<StorageType> storageTypes = _context.StorageTypes;
            sortOrder ??= GetSortStateFromSessionOrSetDefault();
            page ??= GetCurrentPageFromSessionOrSetDefault();
            if (resetFilter)
            {
                HttpContext.Session.Remove(_currentFilterStorageType);
            }
            searchStorageType ??= GetCurrentFilterStorageTypeOrSetDefault();
            storageTypes = Search(storageTypes, (SortStateStorageType)sortOrder, searchStorageType);
            var count = storageTypes.Count();
            PageViewModel pageViewModel = new PageViewModel(count, (int)page, _pageSize);
            storageTypes = storageTypes.Skip(((int)pageViewModel.PageNumber - 1) * _pageSize).Take(_pageSize);
            SaveValuesInSession((SortStateStorageType)sortOrder, (int)page, searchStorageType);
            IndexViewModel<StorageType> StorageTypesView = new IndexViewModel<StorageType>()
            {
                Items = storageTypes,
                PageViewModel = pageViewModel
            };
            return View(StorageTypesView);
        }

        // GET: StorageTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StorageTypes == null)
            {
                return NotFound();
            }

            var storageType = await _context.StorageTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storageType == null)
            {
                return NotFound();
            }

            return View(storageType);
        }

        // GET: StorageTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StorageTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Temperature,Humidity,Requirement,Equipment")] StorageType storageType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storageType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(storageType);
        }

        // GET: StorageTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StorageTypes == null)
            {
                return NotFound();
            }

            var storageType = await _context.StorageTypes.FindAsync(id);
            if (storageType == null)
            {
                return NotFound();
            }
            return View(storageType);
        }

        // POST: StorageTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Temperature,Humidity,Requirement,Equipment")] StorageType storageType)
        {
            if (id != storageType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storageType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StorageTypeExists(storageType.Id))
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
            return View(storageType);
        }

        // GET: StorageTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StorageTypes == null)
            {
                return NotFound();
            }

            var storageType = await _context.StorageTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storageType == null)
            {
                return NotFound();
            }

            return View(storageType);
        }

        // POST: StorageTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StorageTypes == null)
            {
                return Problem("Entity set 'RecPointContext.StorageTypes'  is null.");
            }
            var storageType = await _context.StorageTypes.FindAsync(id);
            if (storageType != null)
            {
                _context.StorageTypes.Remove(storageType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StorageTypeExists(int id)
        {
          return _context.StorageTypes.Any(e => e.Id == id);
        }
        private IQueryable<StorageType> Search(IQueryable<StorageType> storageTypes,
            SortStateStorageType sortOrder, string searchStorageType)
        {
            ViewData["searchStorageType"] = searchStorageType;
            storageTypes = storageTypes.Where(p => p.Name.Contains(searchStorageType ?? ""));

            ViewData["Name"] = sortOrder == SortStateStorageType.NameAsc ? SortStateStorageType.NameDesc : SortStateStorageType.NameAsc;
            ViewData["Temperature"] = sortOrder == SortStateStorageType.TemperatureAsc ? SortStateStorageType.TemperatureDesc : SortStateStorageType.TemperatureAsc;
            ViewData["Humidity"] = sortOrder == SortStateStorageType.HumidityAsc ? SortStateStorageType.HumidityDesc : SortStateStorageType.HumidityAsc;

            storageTypes = sortOrder switch
            {
                SortStateStorageType.NameAsc => storageTypes.OrderBy(st => st.Name),
                SortStateStorageType.NameDesc => storageTypes.OrderByDescending(st => st.Name),
                SortStateStorageType.HumidityAsc => storageTypes.OrderBy(st => st.Humidity),
                SortStateStorageType.HumidityDesc => storageTypes.OrderByDescending(st => st.Humidity),
                SortStateStorageType.TemperatureAsc => storageTypes.OrderBy(st => st.Temperature),
                SortStateStorageType.TemperatureDesc => storageTypes.OrderByDescending(st => st.Temperature),
                SortStateStorageType.No => storageTypes.OrderBy(st => st.Id),
                _ => storageTypes.OrderBy(st => st.Id)
            };

            return storageTypes;
        }
        private void SaveValuesInSession(SortStateStorageType sortOrder, int page, string searchStorageType)
        {
            HttpContext.Session.Remove(_currentSortOrder);
            HttpContext.Session.Remove(_currentPage);
            HttpContext.Session.Remove(_currentFilterStorageType);
            HttpContext.Session.Set(_currentSortOrder, sortOrder);
            HttpContext.Session.SetString(_currentPage, page.ToString());
            HttpContext.Session.SetString(_currentFilterStorageType, searchStorageType);
        }
        private SortStateStorageType GetSortStateFromSessionOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentSortOrder) ?
                HttpContext.Session.Get<SortStateStorageType>(_currentSortOrder) : SortStateStorageType.No;
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
    }
}
