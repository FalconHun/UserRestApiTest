using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using UserService.Models;

namespace UserService.DataAccess
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviecScope = app.ApplicationServices.CreateScope())
            {
                SeedSqlData(serviecScope.ServiceProvider.GetService<SqlDbContext>());
                SeedMongoDbData(serviecScope.ServiceProvider.GetService<MongoDbContext>());
            }
        }

        public static void SeedSqlData(SqlDbContext context)
        {
            if (context == null)
                return;

            Console.WriteLine("Appling Migrations...");

            if (!context.Database.IsInMemory())
            {
                context.Database.Migrate();
            }

            if (!context.Users.Any())
            {
                Console.WriteLine("Adding data - seeding");

                context.Users.AddRange(GetInitialUsers(() => Guid.NewGuid().ToString()));
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Already have data - not seeding");
            }
        }

        public static void SeedMongoDbData(MongoDbContext context)
        {
            if (context == null)
                return;

            Console.WriteLine("Appling Migrations...");

            if (!context.Users.Find(u => true).Any())
            {
                Console.WriteLine("Adding data - seeding");

                context.Users.InsertMany(GetInitialUsers(() => ObjectId.GenerateNewId().ToString()));
            }
            else
            {
                Console.WriteLine("Already have data - not seeding");
            }
        }

        private static User[] GetInitialUsers(Func<string> idGenerator)
        {
            return new[]
            {
                new User { FirstName = "TestUser1", LastName = "UserName", Age = 38, Id = idGenerator() },
                new User { FirstName = "TestUser2", LastName = "UserName", Age = 40, Id = idGenerator() }
            };
        }
    }
}
