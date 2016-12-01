using System.Collections.Generic;
using user_auth.Models;
using user_auth.Data;

namespace user_auth.ViewModels
{
  /**
   * Class: ProductList
   * Purpose: ViewModel for the Products/Index view.
   * Author: Matt Kraatz
   * Methods:
   *   ProductList(BangazonContext ctx) - Constructor that calls the BaseViewModel constructor.
   */
  public class ProductList : BaseViewModel
  {
    public IEnumerable<Product> Products { get; set; }

    /**
     * Purpose: Saves a reference to the database context.
     * Arguments:
     *      ctx - Database context.
     * Return:
     *      An instance of the class.
     */
    public ProductList(ApplicationDbContext ctx, ApplicationUser user) : base(ctx, user) { }
  }
}