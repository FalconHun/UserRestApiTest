using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UserService.Models;
using UserService.DataAccess;
using System.Linq;

namespace UserService.Tests
{
    public class UserManagerSqlTests
    {
        private UserManagerSql userManager;
        private SqlDbContext userContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SqlDbContext>().UseInMemoryDatabase(databaseName: "UserDatabase").Options;

            userContext = new SqlDbContext(options);
            foreach (var user in userContext.Users.ToArray())
            {
                userContext.Remove(user);
            }
            userManager = new UserManagerSql(userContext);
        }

        [Test]
        public void Create_ReturnsNewUser()
        {
            var user = userManager.Create("TestFirstName", "TestLastName", 10);

            Assert.AreEqual(1, userContext.Users.Count());

            Assert.AreEqual(user.FirstName, "TestFirstName");
            Assert.AreEqual(user.LastName, "TestLastName");
            Assert.AreEqual(user.Age, 10);
        }

        [Test]
        public void Create_StoresNewUser()
        {
            var user = userManager.Create("TestFirstName", "TestLastName", 10);
            
            var newUser = userContext.Users.First();

            Assert.AreEqual(newUser.FirstName, "TestFirstName");
            Assert.AreEqual(newUser.LastName, "TestLastName");
            Assert.AreEqual(newUser.Age, 10);
        }

        [Test]
        public void Delete_RemovesUser()
        {
            var user = new User { Id = "TestId", FirstName = "TestFirstName", LastName = "TestLastName", Age = 10 };
            userContext.Users.Add(user);
            userContext.SaveChanges();

            userManager.Delete(user.Id);

            Assert.IsFalse(userContext.Users.Any());
        }
    }
}
