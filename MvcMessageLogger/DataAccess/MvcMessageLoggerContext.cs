using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.DataAccess
{
    public class MvcMessageLoggerContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }

        public MvcMessageLoggerContext(DbContextOptions<MvcMessageLoggerContext> options)
            : base(options) { }

        public IGrouping<string,string> MostPopularWord()
        {
            var messages = Messages;

            string words = string.Empty;

            foreach (var message in messages)
            {
                words += " " + message.Content.ToLower();
            }
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] allwords = words.Split(delimiterChars);
            var mostPopular = allwords.GroupBy(s => s).OrderByDescending(g => g.Count()).ToList().First();

            return mostPopular;
        }

        public List<int> HourWithMostMessages()
        {
            var hours = Messages.GroupBy(t => t.CreatedAt.ToLocalTime().Hour);
            int mostmessage = 0;
            int hour = 0;
            foreach (var h in hours)
            {
                if (h.Count() > mostmessage)
                {
                    mostmessage = h.Count();
                    hour = h.Key;
                }
            }
            List<int> data = new List<int>
            {
                hour,
                mostmessage
            };
            return data;
        }

        public List<User> GenerateRandomLoop(List<User> listToShuffle)
        {
           var _rand = new Random();
            for (int i = listToShuffle.Count - 1; i > 0; i--)
            {
                var k = _rand.Next(i + 1);
                var value = listToShuffle[k];
                listToShuffle[k] = listToShuffle[i];
                listToShuffle[i] = value;
            }
            return listToShuffle;
        }

        public List<string> MostActiveUser()
        {
            List<string> mostActiveUser = new List<string>{"another baindaid"};
            int bandaid = 0;
            var users = Users.Where(u => u.Messages.Count > 0).Include(u => u.Messages).ToList();

            foreach(var user in users)
            {
                if (user.Messages.Count() > bandaid)
                {
                    bandaid = user.Messages.Count();
                    mostActiveUser[0] = user.UserName;
                }
            }
            mostActiveUser.Add(bandaid.ToString());
            return mostActiveUser;
        }
    }
}
