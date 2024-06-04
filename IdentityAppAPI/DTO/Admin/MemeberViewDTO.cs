using System;
using System.Collections.Generic;

namespace IdentityAppAPI.DTO.Admin
{
    public class MemberViewDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsLocked { get; set; }
        public DateTime DateCreated { get; set; }
        public List<string> Roles { get; set; }
    }
}
