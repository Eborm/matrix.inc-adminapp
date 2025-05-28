using System.ComponentModel.DataAnnotations;

namespace KE03_INTDEV_SE_2_Base.Models
{
    public class UserModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
