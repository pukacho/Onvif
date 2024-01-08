using EFOnvifAPI.Models;
using Microsoft.IdentityModel.Tokens;
using OnvifAPI.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace OnvifAPI.Service
{
    public class UserService : IUserService
    {

        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
            if (_userRepository.GetAll().IsNullOrEmpty())
            {
                byte[] hasPass = PassHash("Admin");

                _userRepository.Add(new User()
                {
                    Name = "Admin",
                    PasswordHash = hasPass,
                    CreateDate = DateTime.Now,

                });
            }
        }

        private static byte[] PassHash(string psaa)
        {
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(psaa);
            byte[] hasPass = sha512.ComputeHash(bytes);
            return hasPass;
        }

        public User? Login(string username, string password)
        {
            var user= _userRepository.GetAll().FirstOrDefault(u=>u.Name.ToLower() == username.ToLower());
            if (user == null)
                return null;

            if (user.PasswordHash.SequenceEqual(PassHash(password)))
            {
                return user;
            }

            return null;
        }

        public User? ChangePassword(string username, string password, string newpassword)
        {
            var user = _userRepository.GetAll().FirstOrDefault(u => u.Name == username);
            if (user == null)
                return null;

            if (user.PasswordHash.Equals(PassHash(password)))
            {
                user.PasswordHash = PassHash(newpassword);
                _userRepository.Update(user);
                return user;
            }

            return null;
        }
    }
}
