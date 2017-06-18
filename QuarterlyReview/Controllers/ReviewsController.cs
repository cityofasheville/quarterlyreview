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

                var createReviewCmd = _context.Database.GetDbConnection().CreateCommand();
                // Call the avp_New_Review stored procedure and get the resulting ID
                createReviewCmd.CommandText = "DECLARE	@return_value int " +
                    "EXEC	@return_value = [dbo].[avp_New_Review] " +
                    String.Format("@EmpID = {0}, @SupID = {1}, @RT_ID = 2, @PeriodStart = \"{2}\", @PeriodEnd = \"{3}\" ",
                                  employee.EmpId, employee.SupId,
                                  startDate.ToString("yyyy-MM-dd"),
                                  endDate.ToString("yyyy-MM-dd")) +
                    "SELECT  'ReturnValue' = @return_value";

                _context.Database.OpenConnection();
                using (var result = createReviewCmd.ExecuteReader())
                {
                    if (result.HasRows)
                    {
                        if (result.Read()) employee.CurrentReview = result.GetInt32(0);
                    }
                }
                return NotFound();
            }

            // Now read the review in
            DisplayReview rev = null;

            var getReviewCmd = _context.Database.GetDbConnection().CreateCommand();

            getReviewCmd.CommandText = "EXEC [dbo].[avp_Get_A_Review] " +
                String.Format("@ReviewID = {0}", employee.CurrentReview);

            _context.Database.OpenConnection();
            using (var result = getReviewCmd.ExecuteReader())
            {
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        if (rev == null)
                        {
                            rev = new DisplayReview(
                                result.GetInt32(1), // R_ID
                                result.GetInt32(4), // ReviewerID
                                result.GetInt32(5), // EmpID
                                result.GetString(7), // Position
                                result.GetDateTime(8), // PeriodStart
                                result.GetDateTime(9), // PeriodEnd
                                result.GetString(19), // Reviewer
                                result.GetString(22) // Employee
                                );
                        }
                        string qtext = result.IsDBNull(13) ? null : result.GetString(13);
                        string atext = result.IsDBNull(15) ? null : result.GetString(15);
                        DisplayQuestion q = new DisplayQuestion(
                            result.GetInt32(14), // Q_ID
                            result.GetString(12), // QT_Type
                            qtext, // QT_Question
                            atext // Answer
                            );
                        rev.questions.Add(q);
                    }
                }
            }
            return View(rev);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            Microsoft.AspNetCore.Http.IFormCollection form = Request.Form;
            if (form.ContainsKey("review-id"))
            {
                int r_id = Int32.Parse(Request.Form["review-id"]);
                Reviews review = _context.Reviews.Find(r_id);
                if (review == null) return NotFound();
                if (form.ContainsKey("startDate"))
                {
                    review.PeriodStart = Convert.ToDateTime(form["startDate"]);
                }
                if (form.ContainsKey("endDate"))
                {
                    review.PeriodEnd = Convert.ToDateTime(form["endDate"]);
                }

                foreach (var key in form)
                {
                    if (key.Key.StartsWith("qanswer-"))
                    {
                        int q_id = Int32.Parse(key.Key.Split('-')[1]);
                        Questions question = _context.Questions.Find(q_id);
                        question.Answer = key.Value;
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return BadRequest();
            }
            return RedirectToRoute(new
            {
                controller = "Home",
                action = "Index"
            });
        }

        private bool ReviewsExists(int id)
        {
            return _context.Reviews.Any(e => e.RId == id);
        }
    }
}
