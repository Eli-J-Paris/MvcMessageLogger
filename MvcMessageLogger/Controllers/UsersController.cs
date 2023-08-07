using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.Controllers
{
    public class UsersController : Controller
    {
        private readonly MvcMessageLoggerContext _context;

        public UsersController(MvcMessageLoggerContext context )
        {
            _context = context; 
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }


        [Route("/users/account/{id:int}")]
        public IActionResult UserHome(int id)
        {
            var user = _context.Users.Where(u=> u.Id == id).Include(u =>u.Messages).First();
            return View(user);
        }

        [Route("/users/newaccount")]
        public IActionResult New(string? error)
        {
            if (error == "true")
            {
                ViewData["FailedSignin"] = true;

            }
            return View();
        }


        [HttpPost]
        [Route("/users/newaccount")]
        public IActionResult Create(User user)
        {
            // DRY1
            User validateUser;
            try
            {
                validateUser = _context.Users.Where(u => u.Email == user.Email).First();
            }
            catch (InvalidOperationException)
            {
                validateUser = null;
            }

            if (validateUser == null)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return Redirect($"/users/account/{user.Id}");
            }
            else
            {
                return Redirect("/users/newaccount/?error=true");
            }
        }

        [Route("/users/login")]
        public IActionResult Login(string? error)
        {
            if(error == "true")
            {
                ViewData["FailedLogin"] = true;
            }
            return View();
        }


        [HttpPost]
        [Route("/users/login")]
        public IActionResult Signin(User user)
        {
            // DRY1
            User validateUser;
            try
            {
                validateUser = _context.Users.Where(u => u.Email == user.Email).First();
            }
            catch (InvalidOperationException)
            {
                validateUser = null;
            }
            if (validateUser != null && validateUser.Password == user.Password)
            {
                return Redirect($"/users/account/{validateUser.Id}");
            }
            else
            {
                return Redirect("/users/login/?error=true");
            }
        }
    }
}
