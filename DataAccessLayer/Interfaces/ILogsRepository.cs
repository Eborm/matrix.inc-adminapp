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

        public Log? GetLogByUID(int uid);

        public void AddCustomer(Log log);

        public Log? GetLogByUserName(string userName);

        public Log? GetLogByAction(string action);
    }
}
