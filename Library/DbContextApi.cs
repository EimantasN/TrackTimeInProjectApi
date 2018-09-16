using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library
{
    public class DbContextApi : DbContext
    {
        public DbContextApi(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Members> Members { get; set; }
        public DbSet<Time> Times { get; set; }
        public DbSet<Error> Errors { get; set; }

        public DbSet<WorkTask> Task { get; set; }
    }
}
