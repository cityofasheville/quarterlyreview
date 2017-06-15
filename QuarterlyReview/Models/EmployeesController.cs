using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace QuarterlyReview.Models
{
    public class EmployeesController : Controller
    {
        private readonly QuarterlyReviewsContext _context;

        public EmployeesController(QuarterlyReviewsContext context)
        {
            _context = context;    
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var quarterlyReviewsContext = _context.Employees.Include(e => e.Div);
            return View(await quarterlyReviewsContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employees = await _context.Employees
                .Include(e => e.Div)
                .SingleOrDefaultAsync(m => m.EmpId == id);
            if (employees == null)
            {
                return NotFound();
            }

            return View(employees);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["DivId"] = new SelectList(_context.DeptToDivMapping, "DivId", "DivId");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpId,Active,Employee,EmpEmail,Position,DeptId,Department,DivId,Division,SupId,Supervisor,SupEmail")] Employees employees)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employees);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["DivId"] = new SelectList(_context.DeptToDivMapping, "DivId", "DivId", employees.DivId);
            return View(employees);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employees = await _context.Employees.SingleOrDefaultAsync(m => m.EmpId == id);
            if (employees == null)
            {
                return NotFound();
            }
            ViewData["DivId"] = new SelectList(_context.DeptToDivMapping, "DivId", "DivId", employees.DivId);
            return View(employees);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmpId,Active,Employee,EmpEmail,Position,DeptId,Department,DivId,Division,SupId,Supervisor,SupEmail")] Employees employees)
        {
            if (id != employees.EmpId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employees);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeesExists(employees.EmpId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["DivId"] = new SelectList(_context.DeptToDivMapping, "DivId", "DivId", employees.DivId);
            return View(employees);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employees = await _context.Employees
                .Include(e => e.Div)
                .SingleOrDefaultAsync(m => m.EmpId == id);
            if (employees == null)
            {
                return NotFound();
            }

            return View(employees);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employees = await _context.Employees.SingleOrDefaultAsync(m => m.EmpId == id);
            _context.Employees.Remove(employees);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool EmployeesExists(int id)
        {
            return _context.Employees.Any(e => e.EmpId == id);
        }
    }
}
