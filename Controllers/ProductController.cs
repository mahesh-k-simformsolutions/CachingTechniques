using CachingTechniques.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace CachingTechniques.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _dbContext;

        private readonly IMemoryCache _memoryCache;

        private readonly IDistributedCache _distributedCache;

        public ProductController(AppDbContext dbContext, IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _dbContext = dbContext;

            _memoryCache = memoryCache;

            _distributedCache = distributedCache;
        }

        public async Task<IActionResult> MemoryCache()
        {
            var cacheData = _memoryCache.Get<IEnumerable<Product>>("products");
            if (cacheData != null)
            {
                return View(cacheData);
            }

            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            cacheData = await _dbContext.Products.ToListAsync();
            _memoryCache.Set("products", cacheData, expirationTime);
            return View(cacheData);
        }

        public async Task<IActionResult> DistributedCache()
        {
            var cacheData = await _distributedCache.GetStringAsync("products");
            if (cacheData != null)
            {
                return View(JsonConvert.DeserializeObject<IEnumerable<Product>>(cacheData));
            }

            var expirationTime = TimeSpan.FromMinutes(5.0);
            var products = await _dbContext.Products.ToListAsync();
            cacheData = JsonConvert.SerializeObject(products);
            var cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(expirationTime);
            await _distributedCache.SetStringAsync("products", cacheData, cacheOptions);
            return View(products);

        }

        [ResponseCache(Location = ResponseCacheLocation.Any,Duration =10000)]
        public async Task<IActionResult> ResponseCache()
        {
            var products = await _dbContext.Products.ToListAsync();
            return View(products);

        }
    }
}
