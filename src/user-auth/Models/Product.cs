using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace user_auth.Models
{
  /**
   * Class: Product
   * Purpose: Represents the Product table in the database
   * Author: Matt Kraatz
   */
  public class Product
  {
    [Key]
    public int ProductId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime DateCreated { get; set; }

    [Required]
    [StringLength(255)]
    public string Description { get; set; }

    [Required]
    [StringLength(55)]
    public string Name { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int ProductSubTypeId { get; set; }
    public ProductSubType ProductSubType { get; set; }

    [Required]
    public int ProductTypeId { get; set; }
    public ProductType ProductType { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [Required]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public ICollection<LineItem> LineItems;
  }
}