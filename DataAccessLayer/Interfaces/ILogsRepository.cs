using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces
{
    // Interface voor logsrepository
    // Definieert methoden voor het beheren en ophalen van logregels
    public interface ILogsRepository
    {
        // Haalt alle logregels op
        public IEnumerable<Log> GetAllLogs();

        // Haalt logregels op basis van een ID
        public IEnumerable<Log>? GetLogByID(int id);

        // Voegt een nieuwe logregel toe aan de database (asynchroon)
        public Task AddLog(Log log);

        // Haalt logregels op basis van een actie
        public IEnumerable<Log>? GetLogByAction(string action);
    }
}
