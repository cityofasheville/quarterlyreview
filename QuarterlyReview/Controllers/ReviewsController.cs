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

        private Boolean viewAllowed (string userID, string targetID)
        {
            var mayViewCmd = _context.Database.GetDbConnection().CreateCommand();
            // Call the avp_New_Review stored procedure and get the resulting ID
            mayViewCmd.CommandText = "DECLARE	@May_View nchar(1) " +
                "EXEC [dbo].[avp_May_View_Emp] " +
                String.Format("@UserEmpID = {0}, @EmpID = {1},", userID, targetID) +
                "@May_View = @May_View OUTPUT " +
                "SELECT  @May_View as N'@May_View'";

            _context.Database.OpenConnection();
            Char retVal = 'N';
            using (var result = mayViewCmd.ExecuteReader())
            {
                if (result.HasRows)
                {
                    if (result.Read())
                    {
                        retVal = result.GetString(0)[0];
                    }
                }
            }
            return (retVal == 'Y') ? true : false;
        }

        // GET: Reviews
        public async Task<IActionResult> Index(string emp)
        {
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                DateTime startDate = DateTime.Now;
                DateTime endDate = startDate.AddDays(90);
                string getEmployee = "EXECUTE dbo.avp_Get_Employee @UserEmpId";
                string getMyEmployees = "EXECUTE dbo.avp_Get_My_Employees @UserEmpId";
                
                string targetID = (emp == null) ? user.EmployeeID : emp.Trim();

                // Redirect to their own page if they don't have permission to view
                if (targetID != user.EmployeeID)
                {
                    if (!viewAllowed(user.EmployeeID, targetID))
                    {
                        return RedirectToRoute(new
                        {
                            controller = "Reviews",
                            action = "Index"
                        });
                    }
                }

                var empId = new SqlParameter("@UserEmpId", targetID);

                var employee = await _context.Employees.FromSql(getEmployee, empId)
                    .SingleOrDefaultAsync<Employees>();
                var myEmployees = await _context.Employees.FromSql(getMyEmployees, empId)
                    .ToListAsync<Employees>();
                List<DisplayReviewSummary> myReviews = new List<DisplayReviewSummary>();

                // Read reviews of me

                var getMyReviewsCmd = _context.Database.GetDbConnection().CreateCommand();

                int mrid = employee.EmpId;
                // mrid = 3399;
                getMyReviewsCmd.CommandText = "EXEC [dbo].[avp_Reviews_of_Me] " +
                    String.Format("@UserEmpID = {0}", mrid);

                _context.Database.OpenConnection();
                using (var result = getMyReviewsCmd.ExecuteReader())
                {
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            DisplayReviewSummary rev = new DisplayReviewSummary(
                                result.GetInt32(0), // RT_ID
                                result.GetInt32(1), // R_ID
                                result.GetString(2), // RT_Name
                                result.GetString(3), // RT_Desc
                                result.GetString(4), // Status
                                result.GetDateTime(5), // Status_Date
                                result.GetInt32(6), // SupID
                                result.GetInt32(7), // EmpID
                                result.GetString(8), // Employee
                                result.GetString(9), // DivID
                                result.GetString(10), // Position
                                result.GetDateTime(11), // PeriodStart
                                result.GetDateTime(12) // PeriodEnd
                             );
                            myReviews.Add(rev);
                        }
                    }
                }

                // Done

                ViewData["EmployeeID"] = user.EmployeeID;
                ViewData["EmployeeID"] = user.EmployeeID;
                ViewData["Supervisor"] = employee.Supervisor;
                ViewData["Employees"] = myEmployees;
                ViewData["Employee"] = employee.Employee;
                ViewData["MyReviews"] = myReviews;
                return View(myEmployees);

            }
            return View(null);
        }

        // GET: Reviews/Review/5?emp=NNNN
        public async Task<IActionResult> Review(int? id, string emp)
        {
            _logger.LogInformation("RevRev: start");
            if (User.Identity.IsAuthenticated)
            {
                int reviewID = (id == null) ? -1 : (int)id;
                _logger.LogInformation("RevRev: authenticated with revid = {val}", reviewID);

                // emp is the employee whose review it is - it is a required parameter
                if (emp == null) return NotFound();
                string targetID = emp.Trim();
                _logger.LogInformation("RevRev: targetID = {val}", targetID);

                // Make sure user has permission to view
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                // Redirect to their own index page if they don't have permission to view
                if (targetID != user.EmployeeID)
                {
                    if (!viewAllowed(user.EmployeeID, targetID))
                    {
                        _logger.LogInformation("RevRev: user {uid} is not allowed to view {tid}", user.EmployeeID, targetID);
                        return RedirectToRoute(new
                        {
                            controller = "Reviews",
                            action = "Index"
                        });
                    }
                }

                // Get the review ID (and create if needed)
                string getEmployee = "EXECUTE dbo.avp_Get_Employee @UserEmpId";
                var empId = new SqlParameter("@UserEmpId", targetID);
                var employee = await _context.Employees.FromSql(getEmployee, empId)
                    .SingleOrDefaultAsync<Employees>();

                if (reviewID < 0) // Just get the current one.
                {
                    _logger.LogInformation("Getting the current review");
                    if (employee.CurrentReview == null)
                    {
                        // We need to create a new review for this employee
                        DateTime startDate = DateTime.Now;
                        DateTime endDate = startDate.AddDays(90);
                        _logger.LogInformation("Creating a new review");

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
                    }
                    reviewID = (int) employee.CurrentReview;
                    _logger.LogInformation("Current review is {rid}", reviewID);

                }

                // Now read the review
                DisplayReview rev = null;
                _logger.LogInformation("Now read the review");

                var getReviewCmd = _context.Database.GetDbConnection().CreateCommand();

                getReviewCmd.CommandText = "EXEC [dbo].[avp_Get_A_Review] " +
                    String.Format("@ReviewID = {0}", reviewID);

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
                _logger.LogInformation("RevRev: And show it!");
                return View(rev);
            }
            _logger.LogInformation("RevRev: WOOPS");

            return NotFound();
        }


        // POST: Reviews/Review/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(int id)
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
                controller = "Reviews",
                action = "Index"
            });
        }

        // BELOW HERE IS OLD - The Edit GET and POST routines should be deleted.

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
