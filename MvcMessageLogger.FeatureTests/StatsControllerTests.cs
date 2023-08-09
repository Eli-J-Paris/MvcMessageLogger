using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcMessageLogger.FeatureTests
{
    public class StatsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public StatsControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private MvcMessageLoggerContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MvcMessageLoggerContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");

            var context = new MvcMessageLoggerContext(optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }

        //[Fact]
        //public async Task Index_DisplaysStats()
        //{
        //    var context = GetDbContext();
        //    var client = _factory.CreateClient();

        //    var user1 = new User { UserName = "Zoro", Email = "Zoro@Yahoo.com", Password = "sword" };
        //    context.Users.Add(user1);
        //    context.SaveChanges();


        //    var response = await client.GetAsync($"/users/account/{user1.Id}/stats");
        //    response.EnsureSuccessStatusCode();

        //    var html = await response.Content.ReadAsStringAsync();

        //    Assert.Contains("More stats are under construction, thank you for your patiences", html);
        //}
    }
}
