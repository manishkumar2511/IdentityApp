using System.ComponentModel.DataAnnotations;

namespace IdentityAppAPI.DTO.Account
{
    public class ConfirmEmailDTO
    {
        [Required]
        public string Token { get; set; }

        [Required]
       // [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid Email ")]
        public string Email { get; set; }
    }
}
