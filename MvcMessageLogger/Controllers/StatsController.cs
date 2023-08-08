using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using System;

namespace MvcMessageLogger.Controllers
{
    public class StatsController : Controller
    {
        private readonly MvcMessageLoggerContext _context;

        public StatsController(MvcMessageLoggerContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.Include(u => u.Messages).ToList();

            string mostPopularWord = _context.MostPopularWord();
            ViewData["MostPopularWord"] = mostPopularWord;
            ViewData["HourWithMostMessages"] = _context.HourWithMostMessages();
            return View(users);

        }
    }
}
