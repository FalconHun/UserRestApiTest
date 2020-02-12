using NUnit.Framework;

namespace UserService.Tests
{
    public class UserManagerTests
    {
        private UserManager userManager;

        [SetUp]
        public void Setup()
        {
            userManager = new UserManager();
        }

        [Test]
        public void Create_ReturnsNewUser()
        {
            var user = userManager.Create("TestFirstName", "TestLastName", 10);

            Assert.AreEqual(user.FirstName, "TestFirstName");
            Assert.AreEqual(user.LastName, "TestLastName");
            Assert.AreEqual(user.Age, 10);
        }

        [Test]
        public void Create_StoresNewUser()
        {
            var user = userManager.Create("TestFirstName", "TestLastName", 10);

            var getUser = userManager.Get(user.Id);

            Assert.AreEqual(getUser.FirstName, "TestFirstName");
            Assert.AreEqual(getUser.LastName, "TestLastName");
            Assert.AreEqual(getUser.Age, 10);
        }

        [Test]
        public void Delete_RemovesUser()
        {
            var user = userManager.Create("TestFirstName", "TestLastName", 10);

            userManager.Delete(user.Id);

            Assert.That(userManager.GetAll().Length == 0);
        }
    }
}
