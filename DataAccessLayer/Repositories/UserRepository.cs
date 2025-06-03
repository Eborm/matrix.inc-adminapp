using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void UpdateUserPermissions(int userId, string permissions)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            user.Permissions = permissions;
            _context.SaveChanges();
        }

    }
}
