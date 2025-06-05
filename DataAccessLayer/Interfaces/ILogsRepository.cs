using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces
{
    public interface ILogsRepository
    {
        public IEnumerable<Log> GetAllLogs();

        public IEnumerable<Log>? GetLogByID(int id);

        public void AddLog(Log log);

        public IEnumerable<Log>? GetLogByAction(string action);
    }
}
