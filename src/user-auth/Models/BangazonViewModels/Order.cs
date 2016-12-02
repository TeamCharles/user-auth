using System.Collections.Generic;
using System.Linq;
using user_auth.Models;
using user_auth.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace user_auth.ViewModels
{
  /**
   * Class: OrderView
   * Purpose: Used when a user pays for the products in the user's cart
   * Author: Anulfo Ordaz
   * Methods:
   *   Constructor OrderView(ctx) - Initiates an Order view model with a reference to the database context.
   */
  public class OrderView : BaseViewModel
  {
    public decimal TotalPrice { get; set; }
    public IEnumerable<Product> ActiveProducts { get; set; }
    public IEnumerable<SelectListItem> AvailablePaymentType {get; set; }
    public int selectedPaymentId {get; set;}

    /**
     * Purpose: Saves the reference to the database to be used to display the list of products on the active order
     * Arguments:
     *      ctx - Database context reference
     */
    public OrderView(ApplicationDbContext ctx, ApplicationUser user) : base(ctx, user) { }

    public OrderView() { }
  }
}