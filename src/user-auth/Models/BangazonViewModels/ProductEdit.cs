using System.Collections.Generic;
using System.Linq;
using user_auth.Models;
using user_auth.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace user_auth.ViewModels
{
  /**
   * Class: ProductEdit
   * Purpose: ViewModel for the Products/Edit view.
   * Author: Matt Kraatz
   * Methods:
   *   ProductList(BangazonContext ctx) - Constructor that calls the BaseViewModel constructor and loads ProductTypes.
   */
  public class ProductEdit : BaseViewModel
  {
    public Product CurrentProduct { get; set; }
    public IEnumerable<SelectListItem> ProductTypes { get; set; }
    public IEnumerable<SelectListItem> ProductSubTypes { get; set; }

    /**
     * Purpose: Saves a reference to the database context and loads all ProductTypes to be available to the form.
     * Arguments:
     *      ctx - Database context.
     * Return:
     *      An instance of the class.
     */
    public ProductEdit(ApplicationDbContext ctx, ApplicationUser user) : base(ctx, user)
    {
      ProductTypes = context.ProductType
                .OrderBy(l => l.Label)
                .AsEnumerable()
                .Select(li => new SelectListItem {
                    Text = li.Label,
                    Value = li.ProductTypeId.ToString()
                    });
    }
    public ProductEdit() { }
  }
}