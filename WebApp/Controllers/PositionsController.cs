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
    public class PositionsController : Controller
    {
        private readonly RecPointContext _context;
        private int _pageSize = 20;
        private string _currentPage = "pagePositions";
        private string _currentSortOrder = "sortOrderPositions";
        private string _currentFilterPosition = "searchPositionPositions";

        public PositionsController(RecPointContext context)
        {
            _context = context;
        }

        // GET: Positions
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 294)]
        public IActionResult Index(SortStatePosition? sortOrder, string searchPosition, int? page, bool resetFilter = false)
        {
            IQueryable<Position> positions = _context.Positions;
            sortOrder ??= GetSortStateFromSessionOrSetDefault();
            page ??= GetCurrentPageFromSessionOrSetDefault();
            if (resetFilter)
            {
                HttpContext.Session.Remove(_currentFilterPosition);
            }
            searchPosition ??= GetCurrentFilterPositionOrSetDefault();
            positions = Search(positions, (SortStatePosition)sortOrder, searchPosition);
            var count = positions.Count();
            PageViewModel pageViewModel = new PageViewModel(count, (int)page, _pageSize);
            positions = positions.Skip(((int)pageViewModel.PageNumber - 1) * _pageSize).Take(_pageSize);
            SaveValuesInSession((SortStatePosition)sortOrder, (int)page, searchPosition);
            IndexViewModel<Position> positionsView = new IndexViewModel<Position>()
            {
                Items = positions,
                PageViewModel = pageViewModel
            };
            return View(positionsView);
        }

        // GET: Positions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Positions == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        // GET: Positions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Positions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Position position)
        {
            if (ModelState.IsValid)
            {
                _context.Add(position);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(position);
        }

        // GET: Positions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Positions == null)
            {
                return NotFound();
            }

            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }
            return View(position);
        }

        // POST: Positions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Position position)
        {
            if (id != position.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(position);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PositionExists(position.Id))
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
            return View(position);
        }

        // GET: Positions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Positions == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Positions == null)
            {
                return Problem("Entity set 'RecPointContext.Positions'  is null.");
            }
            var position = await _context.Positions.FindAsync(id);
            if (position != null)
            {
                _context.Positions.Remove(position);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PositionExists(int id)
        {
            return _context.Positions.Any(e => e.Id == id);
        }
        private IQueryable<Position> Search(IQueryable<Position> positions,
            SortStatePosition sortOrder, string searchPosition)
        {
            ViewData["searchPosition"] = searchPosition;
            positions = positions.Where(p => p.Name.Contains(searchPosition ?? ""));

            ViewData["Name"] = sortOrder == SortStatePosition.NameAsc ? SortStatePosition.NameDesc : SortStatePosition.NameAsc;

            positions = sortOrder switch
            {
                SortStatePosition.NameAsc => positions.OrderBy(p => p.Name),
                SortStatePosition.NameDesc => positions.OrderByDescending(p => p.Name),
                SortStatePosition.No => positions.OrderBy(p => p.Id),
                _ => positions.OrderBy(p => p.Id)
            };

            return positions;
        }
        private void SaveValuesInSession(SortStatePosition sortOrder, int page, string searchPosition)
        {
            HttpContext.Session.Remove(_currentSortOrder);
            HttpContext.Session.Remove(_currentPage);
            HttpContext.Session.Remove(_currentFilterPosition);
            HttpContext.Session.SetString(_currentSortOrder, sortOrder.ToString());
            HttpContext.Session.SetString(_currentPage, page.ToString());
            HttpContext.Session.SetString(_currentFilterPosition, searchPosition);
        }
        private SortStatePosition GetSortStateFromSessionOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentSortOrder) ?
                (SortStatePosition)Enum.Parse(typeof(SortStatePosition),
                HttpContext.Session.GetString(_currentSortOrder)) : SortStatePosition.No;
        }
        private int GetCurrentPageFromSessionOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentPage) ?
                HttpContext.Session.Get<int>(_currentPage) : 1;
        }
        private string GetCurrentFilterPositionOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentFilterPosition) ?
                HttpContext.Session.GetString(_currentFilterPosition) : string.Empty;
        }
    }
}
