using System.Collections.Generic;
using user_auth.Models;
using user_auth.Data;

namespace user_auth.ViewModels
{
  /**
   * Class: ProductDetail
   * Purpose: Used when a user views the details for a single product
   * Author: Anulfo Ordaz/Matt Kraatz
   * Methods:
   *   Constructor ProductDetail(ctx) - Initiates a ProductDetail view model with a reference to the database context.
   */
  public class ProductDetail : BaseViewModel
  {
    public Product CurrentProduct { get; set; }

    /**
     * Purpose: Saves the reference to the database to display information about the user
     * Arguments:
     *      ctx - Database context reference
     */
    public ProductDetail(ApplicationDbContext ctx, ApplicationUser user) : base(ctx, user) { }
    public ProductDetail() { }
  }
}