using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace DataAccessLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MatrixIncDbContext _context;

        public UserRepository(MatrixIncDbContext context)
        {
            _context = context;
        }

        public User GetUserByUID(int UID)
        {
            try
            {
                return _context.Users
                    .FirstOrDefault(u => u.Id == UID)
                    ?? throw new KeyNotFoundException($"User with ID {UID} not found.");
            }
            catch
            {
                return null;
            }
        }
        
        public User GetUserByUserName(string userName)
        {
            try
            {
                return _context.Users
                    .FirstOrDefault(u => u.UserName == userName)
                    ?? throw new KeyNotFoundException($"User with username {userName} not found.");
            }
            catch { return null; }
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();

        }

        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users;
        }

        public void UpdateUserPermissions(int userId, int permissions)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            user.Permissions = permissions;
            _context.SaveChanges();
        }

        public string EncryptPassword(string password)
        {
            byte[] bytepassword = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] fixedSalt = Encoding.UTF8.GetBytes("YourFixedSaltValue1");
            var passwordBytes = new Rfc2898DeriveBytes(password, fixedSalt, 100000);
            Aes encryptor = Aes.Create();
            encryptor.Key = passwordBytes.GetBytes(32);
            encryptor.IV = passwordBytes.GetBytes(16);
            using(MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytepassword, 0, bytepassword.Length);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
