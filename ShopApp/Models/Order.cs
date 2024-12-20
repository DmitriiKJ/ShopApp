﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShopApp.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Summ { get; set; } = decimal.Zero;

        [Required]
        public DateTime OrderDate { get; set; }

        //[ForeignKey("Customer")]
        [Required]
        public string UserId { get; set; } = string.Empty;

        //[Required]
        public Customer? Customer { get; set; }

        // Navigation property
        //[JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
