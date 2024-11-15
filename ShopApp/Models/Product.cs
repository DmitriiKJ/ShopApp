using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShopApp.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; } = 0;

        [Required(ErrorMessage = "Name is required")]
        [StringLength(maximumLength: 20, MinimumLength = 2, ErrorMessage = "Min: 2, Max: 20")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 100000.00, ErrorMessage = "Min 0.01, Max: 100000.00")]
        public decimal Price { get; set; } = decimal.Zero;

        [StringLength(1024, MinimumLength = 2, ErrorMessage = "Min 2, Max: 1024")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = String.Empty;

        [JsonIgnore]
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
