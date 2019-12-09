using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bank_account.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace bank_account.Controllers
{
    public class HomeController : Controller
    {
    private MyContext dbContext;
    public HomeController(MyContext context)
    {
        dbContext = context;
    }
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost("register")]
    public IActionResult Register(User newUser)
    {
        if(ModelState.IsValid)
        {
            if(dbContext.Users.Any(user => user.email == newUser.email))
            {
                ModelState.AddModelError("email", "Email is already registered.");
                return View("Index");
            }
            else
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.password = Hasher.HashPassword(newUser, newUser.password);
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetString("User", newUser.email);
                return RedirectToAction("Account");
            }
        }
        else
        {
            return View("Index");
        }
    }
    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost("login")]
    public IActionResult Login(LoginUser existingUser)
    {
        if(ModelState.IsValid)
        {
            if(dbContext.Users.Any(user => user.email == existingUser.email))
            {
                User userInDb = dbContext.Users.FirstOrDefault(user => user.email == existingUser.email);
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(existingUser, userInDb.password, existingUser.password);
                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                else
                {
                    HttpContext.Session.SetString("User", userInDb.firstName);
                    HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                    return RedirectToAction("Account");
                }
            }
            else
            {
                ModelState.AddModelError("email", "Email has not been registered.");
                return View("Login");
            }
        }
        else
        {
            return View("Login");
        }
    }
    [HttpGet("account")]
    public IActionResult Account()
    {
        if(HttpContext.Session.GetString("User")==null)
        {
            return RedirectToAction("Index");
        }
        else
        {
            var User = HttpContext.Session.GetString("User");
            ViewBag.User = User;
            var thisUser = dbContext.Transactions.Where(u => u.UserId == HttpContext.Session.GetInt32("UserId")).ToList();
            ViewBag.Balance = thisUser.Sum(t => t.Amount);
            List<Transaction> allTransactions = dbContext.Transactions.OrderByDescending(t=>t.CreatedAt).ToList();
            return View(new AddTransactionViewModel{ListofTransactions = thisUser});
        }
    }
    [HttpPost("transaction")]
    public IActionResult Transaction(AddTransactionViewModel transfer)
    {
        var newTransaction = new Transaction()
        {
            Amount = transfer.Transaction.Amount,
            CreatedAt = DateTime.Now,
            UserId = (int)HttpContext.Session.GetInt32("UserId"),
        };
        dbContext.Add(newTransaction);
        dbContext.SaveChanges();
        return RedirectToAction("Account");
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
