using System.ComponentModel.DataAnnotations;

namespace DotNetCore.NTier.Models.Dto
{
    public class LoginResponse
    {
        [Display(Name = "User Id")]
        public int Id { get; set; }

        [Display(Name = "User Name")]
        public string? Username { get; set; }

        [Display(Name = "Jwt Token")]
        public string? JwtToken { get; set; }
    }
}
