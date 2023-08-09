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
            ViewData["ProfileID"] = id;
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

        [Route("/users/account/{userId:int}/profile")]
        public IActionResult Profile(int userId)
        {
            var user = _context.Users.Where(u => u.Id == userId).Include(u => u.Messages).First();
            ViewData["ProfileID"] = userId;
            return View(user);
        }


        [Route("/users/account/{userId:int}/profile/update")]
        public IActionResult UpdateProfile(int userId)
        {
            var user = _context.Users.Where(u => u.Id == userId).Include(u => u.Messages).First();
            ViewData["ProfileID"] = userId;

            return View(user);
        }


        [HttpPost]
        [Route("/users/account/{userId:int}/profile")]
        public IActionResult ProfileUpdater(User user, int userId)
        {
            user.Id = userId;
            _context.Users.Update(user);
            _context.SaveChanges();
            //ViewData["ProfileID"] = userId;

            return Redirect($"/users/account/{user.Id}/profile");
        }

        [Route("/users/account/{userId:int}/feed")]
        public IActionResult Feed(int userId)
        {
            var users = _context.Users.Include(u => u.Messages).ToList();
            users = _context.GenerateRandomLoop(users);
            users = users.Where(u => u.Id != userId).ToList();
            ViewData["ProfileID"] = userId;

            return View(users);
        }


        [Route("/users/account/{userId:int}/deleteaccount")]
        public IActionResult DeletePage(int userId)
        {
            var user = _context.Users.Find(userId);
            return View(user);
        }

        [HttpPost]
        [Route("/users/{userId:int}/delete")]
        public IActionResult RemoveUser(int userId)
        {
            var user = _context.Users.Where(u => u.Id ==userId).Include( u => u.Messages).First();
            _context.Users.Remove(user);
            _context.SaveChanges();
            return Redirect("/users/login");
        }

        [Route("/users/account/{userId:int}/searchusers")]
        public IActionResult SearchUser(int userId, string q)
        {
            var users = _context.Users.Where(u => u.UserName.Contains(q)).ToList();
            ViewData["LoggedinAccount"] = userId;
            ViewData["ProfileID"] = userId;
            return View(users);
        }

        [Route("/users/account/{userId:int}/viewprofile/{accountId:int}")]
        public IActionResult ViewProfile(int userId, int accountId)
        {
            var user = _context.Users.Where(u => u.Id == accountId).Include(u => u.Messages).First();
            ViewData["ProfileID"] = userId;

            return View(user);
        }

    }
}
