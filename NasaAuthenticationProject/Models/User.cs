using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace NasaAuthenticationProject.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        [Remote("isEmailUnique","User",ErrorMessage ="This email is already registered")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string Type { get; set; }

    }
}
