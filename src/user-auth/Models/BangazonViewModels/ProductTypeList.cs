using System.Collections.Generic;
using user_auth.Models;
using user_auth.Data;

namespace user_auth.ViewModels
{
  /**
   * Class: ProductTypeList
   * Purpose: ViewModel for the ProductType views.
   * Author: Matt Kraatz
   * Methods:
   *   ProductTypeList(BangazonContext ctx) - Constructor that calls the BaseViewModel constructor.
   */
  public class ProductTypeList : BaseViewModel
  {
    public IEnumerable<ProductType> ProductTypes { get; set; }

    /**
     * Purpose: Saves a reference to the database context.
     * Arguments:
     *      ctx - Database context.
     * Return:
     *      An instance of the class.
     */
    public ProductTypeList(ApplicationDbContext ctx, ApplicationUser user) : base(ctx, user) { }
  }
}