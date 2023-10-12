using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedisDemo.Models;
using StackExchange.Redis;

namespace RedisDemo.Controllers
{
    public class RedisDemoController : Controller
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<HomeController> _logger;

        public RedisDemoController(IConnectionMultiplexer redis, ILogger<HomeController> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        public IActionResult Index()
        {
            //// Example: Storing and retrieving data
            //IDatabase db = _redis.GetDatabase();

            //string userString = db.StringGet("9988558855");
            //var user = JsonConvert.DeserializeObject(userString);

            var redis = new RedisHandler(_redis, _logger);
          
                var user = redis.GetDataFromCache<UserModel>("9988558855");
                return View(user);
          
        }
    }
}
