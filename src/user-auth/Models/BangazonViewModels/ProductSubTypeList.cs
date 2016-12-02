using System.Collections.Generic;
using user_auth.Models;
using user_auth.Data;

namespace user_auth.ViewModels
{
  /**
   * Class: ProductSubTypeList
   * Purpose: ViewModel for the ProductSubType views.
   * Author: Dayne Wright
   * Methods:
   *   ProductSubTypeList(BangazonContext ctx) - Constructor that calls the BaseViewModel constructor.
   */
  public class ProductSubTypeList : BaseViewModel
  {
    public IEnumerable<ProductSubType> ProductSubTypes { get; set; }
    public IEnumerable<Product> Products { get; set; }
    public ProductType ProductType { get; set; }
    public ProductSubType ProductSubType { get; set; }

    /**
     * Purpose: Saves a reference to the database context.
     * Arguments:
     *      ctx - Database context.
     * Return:
     *      An instance of the class.
     */
    public ProductSubTypeList(ApplicationDbContext ctx, ApplicationUser user) : base(ctx, user) { }
  }
}