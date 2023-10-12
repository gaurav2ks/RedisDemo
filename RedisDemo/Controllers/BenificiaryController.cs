using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisDemo.Data;
using RedisDemo.Models;
using StackExchange.Redis;
using System.Reflection;

namespace RedisDemo.Controllers
{
    public class BenificiaryController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IConnectionMultiplexer _redisContext;
        private readonly ApplicationDbContext _dbContext;
        

        public BenificiaryController(ILogger<HomeController> logger, IConnectionMultiplexer redis, ApplicationDbContext context)
        {
            _logger = logger;
            _redisContext = redis;
            _dbContext = context;
        }

        // GET: BenificiaryController
        public ActionResult Index()
        {
            var benficiaries = _dbContext.UserModel;
            return View(benficiaries.ToList());
        }

        // GET: BenificiaryController/Details/9988776655
        public ActionResult Details(string id)
        {
            UserModel? beneficiary = TryGetBeneficiary(id);
            return View(beneficiary);
        }

        private UserModel? TryGetBeneficiary(string id)
        {
            var redis = new RedisHandler(_redisContext, _logger);
            var beneficiary = redis.GetDataFromCache<UserModel>(id);

            if (beneficiary == null)
            {
                _logger.LogInformation("Beneficiary Information not available in Cache. Getting it for DB!");
                int beneficiaryId = int.Parse(id);
                beneficiary = _dbContext.UserModel.Find(beneficiaryId);
                redis.Set(beneficiaryId.ToString(), beneficiary);
            }
            else
                _logger.LogInformation("Beneficiary Information found in Cache.");

            return beneficiary;
        }

        // GET: BenificiaryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BenificiaryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                UserModel user = new UserModel()
                {
                    Name = collection["Name"],
                    Age = Convert.ToInt32(collection["Age"]),
                    Mobile = collection["Mobile"],
                    Address = collection["address"]
                };
                var result = _dbContext.UserModel.Add(user);
                _dbContext.SaveChanges();
               
                _logger.LogInformation("User created successfully with UserId: " + result.ToString());
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BenificiaryController/Edit/5
        public ActionResult Edit(int id)
        {
            //var beneficiary = _dbContext.UserModel.Find(id);
            UserModel? beneficiary = TryGetBeneficiary(id.ToString());
            return View(beneficiary);
        }

        // POST: BenificiaryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                var beneficiary = _dbContext.UserModel.Find(id);

                beneficiary.Name = collection["Name"];
                beneficiary.Age = Convert.ToInt32(collection["Age"]);
                beneficiary.Mobile = collection["Mobile"];
                beneficiary.Address = collection["address"];

                _dbContext.Entry(beneficiary).State = EntityState.Modified;
                _dbContext.SaveChanges();
                
                //Refresh the cache.
                RefreshCache(id, beneficiary);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        //Refresh the cache
        private void RefreshCache(int id, UserModel? beneficiary)
        {
            //Update the Cache
            _logger.LogInformation("Updating Beneficiary Information in Cache.");
            var redis = new RedisHandler(_redisContext, _logger);
            redis.Set(id.ToString(), beneficiary);
        }

        // GET: BenificiaryController/Delete/5
        public ActionResult Delete(int id)
        {
            var beneficiary = _dbContext.UserModel;
            return View(beneficiary.Find(id));
        }

        // POST: BenificiaryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                UserModel user = new UserModel()
                {
                    BeneficiaryId = id,
                    Name = collection["Name"],
                    Age = Convert.ToInt32(collection["Age"]),
                    Mobile = collection["Mobile"],
                    Address = collection["address"]
                };
                _dbContext.UserModel.Remove(user);
                _dbContext.SaveChanges();

                //Remove the beneficiary data from cache
                _logger.LogInformation("Flushing beneficiary details from cache. BeneficaryId: " + id.ToString());
                var redis = new RedisHandler(_redisContext, _logger);
                redis.DeleteKey(id.ToString());
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
