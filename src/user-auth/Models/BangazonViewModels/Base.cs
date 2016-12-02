using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Security.Claims;
using user_auth.Models;
using user_auth.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace user_auth.ViewModels
{
  /**
   * Class: BaseViewModel
   * Purpose: Contains the logged in user, products in cart, and all users from database
   * Author: Matt Kraatz/Dayne Wright/Matt Hamil
   * Methods:
   *   Constructor BaseViewModel(ctx) - database context reference
   *      this.Users - All users from database for navbar user selection.
   *      this.CartProducts - All products on the active order for the currently logged in user.
   *      this.TotalCount - Number of active products in the active order for the current user. Used for cart icon notification.
   **/
  public class BaseViewModel
  {
    public IEnumerable<SelectListItem> Users { get; set; }
    public List<Product> CartProducts { get; set; }
    protected ApplicationDbContext context;
    public int TotalCount { get;  private set; }

    /**
     * Purpose: Populates the User dropdown and the Cart product count in the nav bar
     * Arguments:
     *      ctx - Database context reference
     */
    public BaseViewModel(ApplicationDbContext ctx, ApplicationUser user)
    {
        context = ctx;
        
        if (user != null)
            {
                // For help with this LINQ query, refer to
                // https://stackoverflow.com/questions/373541/how-to-do-joins-in-linq-on-multiple-fields-in-single-join
                this.CartProducts = (
                    from product in context.Product
                    from lineItem in context.LineItem
                        .Where(lineItem => lineItem.OrderId == context.Order.SingleOrDefault(o => o.DateCompleted == null && o.UserId == user.Id).OrderId && lineItem.ProductId == product.ProductId)
                    select product).ToList();

                foreach (Product product in this.CartProducts)
                {
                    if (product.IsActive)
                        this.TotalCount += 1;
                }
            }
    }

    public BaseViewModel() { }
  }
}