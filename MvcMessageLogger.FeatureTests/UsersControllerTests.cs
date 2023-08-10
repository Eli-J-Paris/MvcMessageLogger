using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.FeatureTests
{
    [Collection("Controller Tests")]
    public class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UsersControllerTests(WebApplicationFactory<Program> factory)
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
        public async Task Index_DisplaysAllUsers()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var user = new User { UserName = "eli", Email = "eli@Yahoo.com", Password = "password123" };
            var user2 = new User { UserName = "Zoro", Email = "Zoro@Yahoo.com", Password = "sword" };

            context.Users.Add(user);
            context.Users.Add(user2);
            context.SaveChanges();

            var response = await client.GetAsync("/users");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains(user.UserName, html);
            Assert.Contains(user2.UserName, html);

        }

        [Fact]
        public async Task UserHome_DisplaysUsersPage()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var user = new User { UserName = "eli", Email = "eli@Yahoo.com", Password = "password123" };
            var user2 = new User { UserName = "Zoro", Email = "Zoro@Yahoo.com", Password = "sword" };

            context.Users.Add(user);
            context.Users.Add(user2);
            context.SaveChanges();

            var response = await client.GetAsync($"/users/account/{user.Id}");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains(user.UserName, html);
            Assert.DoesNotContain(user2.UserName, html);

        }

        [Fact]
        public async Task New_DisplaysFormToCreateNewAccount()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/users/newaccount");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("<form method=\"post\" action=\"/users/newaccount\">", html);

        }


        [Fact]
        public async Task Create_AddsUserToDataBase()
        {
            var client = _factory.CreateClient();

            var formData = new Dictionary<string, string>
            {
                {"UserName","Usopp" },
                {"Email", "Usopp@hotmail" },
                {"Password", "sniperking" }
            };

            var response = await client.PostAsync($"users/newaccount", new FormUrlEncodedContent(formData));
            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("Usopp", html);
        }

        [Fact]
        public async Task Login_DisplaysLoginPage()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/users/login");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("<form method=\"post\" action=\"/users/login\">", html);
        }

        

        [Fact]
        public async Task Signin_RedirectsUserToAccountPage()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();


            var user = new User { UserName = "Usopp", Email = "Usopp@hotmail", Password = "sniperking" };
            var message = new Message { Content = "hello" };
            user.Messages.Add(message);
            context.Users.Add(user);
            context.SaveChanges();

            var formData = new Dictionary<string, string>
            {
                {"UserName","Usopp" },
                {"Email", "Usopp@hotmail" },
                {"Password", "sniperking" }
            };

            var response = await client.PostAsync($"users/account/1", new FormUrlEncodedContent(formData));

            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("Usopp",html);
        }

        [Fact]
        public async Task Profile_DisplaysUsersProfilePage()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var user = new User { UserName = "eli", Email = "eli@Yahoo.com", Password = "password123" };
            var user2 = new User { UserName = "Zoro", Email = "Zoro@Yahoo.com", Password = "sword" };

            context.Users.Add(user);
            context.Users.Add(user2);
            context.SaveChanges();

            var response = await client.GetAsync($"/users/account/{user.Id}/profile");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains(user.UserName, html);
            Assert.Contains("profile",html);
            Assert.DoesNotContain(user2.UserName, html);
        }

        [Fact]
        public async Task UpdateProfile_DisplaysUsersUpdatePageforUserProfile()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var user = new User { UserName = "eli", Email = "eli@Yahoo.com", Password = "password123" };
            var user2 = new User { UserName = "Zoro", Email = "Zoro@Yahoo.com", Password = "sword" };

            context.Users.Add(user);
            context.Users.Add(user2);
            context.SaveChanges();

            var response = await client.GetAsync($"/users/account/{user.Id}/profile/update");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains(user.UserName, html);
            Assert.Contains("Update Your Profile", html);
            Assert.DoesNotContain(user2.UserName, html);
        }

        [Fact]
        public async Task ProfileUpdater_UpdatesAUsersProfile()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var user = new User { UserName = "eli", Email = "eli@Yahoo.com", Password = "password123" };
            var user2 = new User { UserName = "Zoro", Email = "Zoro@Yahoo.com", Password = "sword" };

            context.Users.Add(user);
            context.Users.Add(user2);
            context.SaveChanges();

            var formData = new Dictionary<string, string>
            {
                {"Name","Usopp" },
                {"Location", "CO" },
                {"Bio", "Hello world" }
            };

            var response = await client.PostAsync($"users/account/{user.Id}/profile", new FormUrlEncodedContent(formData));

            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("Usopp", html);
            Assert.Contains("CO", html);
            Assert.Contains("Hello world", html);

        }

        [Fact]
        public async Task Feed_DisplaysOtherUSersMessages()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var user = new User { UserName = "eli", Email = "eli@Yahoo.com", Password = "password123" };
            var user2 = new User { UserName = "Zoro", Email = "Zoro@Yahoo.com", Password = "sword" };
            var message = new Message { Content = "hello", CreatedAt = DateTime.Now.ToUniversalTime() };
            var message2 = new Message { Content = "bye", CreatedAt = DateTime.Now.ToUniversalTime() };
            user.Messages.Add(message);
            user2.Messages.Add(message2);
            context.Users.Add(user);
            context.Users.Add(user2);
            context.SaveChanges();


            var response = await client.GetAsync($"/users/account/{user.Id}/feed");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("bye", html);
            Assert.DoesNotContain("hello", html);
        }

        [Fact]
        public async Task DeletePage_DisplayDeletePageToConfirmDelete()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var user = new User { UserName = "eli", Email = "eli@Yahoo.com", Password = "password123" };

            context.Users.Add(user);
            context.SaveChanges();

            var response = await client.GetAsync($"/users/account/{user.Id}/deleteaccount");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("Delete Account", html);
            Assert.Contains("Keep Account", html);
        }

        [Fact]
        public async Task RemoveUser_RemovesUserFromDataBase()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var user = new User { UserName = "A3&h442", Email = "eli@Yahoo.com", Password = "password123" };

            context.Users.Add(user);
            context.SaveChanges();

            var formData = new Dictionary<string, string>
            {

            };

            var response = await client.PostAsync($"users/{user.Id}/delete", new FormUrlEncodedContent(formData));

            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain("A3&h442", html);

        }

        [Fact]
        public async Task SearchUser_findsUserInDB()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var user = new User { UserName = "Eli", Email = "eli@Yahoo.com", Password = "password123" };
            var user2 = new User { UserName = "Zoro", Email = "Zoro@Yahoo.com", Password = "sword" };
            context.Users.Add(user);
            context.Users.Add(user2);
            context.SaveChanges();


            var formData = new Dictionary<string, string>
            {
                {"q","Zoro" }
            };

            var response = await client.PostAsync($"users/account/{user.Id}/searchusers", new FormUrlEncodedContent(formData));
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("Zoro", html);
            Assert.DoesNotContain("Eli", html);
        }

        [Fact]
        public async Task ViewProfile_ViewsAnotherUsersProfile()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var user = new User { UserName = "Eli", Email = "eli@Yahoo.com", Password = "password123", Bio = "test bio" };
            var user2 = new User { UserName = "Zoro", Email = "Zoro@Yahoo.com", Password = "sword", Name = "Zolo", Bio = "Swords" };
            context.Users.Add(user);
            context.Users.Add(user2);
            context.SaveChanges();

            var response = await client.GetAsync($"/users/account/{user.Id}/viewprofile/{user2.Id}");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("Zolo", html);
            Assert.Contains("Swords", html);
            Assert.Contains("Location", html);
            Assert.DoesNotContain("test bio", html);

        }
    }
    
}