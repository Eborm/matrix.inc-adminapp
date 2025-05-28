using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    internal interface IUserRepository
    {
        public User GetUserByname(string UserName);

        public void AddUser(User user);

        public void UpdateUser(User user);

        public void DeleteUser(User user);
    }
}
