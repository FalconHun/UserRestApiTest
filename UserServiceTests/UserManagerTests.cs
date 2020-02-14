using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UserService.Models;
using System.Linq;

namespace UserService.Tests
{
    public class UserManagerTests
    {
        private UserManager userManager;
        private UserContext userContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<UserContext>().UseInMemoryDatabase(databaseName: "UserDatabase").Options;

            userContext = new UserContext(options);
            foreach (var user in userContext.Users.ToArray())
            {
                userContext.Remove(user);
            }
            userManager = new UserManager(userContext);
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
