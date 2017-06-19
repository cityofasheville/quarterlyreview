using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using QuarterlyReview.Models;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace QuarterlyReview.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = null;
        private readonly ILogger _logger;
        private readonly QuarterlyReviewsContext _context;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            ILoggerFactory loggerFactory,
            QuarterlyReviewsContext context
            )
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<HomeController>();
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                DateTime startDate = DateTime.Now;
                DateTime endDate = startDate.AddDays(90);
                string getEmployee = "EXECUTE dbo.avp_Get_Employee @UserEmpId";
                string getMyEmployees = "EXECUTE dbo.avp_Get_My_Employees @UserEmpId";
                string getReviewsOfMe = "EXECUTE dbo.avp_Reviews_Of_Me @UserEmpId";

                var empId = new SqlParameter("@UserEmpId", user.EmployeeID);
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

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
