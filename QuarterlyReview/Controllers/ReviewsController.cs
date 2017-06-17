using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuarterlyReview.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace QuarterlyReview.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly QuarterlyReviewsContext _context;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager = null;

        public ReviewsController(QuarterlyReviewsContext context,
            ILoggerFactory loggerFactory,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<ReviewsController>();
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var quarterlyReviewsContext = _context.Reviews.Include(r => r.Div).Include(r => r.Emp).Include(r => r.Rt).Include(r => r.StatusNavigation).Include(r => r.Sup);
            return View(await quarterlyReviewsContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews
                .Include(r => r.Div)
                .Include(r => r.Emp)
                .Include(r => r.Rt)
                .Include(r => r.StatusNavigation)
                .Include(r => r.Sup)
                .SingleOrDefaultAsync(m => m.RId == id);
            if (reviews == null)
            {
                return NotFound();
            }

            return View(reviews);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // The id parameter is the ID of the employee whose current review we are editing
            if (id == null)
            {
                return NotFound();
            }
            string getEmployee = "EXECUTE dbo.avp_Get_Employee @UserEmpId";
            var empId = new SqlParameter("@UserEmpId", id);
            var employee = await _context.Employees.FromSql(getEmployee, empId)
                .SingleOrDefaultAsync<Employees>();
            if (employee.CurrentReview == null)
            {
                // We need to create a new review for this employee
                DateTime startDate = DateTime.Now;
                DateTime endDate = startDate.AddDays(90);

                var command = _context.Database.GetDbConnection().CreateCommand();
                // Call the avp_New_Review stored procedure and get the resulting ID
                command.CommandText = "DECLARE	@return_value int " +
                    "EXEC	@return_value = [dbo].[avp_New_Review] " +
                    String.Format("@EmpID = {0}, @SupID = {1}, @RT_ID = 2, @PeriodStart = \"{2}\", @PeriodEnd = \"{3}\" ",
                                  employee.EmpId, employee.SupId,
                                  startDate.ToString("yyyy-MM-dd"),
                                  endDate.ToString("yyyy-MM-dd")) +
                    "SELECT  'ReturnValue' = @return_value";

                _context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    if (result.HasRows)
                    {
                        //while (result.Read())
                        //{
                        //    employee.CurrentReview = result.GetInt32(0);
                        //}
                        if (result.Read()) employee.CurrentReview = result.GetInt32(0);
                    }
                }
                return NotFound();
            }
            else
            {
                _logger.LogInformation(1, "HaveCurrent {currentid}.", employee.CurrentReview);
            }
            ViewData["Employee"] = employee.Employee;
            var reviews = await _context.Reviews.SingleOrDefaultAsync(m => m.RId == id);
            if (reviews == null)
            {
                return NotFound();
            }
            ViewData["DivId"] = new SelectList(_context.DeptToDivMapping, "DivId", "DivId", reviews.DivId);
            ViewData["EmpId"] = new SelectList(_context.Employees, "EmpId", "Active", reviews.EmpId);
            ViewData["RtId"] = new SelectList(_context.ReviewTemplate, "RtId", "Description", reviews.RtId);
            ViewData["Status"] = new SelectList(_context.ReviewStatusList, "ReviewStatus", "ReviewStatus", reviews.Status);
            ViewData["SupId"] = new SelectList(_context.Employees, "EmpId", "Active", reviews.SupId);
            return View(reviews);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {

            if (HttpContext.Request.Form["rid"] == "1b2")
            {
                // xx
            }

            return RedirectToRoute(new
            {
                controller = "Home",
                action = "Index"
            });
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews
                .Include(r => r.Div)
                .Include(r => r.Emp)
                .Include(r => r.Rt)
                .Include(r => r.StatusNavigation)
                .Include(r => r.Sup)
                .SingleOrDefaultAsync(m => m.RId == id);
            if (reviews == null)
            {
                return NotFound();
            }

            return View(reviews);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reviews = await _context.Reviews.SingleOrDefaultAsync(m => m.RId == id);
            _context.Reviews.Remove(reviews);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ReviewsExists(int id)
        {
            return _context.Reviews.Any(e => e.RId == id);
        }
    }
}
