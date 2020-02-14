using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserService.Models;
using UserService.DataAccess;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private IUserManager userManager;
        private IRabbitMqMessagePublisher rabbitMqMessagePublisher;

        private readonly ILogger<UserController> logger;

        public UserController(ILogger<UserController> logger, IUserManager userManager, IRabbitMqMessagePublisher rabbitMqMessagePublisher)
        {
            this.logger = logger;
            this.rabbitMqMessagePublisher = rabbitMqMessagePublisher;
            this.userManager = userManager;
        }

        [HttpGet]
        public IEnumerable<User> GetAll()
        {
            return userManager.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<User> Get(string id)
        {
            var user = userManager.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public ActionResult<User> CreateUser(User user)
        {
            var createdUser = userManager.Create(user.FirstName, user.LastName, user.Age);

            PublishOrderCreatedMessage(createdUser.Id);

            return CreatedAtAction(nameof(CreateUser), createdUser);
        }

        [HttpPut("{id}")]
        public ActionResult<User> UpdateUser(string id, User user)
        {
            var updatedUser = userManager.Update(id, user);

            if (updatedUser == null)
            {
                return NotFound();
            }

            return updatedUser;
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(string id)
        { 
            if (!userManager.Delete(id))
            {
                return NotFound();
            }
            return Ok();
        }

        private void PublishOrderCreatedMessage(string id)
        {
            try
            {
                rabbitMqMessagePublisher.PublishMessage(id);
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Error publishing to rabbitmq: {ex.Message}");
            }

        }
    }
}
