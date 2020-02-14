using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace UserService.Models
{
    public static class PrepSqlDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviecScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviecScope.ServiceProvider.GetService<UserContext>());
            }
        }

        public static void SeedData(UserContext context)
        {          
            System.Console.WriteLine("Appling Migrations...");

            if (!context.Database.IsInMemory())
            {
                context.Database.Migrate();
            }

            if (!context.Users.Any())
            {
                System.Console.WriteLine("Adding data - seeding");
                context.Users.AddRange(
                    new User { FirstName = "TestUser1", LastName = "UserName", Age = 38, Id = System.Guid.NewGuid().ToString() },
                    new User { FirstName = "TestUser2", LastName = "UserName", Age = 40, Id = System.Guid.NewGuid().ToString() }
                );
                context.SaveChanges();
            }
            else
            {
                System.Console.WriteLine("Already have data - not seeding");
            }
        }
    }
}
