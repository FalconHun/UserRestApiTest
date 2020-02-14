using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.DataAccess
{
    public class SqlDbContext : DbContext
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
