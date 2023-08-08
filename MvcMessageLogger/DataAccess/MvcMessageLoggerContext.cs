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

        public string MostPopularWord()
        {
            var messages = Messages;

            string words = string.Empty;

            foreach (var message in messages)
            {
                words += " " + message.Content.ToLower();
            }
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] allwords = words.Split(delimiterChars);
            string mostPopular = allwords.GroupBy(s => s).OrderByDescending(g => g.Count()).ToList().First().ToString();

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
    }
}
