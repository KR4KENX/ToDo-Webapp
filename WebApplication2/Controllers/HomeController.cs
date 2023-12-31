﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using WebApplication2.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private DbConfig _dbConfig;
        private Ultilities ultilities = new();

        //Routes
        public HomeController(ILogger<HomeController> logger, DbConfig dbConfig)
        {
            _logger = logger;
            _dbConfig = dbConfig;
        }
        public IActionResult Index()
        {
            return View();
        }
   
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ToDo(string Title, string Description)
        {
            if (!ultilities.IsLogged(HttpContext))
                return View("Unathorized");

            var username = HttpContext.Session.GetString(SessionVariables.SessionKeyUsername);
            var userId = _dbConfig.User.First<UserModel>(x => x.Username == username).Id;

            if (Title != null && Description != null)
            {
                var toDo = new ToDoModel();
                toDo.Title = Title;
                toDo.Description = Description;

                toDo.OwnerId = userId;

                _dbConfig.ToDo.Add(toDo);
                _dbConfig.SaveChanges();
            };

            ViewBag.toDos = _dbConfig.ToDo.Where(x => x.OwnerId == userId).ToList();

            return View();
        }
        public async Task<IActionResult> Account(string Username, string Password)
        {
            if (Username != null && Password != null)
            {
                UserModel user;
                try
                {
                    user = _dbConfig.User.First<UserModel>(x => x.Username == Username);
                }
                catch(Exception ex)
                {
                    return Content(ex.Message);
                }
                string hashedPasswd = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Password,
                salt: user.Salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

                try {  user = _dbConfig.User.First<UserModel>(x => x.Username == Username && x.Password == hashedPasswd); }
                catch (Exception ex)
                {
                    return Content("Bad password");
                }

                if (user != null && string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionVariables.SessionKeyUsername)))
                {
                    HttpContext.Session.SetString(SessionVariables.SessionKeyUsername.ToString(), Username);
                    HttpContext.Session.SetString(SessionVariables.SessionKeySessionId.ToString(), Guid.NewGuid().ToString());
                }

                return View();
            }
            else { return View(); }
        }
        public IActionResult Register(string Username, string Password)
        {
            if (Username != null && Password != null) {
                byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
                string hashedPasswd = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

                var user = new UserModel();
                user.Username = Username;
                user.Password = hashedPasswd;
                user.Salt = salt;

                _dbConfig.User.Add(user);
                _dbConfig.SaveChanges();
                ViewBag.Registered = true;
                return View();
            }
            else
            {
                return View();
            }

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionVariables.SessionKeySessionId);
            HttpContext.Session.Remove(SessionVariables.SessionKeyUsername);
            return Redirect("/");
        }
        public IActionResult Delete(int id)
        {
            _dbConfig.ToDo.Where(x => x.Id == id).ExecuteDelete();
            _dbConfig.SaveChanges();
            return Redirect("/Home/ToDo");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}