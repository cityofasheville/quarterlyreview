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
                DateTime startDate = DateTime.Now;
                DateTime endDate = startDate.AddDays(90);
                string getEmployee = "EXECUTE dbo.avp_Get_Employee @UserEmpId";
                string getMyEmployees = "EXECUTE dbo.avp_Get_My_Employees @UserEmpId";
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                ViewData["EmployeeID"] = user.EmployeeID;
                _logger.LogInformation(1, "Yippee 1 user email is {Email} and ID = {Id}.", user.Email, user.EmployeeID);
                var empId = new SqlParameter("@UserEmpId", user.EmployeeID);
                var employee = await _context.Employees.FromSql(getEmployee, empId)
                    .SingleOrDefaultAsync<Employees>();
                ViewData["EmployeeID"] = user.EmployeeID;
                ViewData["Supervisor"] = employee.Supervisor;
                //ViewData["Supervisor"] = String.Format("Hello {0}", employee.Supervisor);

                var myEmployees = await _context.Employees.FromSql(getMyEmployees, empId)
                    .ToListAsync<Employees>();
                ViewData["Employees"] = myEmployees;
                ViewData["Employee"] = employee.Employee;
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
