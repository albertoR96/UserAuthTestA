using System.ComponentModel.DataAnnotations;

namespace UserAuthTestA.DTOs
{
    public class RemoveAdminRequest
    {
        [Required]
        [EmailAddress]
        public String Email { get; set; }
    }
}
