using Microsoft.AspNetCore.Mvc;
using RedisDemo.Models;
using StackExchange.Redis;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using RedisDemo.Data;

namespace RedisDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConnectionMultiplexer _redis;
      

        public HomeController(ILogger<HomeController> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
        }

        public IActionResult Index()
        {
            var user = new UserModel
            {
                Address = "Some house somewhere",
                Age = 40,
                Mobile = "9988558855",
                Name = "Gaurav Singh"
            };

            RedisHandler redisHandler = new RedisHandler(_redis, _logger);
            redisHandler.Set(user.Mobile, user);
            


            //    string userString = JsonConvert.SerializeObject(user);

            //IDatabase db = _redis.GetDatabase();
            //db.StringSet(user.Mobile, userString);
            return View();
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation("Inside Privacy Action Method.");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}