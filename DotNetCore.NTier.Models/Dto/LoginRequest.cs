using System.ComponentModel.DataAnnotations;

namespace DotNetCore.NTier.Models.Dto
{
    public class LoginRequest
    {
        [Required]
        [Display(Name = "User Name")]
        public string? Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string? Password { get; set; }
    }
}
