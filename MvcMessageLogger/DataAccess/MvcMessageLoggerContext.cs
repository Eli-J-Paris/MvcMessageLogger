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

    }
}
