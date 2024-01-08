using EFOnvifAPI.Models;

namespace OnvifAPI.Interfaces
{
    public interface IUserService
    {
        //IEnumerable<User> GetAll();
        //User Add(User newUser);
        //User Update(User updateUser);
        //bool Delete(int userId);
        //User GetById(int userid);

        User? Login(string username,string password);

        User? ChangePassword(string username, string password, string newpassword);
    }
}
