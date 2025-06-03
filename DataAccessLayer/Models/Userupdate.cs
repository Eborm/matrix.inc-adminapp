using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Userupdate
    {

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Password_new { get; set; }
    }
}
