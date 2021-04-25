using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using SiteActivityApi.Controllers;
using SiteActivityApi.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiteActivityApi.Test
{
    public class Tests
    {
        private ActivityController _controller;
        private MemoryCache _cache;

        [SetUp]
        public void Setup()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _controller = new ActivityController(_cache);
        }

        [Test]
        public void Post_ReturnsOk()
        {
            var response = _controller.Post("learn_more_page", new Activity { Value = 16d });
            Assert.IsInstanceOf<OkResult>(response);
        }

        [Test]
        public void Post_ValueLessThanOrEqualToZeroReturnsBadRequest()
        {
            var response = _controller.Post("learn_more_page", new Activity { Value = 0d });
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public void Get_WhenRequestedAnyKeyReturnsOk()
        {
            var response = _controller.Get("Any Key");
            Assert.IsInstanceOf<OkObjectResult>(response);
        }

        [Test]
        public void Get_SumOfActivitiesOnlyEqualsToValuesInsertedLast12Hours()
        {
            var activities = new List<Activity>
            {
                new Activity{Key="learn_more_page", Value=16d, Time = DateTime.Now.AddMinutes(-781)},
                new Activity{Key="learn_more_page", Value=5d, Time = DateTime.Now.AddMinutes(-510)},
                new Activity{Key="learn_more_page", Value=32d, Time = DateTime.Now.AddSeconds(-50)},
                new Activity{Key="learn_more_page", Value=4d, Time = DateTime.Now.AddSeconds(-3)},
            };
            _cache.Set("activities", activities);
            var result = (_controller.Get("learn_more_page") as OkObjectResult).Value;
            var actual = (double)result.GetType().GetProperty("value").GetValue(result, null);
            var expected = 41d;
            Assert.AreEqual(expected, actual);
        }
    }
}