using ApiCachingApp.Data;
using ApiCachingApp.Models;
using ApiCachingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace ApiCachingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DriverController :ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly ApiDbContext _dbContext;
        public DriverController(ICacheService cacheService, ApiDbContext dbContext)
        {
            _cacheService = cacheService;
            _dbContext = dbContext;
        }

        [HttpGet("drivers")]
        public async Task<IActionResult> Get()
        {
            var cacheDrivers = _cacheService.GetData<IEnumerable<Driver>>("drivers");
            if(cacheDrivers != null && cacheDrivers.Count() > 0)
            {
                return Ok(cacheDrivers);
            }
            var drivers = await _dbContext.Drivers.ToListAsync();
            var expireTime = DateTimeOffset.Now.AddMinutes(2);
            _cacheService.SetData<IEnumerable<Driver>>("drivers", drivers, expireTime);
            return Ok(drivers);
        }
        [HttpPost]
        public async Task<IActionResult> Post(Driver driver)
        {
            await _dbContext.Drivers.AddAsync(driver);
            await _dbContext.SaveChangesAsync();
            return Ok(driver);
        }
    }
}
