using UserService.Models;

namespace UserService.DataAccess
{
    public interface IUserManager
    {
        User[] GetAll();
        User Get(string id);
        User Create(string firstName, string lastName, int age);
        User Update(string id, User user);
        bool Delete(string id);
    }
}
