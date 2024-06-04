using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityAppAPI.Model
{
    public class User:IdentityUser
    {
        internal string FirstName;

        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
        public DateTime dateCreated {  get; set; }
    }
}
