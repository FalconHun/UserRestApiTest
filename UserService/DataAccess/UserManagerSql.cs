using System.Linq;
using UserService.Models;

namespace UserService.DataAccess
{
    public class UserManagerSql : IUserManager
    {
        private readonly SqlDbContext context;

        public UserManagerSql(SqlDbContext context)
        {
            this.context = context;
        }

        public User Create(string firstName, string lastName, int age)
        {
            var user = new User
            {
                Id = System.Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName,
                Age = age
            };
            context.Users.Add(user);
            SaveChanges();

            return user;
        }

        public bool Delete(string id)
        {
            var user = FindUser(id);

            if (user == null)
            {
                return false;
            }
            context.Users.Remove(user);
            SaveChanges();
            return true;
        }

        public User Get(string id)
        {
            return FindUser(id);
        }

        public User[] GetAll()
        {
            return context.Users.ToArray();
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

            SaveChanges();

            return userToUpdate;
        }

        private User FindUser(string id)
        {
            return context.Users.FirstOrDefault(user => user.Id.Equals(id));
        }

        private void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
