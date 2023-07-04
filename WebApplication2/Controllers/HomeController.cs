using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private DbConfig _dbConfig;

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
        public IActionResult ToDo()
        {
            var myData = _dbConfig.ToDo.ToList();
            ViewBag.toDos = myData;

            return View();
        }

        [HttpPost]
        public IActionResult ToDo(string Title, string Description)
        {
            var toDo = new ToDoModel();
            toDo.Title = Title;
            toDo.Description = Description;

            _dbConfig.ToDo.Add(toDo);
            _dbConfig.SaveChanges();

            var myData = _dbConfig.ToDo.ToList();
            ViewBag.toDos = myData;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}