using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.SortStates;
using WebApp.ViewModels;
using WebApp.Infrastructure;
using System.Data;

namespace WebApp.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly RecPointContext _context;
        private int _pageSize = 50;
        private string _currentPage = "pageEmployees";
        private string _currentSortOrder = "sortOrderEmployees";
        private string _currentFilterSurname = "searchSurnameEmployees";
        private string _currentFilterExperience = "searchExpEmployees";

        public EmployeesController(RecPointContext context)
        {
            _context = context;
        }

        // GET: Employees
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 294)]
        public IActionResult Index(SortStateEmployee? sortOrder, string searchSurname, int? searchExperience, int? page, bool resetFilter = false)
        {
            IQueryable<Employee> employees = _context.Employees.Include(e => e.Position);
            sortOrder ??= GetSortStateFromSessionOrSetDefault();
            page ??= GetCurrentPageFromSessionOrSetDefault();
            if(resetFilter)
            {
                HttpContext.Session.Remove(_currentFilterSurname);
                HttpContext.Session.Remove(_currentFilterExperience);
            }
            searchSurname ??= GetCurrentFilterSurnameOrSetDefault();
            searchExperience ??= GetCurrentFilterExperienceOrSetDefault();
            employees = Search(employees, (SortStateEmployee)sortOrder, searchSurname, searchExperience);
            var count = employees.Count();
            PageViewModel pageViewModel = new PageViewModel(count, (int)page, _pageSize);
            employees = employees.Skip(((int)pageViewModel.PageNumber - 1) * _pageSize).Take(_pageSize);
            SaveValuesInSession((SortStateEmployee)sortOrder, (int)page, searchSurname, searchExperience);
            IndexViewModel<Employee> employeesView = new IndexViewModel<Employee>()
            {
                Items = employees,
                PageViewModel = pageViewModel
            };
            return View(employeesView);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Position)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Name");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PositionId,Name,Surname,Patronymic,Experience")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Name", employee.PositionId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Name", employee.PositionId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PositionId,Name,Surname,Patronymic,Experience")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Name", employee.PositionId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Position)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'RecPointContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
          return _context.Employees.Any(e => e.Id == id);
        }

        private IQueryable<Employee> Search(IQueryable<Employee> employees,
            SortStateEmployee sortOrder, string searchSurname, int? searchExperience)
        {
            ViewData["searchSurname"] = searchSurname;
            ViewData["searchExperience"] = searchExperience;
            employees = employees.Where(e => e.Surname.Contains(searchSurname ?? ""));
            if(searchExperience != null)
            {
                employees = employees.Where(e => e.Experience == searchExperience);
            }

            ViewData["Surname"] = sortOrder == SortStateEmployee.SurnameAsc ? SortStateEmployee.SurnameDesc : SortStateEmployee.SurnameAsc;
            ViewData["Name"] = sortOrder == SortStateEmployee.NameAsc ? SortStateEmployee.NameDesc : SortStateEmployee.NameAsc;
            ViewData["Patronymic"] = sortOrder == SortStateEmployee.PatronymicAsc ? SortStateEmployee.PatronymicDesc : SortStateEmployee.PatronymicAsc;
            ViewData["Experience"] = sortOrder == SortStateEmployee.ExperienceAsc ? SortStateEmployee.ExperienceDesc : SortStateEmployee.ExperienceAsc;
            ViewData["Position"] = sortOrder == SortStateEmployee.PositionAsc ? SortStateEmployee.PositionDesc : SortStateEmployee.PositionAsc;

            employees = sortOrder switch
            {
                SortStateEmployee.NameAsc => employees.OrderBy(e => e.Name),
                SortStateEmployee.NameDesc => employees.OrderByDescending(e => e.Name),
                SortStateEmployee.SurnameAsc => employees.OrderBy(e => e.Surname),
                SortStateEmployee.SurnameDesc => employees.OrderByDescending(e => e.Surname),
                SortStateEmployee.PatronymicAsc => employees.OrderBy(e => e.Patronymic),
                SortStateEmployee.PatronymicDesc => employees.OrderByDescending(e => e.Patronymic),
                SortStateEmployee.ExperienceAsc => employees.OrderBy(e => e.Experience),
                SortStateEmployee.ExperienceDesc => employees.OrderByDescending(e => e.Experience),
                SortStateEmployee.PositionAsc => employees.OrderBy(e => e.Position.Name),
                SortStateEmployee.PositionDesc => employees.OrderByDescending(e => e.Position.Name),
                SortStateEmployee.No => employees.OrderBy(e => e.Id),
                _ => employees.OrderBy(e => e.Id)
            };

            return employees;
        }
        private void SaveValuesInSession(SortStateEmployee sortOrder, int page, string searchSurname, int? searchExperience)
        {
            HttpContext.Session.Remove(_currentSortOrder);
            HttpContext.Session.Remove(_currentPage);
            HttpContext.Session.Remove(_currentFilterSurname);
            HttpContext.Session.Remove(_currentFilterExperience);
            HttpContext.Session.SetString(_currentSortOrder, sortOrder.ToString());
            HttpContext.Session.Set<int>(_currentPage, page);
            HttpContext.Session.SetString(_currentFilterSurname, searchSurname);
            HttpContext.Session.Set<int?>(_currentFilterExperience, searchExperience);
        }
        private SortStateEmployee GetSortStateFromSessionOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentSortOrder) ? 
                (SortStateEmployee)Enum.Parse(typeof(SortStateEmployee), 
                HttpContext.Session.GetString(_currentSortOrder)) : SortStateEmployee.No;
        }
        private int GetCurrentPageFromSessionOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentPage) ?
                HttpContext.Session.Get<int>(_currentPage) : 1;
        }
        private string GetCurrentFilterSurnameOrSetDefault()
        {
            return HttpContext.Session.Keys.Contains(_currentFilterSurname) ?
                HttpContext.Session.GetString(_currentFilterSurname) : string.Empty;
        }

        private int? GetCurrentFilterExperienceOrSetDefault()
        {
            if (HttpContext.Session.Keys.Contains(_currentFilterExperience))
            {
                try
                {
                    HttpContext.Session.Get<int>(_currentFilterExperience);
                }
                catch
                {
                    return null;
                }
                return HttpContext.Session.Get<int>(_currentFilterExperience);
            }
            return null;
        }
    }
}