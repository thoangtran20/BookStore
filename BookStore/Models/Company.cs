using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "PhoneNumber must be exactly 10 characters.")]
        public string? PhoneNumber { get; set; }

    }
}
