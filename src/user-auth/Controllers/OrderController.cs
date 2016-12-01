using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using user_auth.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using user_auth.Models;
using Microsoft.AspNetCore.Routing;
using user_auth.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace user_auth.Controllers
{
    /**
     * Class: OrderController
     * Purpose: Controls logged in user's cart
     * Author: Anulfo Ordaz
     * Methods:
     *   Task<IActionResult> Final(int id) - Queries available PaymentTypes and returns a Checkout view for the current active order.
     *   IActionResult - Returns an Error view. Currently not in use.
     */
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext context;

        public OrderController(UserManager<ApplicationUser> userManager, ApplicationDbContext ctx)
        {
            _userManager = userManager;
            context = ctx;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        /**
        * Purpose: Retrieve the active products,  the payment types available for the user, and the total price to be
            displayed on the Order/Final view
        * Arguments:
        *      id - OrderId to get the current Active order
        * Return:
        *      Redirects user to Confirmation view
        */
        public async Task<IActionResult> Final([FromRoute] int id)
        {
            var user = await GetCurrentUserAsync();
            string userId = user.Id;

            //get the active products to show
            var activeProducts = await(
                from product in context.Product
                from lineItem in context.LineItem
                    .Where(lineItem => lineItem.OrderId == context.Order.SingleOrDefault(o => o.DateCompleted == null && o.UserId == userId).OrderId && lineItem.ProductId == product.ProductId && lineItem.Product.IsActive == true)
                select product).ToListAsync();

            if (activeProducts == null)
            {
                // Redirect to ProductTypes
                return RedirectToAction("Index", "ProductTypes");
            }
            //Instanciate a OrderView  to attach the ActiveProducts to it
            var model = new OrderView(context, user);
            model.ActiveProducts = activeProducts;

            //Looks for a valid PaymentTypeId
            if (id > 0)
            {
                model.selectedPaymentId = id;
            }

            foreach (var product in activeProducts)
            {
                model.TotalPrice += product.Price;
            }
            //set the model's AvailablePaymentType to feed the dropdown of PaymentTypes
            model.AvailablePaymentType =
                from PaymentType in context.PaymentType
                orderby PaymentType.Description
                where PaymentType.UserId == userId
                select new SelectListItem {
                    Text = PaymentType.Description,
                    Value = PaymentType.PaymentTypeId.ToString()
                    };

            return View(model);
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}