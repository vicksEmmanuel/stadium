using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Models
{
    public class UsersDto {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Username { get; set; }
        [Required]
        [Range(typeof(string), "Admin", "User", 
            ErrorMessage = "Value {0} must be between {1} and {2}")
        ]
        public string UserType { get; set; }
        public string ImageName { get; set; }
        
    }
}