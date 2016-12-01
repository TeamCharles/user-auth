using System.Collections.Generic;
using System.Linq;
using user_auth.Models;
using user_auth.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace user_auth.ViewModels
{
  /**
   * Class: CartView
   * Purpose: Used when displaying a user's cart
   * Author: Garrett Vangilder
   * Methods:
   *   Constructor CartView(ctx) - Initiates a cart view model with a reference to the database context.
   */
  public class CartView : BaseViewModel
  {
    public decimal TotalPrice { get; set; }
    public Order Order { get; set; }
    public PaymentType PaymentType { get; set; }
    public IEnumerable<Product> LineItems { get; set; }

    /**
     * Purpose: Saves the reference to the database to be used to display the list of products on the active order
     * Arguments:
     *      ctx - Database context reference
     */
    public CartView(ApplicationDbContext ctx, ApplicationUser user) : base(ctx, user) { }
    public CartView() { }
  }
}