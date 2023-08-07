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
    public class MessagesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public MessagesControllerTests(WebApplicationFactory<Program> factory)
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

        [Fact]
        public async Task New_DisplaysFormforNewMessage()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();
            var user = new User { UserName = "eli", Email = "eli@Yahoo.com", Password = "password123" };

            context.Users.Add(user);
            context.SaveChanges();

            var response = await client.GetAsync($"/users/account/{user.Id}/newmessage");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("<form method=\"post\" action=\"/users/account/1\">", html);
            Assert.Contains("<textarea id=\"Content\" name=\"Content\" maxlength=\"255\"></textarea>", html);
        }

        [Fact]
        public async Task Create_AddsMessageToDataBase()
        {
            var client = _factory.CreateClient();
            var context = GetDbContext();
            var user = new User { UserName = "eli", Email = "eli@Yahoo.com", Password = "password123" };
            context.Users.Add(user);
            context.SaveChanges();

            var formData = new Dictionary<string, string>
            {
                {"Content","hello world" },
                {"CreatedAt","01-01-02" }
            };

            var response = await client.PostAsync($"users/account/{user.Id}", new FormUrlEncodedContent(formData));
            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("hello world", html);
        }

    }
}
