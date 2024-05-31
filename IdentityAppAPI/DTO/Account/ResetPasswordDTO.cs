using System.ComponentModel.DataAnnotations;

namespace IdentityAppAPI.DTO.Account
{
    public class ResetPasswordDTO
    {
       
        [Required]
        public string Token { get; set; }
        [Required]
        // [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage ="Invalid Email ")]
        public string Email { get; set; }
        [Required]
        [StringLength(12, MinimumLength = 6, ErrorMessage = "password must be atleast {2} and maximum {1} character")]
        public string NewPassword { get; set; }
    }
}
