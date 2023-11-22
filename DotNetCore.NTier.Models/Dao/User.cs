using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetCore.NTier.Models.Dao
{
    public class User
    {
        [Key]
        [Display(Name ="User Id")]
        public int Id { get; set; }

        [Display(Name = "User Name")]
        public string Username { get; set; }

        [JsonIgnore]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Create Date")]
        public DateTime CreateDate { get; set; }
    }
}
