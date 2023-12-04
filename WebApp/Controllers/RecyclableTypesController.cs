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
    public class RecyclableTypesController : Controller
    {
        private readonly RecPointContext _context;
        private int _pageSize = 20;
        private string _currentPage = "pageRecyclableTypes";
        private string _currentSortOrder = "sortOrderRecyclableTypes";
        private string _currentFilterRecyclableType = "searchRecyclableTypeRecyclableTypes";

        public RecyclableTypesController(RecPointContext context)
        {
            _context = context;
        }

        // GET: RecyclableTypes

        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 294)]
        public IActionResult Index(SortStateRecyclableType? sortOrder, string searchRecyclableType, int? page, bool resetFilter = false)
        {
            IQueryable<RecyclableType> RecyclableTypes = _context.RecyclableTypes;
            sortOrder ??= GetSortStateFromSessionOrSetDefault();
            page ??= GetCurrentPageFromSessionOrSetDefault();
            if (resetFilter)
            {
                HttpContext.Session.Remove(_currentFilterRecyclableType);
            }
            searchRecyclableType ??= GetCurrentFilterRecyclableTypeOrSetDefault();
            RecyclableTypes = Search(RecyclableTypes, (SortStateRecyclableType)sortOrder, searchRecyclableType);
            var count = RecyclableTypes.Count();
            PageViewModel pageViewModel = new PageViewModel(count, (int)page, _pageSize);
            RecyclableTypes = RecyclableTypes.Skip(((int)pageViewModel.PageNumber - 1) * _pageSize).Take(_pageSize);
            SaveValuesInSession((SortStateRecyclableType)sortOrder, (int)page, searchRecyclableType);
            IndexViewModel<RecyclableType> RecyclableTypesView = new IndexViewModel<RecyclableType>()
            {
                Items = RecyclableTypes,
                PageViewModel = pageViewModel
            };
            return View(RecyclableTypesView);
        }

        // GET: RecyclableTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RecyclableTypes == null)
            {
                return NotFound();
            }

            var recyclableType = await _context.RecyclableTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recyclableType == null)
            {
                return NotFound();
            }

            return View(recyclableType);
        }

        // GET: RecyclableTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RecyclableTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Description,AcceptanceCondition,StorageCondition")] RecyclableType recyclableType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recyclableType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(recyclableType);
        }

        // GET: RecyclableTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RecyclableTypes == null)
            {
                return NotFound();
            }

            var recyclableType = await _context.RecyclableTypes.FindAsync(id);
            if (recyclableType == null)
            {
                return NotFound();
            }
            return View(recyclableType);
        }

        // POST: RecyclableTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,AcceptanceCondition,StorageCondition")] RecyclableType recyclableType)
        {
            if (id != recyclableType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recyclableType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecyclableTypeExists(recyclableType.Id))
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
            return View(recyclableType);
        }

        // GET: RecyclableTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RecyclableTypes == null)
            {
                return NotFound();
            }

            var recyclableType = await _context.RecyclableTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recyclableType == null)
            {
                return NotFound();
            }

            return View(recyclableType);
        }

        // POST: RecyclableTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RecyclableTypes == null)
            {
                return Problem("Entity set 'RecPointContext.RecyclableTypes'  is null.");
            }
            var recyclableType = await _context.RecyclableTypes.FindAsync(id);
            if (recyclableType != null)
            {
                _context.RecyclableTypes.Remove(recyclableType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecyclableTypeExists(int id)
        {
            return _context.RecyclableTypes.Any(e => e.Id == id);
        }
        private IQueryable<RecyclableType> Search(IQueryable<RecyclableType> recyclableTypes,
            SortStateRecyclableType sortOrder, string searchRecyclableType)
        {
            ViewData["searchRecyclableType"] = searchRecyclableType;
            recyclableTypes = recyclableTypes.Where(p => p.Name.Contains(searchRecyclableType ?? ""));

            ViewData["Name"] = sortOrder == SortStateRecyclableType.NameAsc ? SortStateRecyclableType.NameDesc : SortStateRecyclableType.NameAsc;
            ViewData["Price"] = sortOrder == SortStateRecyclableType.PriceAsc ? SortStateRecyclableType.PriceDesc : SortStateRecyclableType.PriceAsc;

            recyclableTypes = sortOrder switch
            {
                SortStateRecyclableType.NameAsc => recyclableTypes.OrderBy(rt => rt.Name),
                SortStateRecyclableType.NameDesc => recyclableTypes.OrderByDescending(rt => rt.Name),
                SortStateRecyclableType.PriceAsc => recyclableTypes.OrderBy(rt => rt.Price),
                SortStateRecyclableType.PriceDesc => recyclableTypes.OrderByDescending(rt => rt.Price),
                SortStateRecyclableType.No => recyclableTypes.OrderBy(rt => rt.Id),
                _ => recyclableTypes.OrderBy(rt => rt.Id)
            };

            return recyclableTypes;
        }
        private void SaveValuesInSession(SortStateRecyclableType sortOrder, int page, string searchRecyclableType)
        {
            HttpContext.Session.Remove(_currentSortOrder);
            HttpContext.Session.Remove(_currentPage);
            HttpContext.Session.Remove(_currentFilterRecyclableType);
            HttpContext.Session.SetString(_currentSortOrder, sortOrder.ToString());
            HttpContext.Session.SetString(_currentPage, page.ToString());
            HttpContext.Session.SetString(_currentFilterRecyclableType, searchRecyclableType);
        }
        private SortStateRecyclableType GetSortStateFromSessionOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentSortOrder) ?
                (SortStateRecyclableType)Enum.Parse(typeof(SortStateRecyclableType),
                HttpContext.Session.GetString(_currentSortOrder)) : SortStateRecyclableType.No;
        }
        private int GetCurrentPageFromSessionOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentPage) ?
                HttpContext.Session.Get<int>(_currentPage) : 1;
        }
        private string GetCurrentFilterRecyclableTypeOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentFilterRecyclableType) ?
                HttpContext.Session.GetString(_currentFilterRecyclableType) : string.Empty;
        }
    }
}
