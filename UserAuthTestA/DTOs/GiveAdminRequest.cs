using System.ComponentModel.DataAnnotations;

namespace UserAuthTestA.DTOs
{
    public class GiveAdminRequest
    {
        [Required]
        [EmailAddress]
        public String Email { get; set; }
    }
}
