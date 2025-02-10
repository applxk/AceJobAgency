using Microsoft.AspNetCore.Identity;
using System;

namespace AceJobAgency.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string NRIC { get; set; } // Store encrypted
        public DateTime DateOfBirth { get; set; }
        public string WhoAmI { get; set; }
        public string ResumePath { get; set; } // File path of uploaded resume

        public string CurrentSessionId { get; set; }
    }
}
