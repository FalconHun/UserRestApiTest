using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using UserService.Models;

namespace UserService.DataAccess
{
    public class UserManagerMongoDb : IUserManager
    {
        private readonly MongoDbContext context;

        public UserManagerMongoDb(MongoDbContext context)
        {
            this.context = context;
        }

        public User Create(string firstName, string lastName, int age)
        {
            var user = new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                FirstName = firstName,
                LastName = lastName,
                Age = age
            };
            context.Users.InsertOne(user);

            return user;
        }

        public bool Delete(string id)
        {
            var user = FindUser(id);

            if (user == null)
            {
                return false;
            }

            context.Users.DeleteOne(user => user.Id.Equals(id));
            return true;
        }

        public User Get(string id)
        {
            return FindUser(id);
        }

        public User[] GetAll()
        {
            return context.Users.Find(book => true).ToList().ToArray();
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

            context.Users.ReplaceOne(user => user.Id.Equals(id), userToUpdate);

            return userToUpdate;
        }

        private User FindUser(string id)
        {
            try
            {
                return context.Users.Find(user => user.Id.Equals(id)).FirstOrDefault();
            }
            catch(FormatException)
            {
                return null;
            }
        }
    }
}
