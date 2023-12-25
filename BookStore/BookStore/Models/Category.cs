using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Display(Name = "Display Order")]
        [Range(1, 1000,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public string DisplayOrder { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Now;
        

    }
}
