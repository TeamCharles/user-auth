using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace user_auth.Models
{
  /**
   * Class: ProductSubType
   * Purpose: Represents the ProductSubType table in the database
   * Author: Dayne Wright
   */
  public class ProductSubType
  {
      [Key]
      public int ProductSubTypeId { get;set; }

      [Required]
      [DataType(DataType.Date)]
      [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
      public DateTime DateCreated {get;set;}

      [Required]
      [StringLength(20)]
      public string Label { get; set; }

      [Required]
      public int ProductTypeId { get; set; }
      public int Quantity { get; set; }
  }
}