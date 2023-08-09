using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;
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

        [Route("/users/account/{userId:int}/stats")]
        public IActionResult Index(int userId)
        {
            var users = _context.Users.Include(u => u.Messages).ToList();

            var mostPopularWord = _context.MostPopularWord();
            ViewData["MostPopularWord"] = mostPopularWord;
            ViewData["HourWithMostMessages"] = _context.HourWithMostMessages();
            ViewData["MostActiveUser"] = _context.MostActiveUser();

            ViewData["ProfileID"] = userId;
            return View(users);

        }
    }
}
