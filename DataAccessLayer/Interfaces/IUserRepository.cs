using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IUserRepository
    {
        public User GetUserByUID(int UID);

        public User GetUserByUserName(string userName);

        public void AddUser(User user);

        public void UpdateUser(User user);

        public void DeleteUser(User user);

        public IEnumerable<User> GetAllUsers();

        public void UpdateUserPermissions(int userId, string permissions);
    }
}
