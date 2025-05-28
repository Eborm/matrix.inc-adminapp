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
    internal class UserRepository : IUserRepository
    {
        private readonly MatrixIncDbContext _context;

        public UserRepository(MatrixIncDbContext context)
        {
            _context = context;
        }

        public User GetUserByname(string UserName)
        {
            return null;
        }

        public void AddUser(User user)
        {

        }

        public void UpdateUser(User user)
        {

        }

        public void DeleteUser(User user)
        {

        }
    }
}
