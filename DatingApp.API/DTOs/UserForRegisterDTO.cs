using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class UserForRegisterDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(10, MinimumLength=4, ErrorMessage="You must specify password between 4 and 10 characters.")]
        public string Password { get; set; }
        
        [Required]
        public string Gender { get; set; }
        
        [Required]
        public string KnownAs { get; set; }
        
        [Required]
        public DateTime DataOfBirth { get; set; }
        
        [Required]
        public string City { get; set; }
        
        [Required]
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public UserForRegisterDTO()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}