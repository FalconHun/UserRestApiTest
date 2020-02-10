using System.Collections.Generic;
using System.Linq;
using UserService.Models;

namespace UserService
{
    public class UserManager : IUserManager
    {
        private List<User> users = new List<User>();

        public User Create(string firstName, string lastName, int age)
        {
            var user = new User
            {
                Id = System.Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName,
                Age = age
            };
            users.Add(user);

            return user;
        }

        public bool Delete(string id)
        {
            var user = FindUser(id);

            if (user == null)
            {
                return false;
            }
            return users.Remove(user);
        }

        public User Get(string id)
        {
            return FindUser(id);
        }

        public User[] GetAll()
        {
            return users.ToArray();
        }

        public User Update(string id, User user)
        {
            var userToUpdate = FindUser(id);
            if (userToUpdate == null)
            {
                return null;
            }

            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.Age = user.Age;

            return userToUpdate;
        }

        private User FindUser(string id)
        {
            return users.FirstOrDefault(user => user.Id.Equals(id));
        }
    }
}
