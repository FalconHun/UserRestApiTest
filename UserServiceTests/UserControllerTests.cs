using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using UserService;
using UserService.Controllers;
using UserService.Models;
using UserService.DataAccess;
using System.Linq;

namespace UserServiceTests
{
    public class UserControllerTests
    {
        private ILogger<UserController> logger;
        private IUserManager userManager;
        private IRabbitMqMessagePublisher rabbitMq;
        private UserController userController;

        [SetUp]
        public void SetUp()
        {
            logger = Substitute.For<ILogger<UserController>>();
            userManager = Substitute.For<IUserManager>();
            rabbitMq = Substitute.For<IRabbitMqMessagePublisher>();

            userController = new UserController(logger, userManager, rabbitMq);
        }

        [Test]
        public void CreateUser_ReturnsUser()
        {
            var newUser = new User { FirstName = "firstName", LastName = "lastName", Age = 10 };
            userManager.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>()).Returns(newUser);

            var actionResult = userController.CreateUser(newUser).Result as CreatedAtActionResult;

            var createdUser = actionResult.Value as User;

            Assert.AreEqual(newUser.FirstName, createdUser.FirstName);
        }

        [Test]
        public void CreateUser_PublishesToRabbitMq()
        {
            var newUser = new User { FirstName = "firstName", LastName = "lastName", Age = 10 };

            userManager.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>()).Returns(newUser);

            userController.CreateUser(newUser);

            rabbitMq.Received().PublishMessage(Arg.Any<string>());
        }

        [Test]
        public void GetAll_ReturnsAllUsers()
        {
            userManager.GetAll().Returns(new[] 
            {
                new User{ Id = "TestId1"},
                new User{ Id = "TestId2"}
            });

            var result = userController.GetAll();

            Assert.AreEqual(result.Count(), 2);
            Assert.AreEqual(result.ElementAt(0).Id, "TestId1");
            Assert.AreEqual(result.ElementAt(1).Id, "TestId2");
        }

        [Test]
        public void Get_ReturnsUser()
        {
            var user = new User { Id = "TestId1", FirstName = "TestName" };

            userManager.Get("TestId1").Returns(user);                

            var result = userController.Get("TestId1");

            Assert.AreEqual(result.Value.FirstName, "TestName");
        }

        [Test]
        public void Get_UserDoesNotExist_ReturnsNotFound()
        {
            var user = new User { Id = "TestId1", FirstName = "TestName" };

            userManager.Get("TestId1").Returns(user);

            var result = userController.Get("UnknownId");

            Assert.IsTrue(result.Result is NotFoundResult);
        }


        [Test]
        public void UpdateUser_CallsUserManager()
        {
            var user = new User { Id = "TestId1", FirstName = "TestName" };

            userController.UpdateUser("TestId1", user);

            userManager.Received().Update("TestId1", user);
        }

        [Test]
        public void Update_UserDoesNotExist_ReturnsNotFound()
        {
            var result = userController.UpdateUser("TestId1", new User { FirstName = "TestName" });

            Assert.IsTrue(result.Result is NotFoundResult);
        }

        [Test]
        public void UpdateUser_ReturnUser()
        {
            var user = new User { Id = "TestId1", FirstName = "TestName" };
            userManager.Update(user.Id, user).Returns(user);

            var result = userController.UpdateUser("TestId1", user);

            Assert.AreEqual(result.Value, user);
        }

        [Test]
        public void Delete_CallsUserManager()
        {
            userController.DeleteUser("TestId");

            userManager.Received().Delete("TestId");
        }

        [Test]
        public void Delete_IdDoesNotExist_ReturnsNotFound()
        {
            userManager.Delete(Arg.Any<string>()).Returns(false);

            var result = userController.DeleteUser("TestId1");

            Assert.IsTrue(result is NotFoundResult);
        }

        public void Delete_IdExists_ReturnOk()
        {
            userManager.Delete(Arg.Any<string>()).Returns(true);

            var result = userController.DeleteUser("TestId1");

            Assert.IsTrue(result is OkResult);
        }
    }
}
