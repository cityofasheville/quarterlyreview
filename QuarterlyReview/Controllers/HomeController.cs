using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using QuarterlyReview.Models;
using Microsoft.Extensions.Logging;

namespace QuarterlyReview.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = null;
        private readonly ILogger _logger;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            ILoggerFactory loggerFactory
            )
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<HomeController>();
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                _logger.LogInformation(1, "Yippee 1 user email is {Email} and ID = {Id}.", user.Email, user.EmployeeID);
            }
            return View();
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
