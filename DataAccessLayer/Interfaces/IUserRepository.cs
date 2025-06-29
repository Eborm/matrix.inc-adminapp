using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    // Interface voor gebruikersrepository
    // Definieert methoden voor het beheren van gebruikers in de database
    public interface IUserRepository
    {
        // Haalt een gebruiker op basis van het unieke ID
        public User GetUserByUID(int UID);

        // Haalt een gebruiker op basis van de gebruikersnaam
        public User GetUserByUserName(string userName);

        // Voegt een nieuwe gebruiker toe aan de database
        public void AddUser(User user);

        // Werkt een bestaande gebruiker bij
        public void UpdateUser(User user);

        // Verwijdert een gebruiker uit de database
        public void DeleteUser(User user);

        // Haalt alle gebruikers op
        public IEnumerable<User> GetAllUsers();

        // Werkt de permissies van een gebruiker bij
        public void UpdateUserPermissions(int userId, int permissions);

        // Versleutelt een wachtwoord voor veilige opslag
        public string EncryptPassword(string password);
    }
}
