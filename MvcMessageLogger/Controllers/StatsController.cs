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

            //string mostPopularWord = MostPopularWord();
            //ViewData["MostPopularWord"] = mostPopularWord;
            ViewData["HourWithMostMessages"] = HourWithMostMessages();
            return View(users);

        }

        //string MostPopularWord()
        //{
        //    var messages = _context.Messages;

        //    string words =string.Empty;

        //    foreach (var message in messages)
        //    {
        //        words += " " + message.Content.ToLower();
        //    }
        //    char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
        //    string[] allwords = words.Split(delimiterChars);
        //    string mostPopular = allwords.GroupBy(s => s).OrderByDescending(g => g.Count()).ToList().First().ToString();

        //    return mostPopular;


        //}

         List<int> HourWithMostMessages()
        {
            var hours = _context.Messages.GroupBy(t => t.CreatedAt.ToLocalTime().Hour);
            int mostmessage = 0;
            int hour = 0;
            foreach (var h in hours)
            {
                if (h.Count() > mostmessage)
                {
                    mostmessage = h.Count();
                    hour = h.Key;
                }
            }
            List<int> data = new List<int>();
            data.Add(hour);
            data.Add(mostmessage);
            return data;
        }
    }


}
