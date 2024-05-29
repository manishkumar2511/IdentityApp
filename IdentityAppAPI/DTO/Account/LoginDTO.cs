using System.ComponentModel.DataAnnotations;

namespace IdentityAppAPI.DTO.Account
{
    public class LoginDTO
    {
        [Required]
        public string userName { get; set; }
        [Required]
        public string password { get; set; }
    }
}
