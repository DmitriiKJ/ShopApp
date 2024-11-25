using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; } = 0;

        [Required(ErrorMessage = "Name is required")]
        [StringLength(maximumLength: 20, MinimumLength = 2, ErrorMessage = "Min: 2, Max: 20")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Order>? Orders { get; set; }
    }
}
