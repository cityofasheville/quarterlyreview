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
            return RedirectToRoute(new
            {
                controller = "Reviews",
                action = "Index"
            });
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
