using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SiteActivityApi.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiteActivityApi.Controllers
{
    [ApiController]
    [Route("/activity/{key}")]
    public class ActivityController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public ActivityController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        [HttpPost]
        public IActionResult Post(string key, Activity activity)
        {
            if (activity.Value <= 0)
            {
                return BadRequest("value should be greater than 0");
            }
            activity.Key =  key.ToLower().Trim();
            activity.Time = DateTime.Now;
            activity.Value = Math.Round(activity.Value);
            var activities = GetActivities();
            activities.Add(activity);
            return Ok();
        }

        [HttpGet("{total}")]
        public IActionResult Get(string key)
        {
            var now = DateTime.Now;
            var activities = GetActivities();
            var result = activities.Where(a => a.Key == key && (now - a.Time).TotalHours < 12 ).Sum(p=> p.Value);
            CleanUpActivityEntriesOlderThan12Hours();
            return Ok(new { value = result});
        }

        private void CleanUpActivityEntriesOlderThan12Hours()
        {
            var activities = GetActivities();
            var now = DateTime.Now;
            activities.RemoveAll(a => (now - a.Time).TotalHours >= 12);
        }

        private List<Activity> GetActivities()
        {
            return _cache.GetOrCreate<List<Activity>>("activities", p => new List<Activity>());
        }
    }
}
