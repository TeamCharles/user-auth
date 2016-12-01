using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace user_auth.Models
{
  /**
   * Class: Order
   * Purpose: Represents the Order table in the database
   * Author: Matt Kraatz
   */
  public class Order
  {
    [Key]
    public int OrderId {get;set;}

    [Required]
    [DataType(DataType.Date)]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime DateCreated {get;set;}

    
    [DataType(DataType.Date)]
    public DateTime? DateCompleted {get;set;}

    public int UserId {get;set;}
    public ApplicationUser User {get;set;}

    public int? PaymentTypeId {get;set;}
    public PaymentType PaymentType {get;set;} 

    public ICollection<LineItem> LineItems;
  }
}