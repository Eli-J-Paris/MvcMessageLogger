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
    [Collection("Controller Tests")]
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
            Assert.Contains("<textarea class=\"form-control\" id=\"exampleTextarea\" rows=\"5\" maxlength=\"255\" placeholder=\"new chirp\" name=\"Content\"></textarea>\r\n", html);
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

        [Fact]
        public async Task EditMessage_DisplaysFormToEditAMessage()
        {
            var client = _factory.CreateClient();
            var context = GetDbContext();
            var user = new User { UserName = "eli", Email = "eli@Yahoo.com", Password = "password123" };
            var message = new Message { Content = "hello", CreatedAt = DateTime.Now.ToUniversalTime() };
            user.Messages.Add(message);
            context.Users.Add(user);
            context.SaveChanges();

            var response = await client.GetAsync($"/users/account/{user.Id}/edit/{message.Id}");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("Save Edits", html);
        }

        [Fact]
        public async Task Update_UpdatesAMessage()
        {
            var client = _factory.CreateClient();
            var context = GetDbContext();
            var user = new User { UserName = "eli", Email = "eli@Yahoo.com", Password = "password123" };
            var message = new Message { Content = "hello", CreatedAt = DateTime.Now.ToUniversalTime() };
            user.Messages.Add(message);
            context.Users.Add(user);
            context.SaveChanges();

            var formData = new Dictionary<string, string>
            {
                {"Content","hello world" },
                {"CreatedAt","01-01-02" }
            };

            var response = await client.PostAsync($"/users/account/{user.Id}/edit/{message.Id}", new FormUrlEncodedContent(formData));
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("hello world", html);
        }

        [Fact]
        public async Task Delete_RemovesAMessageFromDataBase()
        {
            var client = _factory.CreateClient();
            var context = GetDbContext();
            var user = new User { UserName = "eli", Email = "eli@Yahoo.com", Password = "password123" };
            var message = new Message { Content = "hello", CreatedAt = DateTime.Now.ToUniversalTime() };
            user.Messages.Add(message);
            context.Users.Add(user);
            context.SaveChanges();

            var formData = new Dictionary<string, string>
            {

            };

            var response = await client.PostAsync($"/users/account/{user.Id}/delete/{message.Id}", new FormUrlEncodedContent(formData));
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain("hello", html);
            Assert.Contains("eli's", html);
        }
    }
}
