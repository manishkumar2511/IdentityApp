using System.ComponentModel.DataAnnotations;

namespace IdentityAppAPI.DTO.Account
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(6,MinimumLength =3,ErrorMessage ="First Name must be atleast {2} and maximum {1} character")]
        public string firstName { get; set; }
        [Required]
        [StringLength(6, MinimumLength = 3, ErrorMessage = "Last Name must be atleast {2} and maximum {1} character")]
        public string lastName { get; set; }
        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage ="Invalid Email ")]
        public string email { get; set; }
        [Required]
        [StringLength(6, MinimumLength = 3, ErrorMessage = "password must be atleast {2} and maximum {1} character")]
        public string password { get; set; }

    }
}
