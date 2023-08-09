using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.Controllers
{
    public class MessagesController : Controller
    {
        private readonly MvcMessageLoggerContext _context;

        public MessagesController(MvcMessageLoggerContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("/users/account/{userId:int}/newmessage")]
        public IActionResult New(int userId)
        {
            var user = _context.Users.Find(userId);
            ViewData["ProfileID"] = userId;

            return View(user);
        }


        [HttpPost]
        [Route("users/account/{userId:int}")]
        public IActionResult Create(int userId, Message message)
        {
            var user = _context.Users.Where(u =>u.Id == userId).Include(u => u.Messages).First();
           // var newMessage = new Message(message.Content);
            message.TimeCreated();
            user.Messages.Add(message);
            _context.SaveChanges();

            return Redirect($"/users/account/{userId}");
        }

    }
}
