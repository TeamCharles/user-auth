using System;
using System.Linq;
using System.Threading.Tasks;
using user_auth.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using user_auth.Models;
using Microsoft.AspNetCore.Routing;
using user_auth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace user_auth.Controllers
{
    /**
     * Class: CartController
     * Purpose: Controls logged in user's cart
     * Author: Matt Hamil/Dayne Wright
     * Methods:
     *   IActionResult Index() - Queries the Active User and returns that users' Cart view.
     *   Task<IActionResult> AddToCart(int id) - Creates a new line item for the current active order. If no current order, also creates a new order. Returns Product Detail view.
     *   Task<IActionResult> DeleteLineItem(int id) - Deletes a LineItem from the current active order. Redirects to Cart view.
     *   Task<IActionResult> CompleteOrder(OrderView orderView) - Checks whether a PaymentType has been added to the current active order. If so, adds a DateCompleted to the order and saves to DB.
     *   Task<IActionResult> Confirmation(int id) - Returns a Confirmation view that lists cart information, the order# and payment method.
     *   IActionResult Error() - Returns an Error view. Currently not used.
     */
    
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext context;

        /**
         * Purpose: load the content of BangazonContext into the context variable for further use
         * Arguments:
         *      ctx - current database connection
         * Return:
         *      CartsController
         */

        public CartController(UserManager<ApplicationUser> userManager, ApplicationDbContext ctx)
        {
            _userManager = userManager;
            context = ctx;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        /**
         * Purpose: Retrieve products to a list the Cart View. If there are no Objects,
         it redirects to ProductTypes/Index, sets TotalPrice for the items in the cart.
         * Arguments:
         *      None.
         * Return:
         *      Model of the Cart Index View
         */
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var model = new CartView(context, user);

            if (model.CartProducts == null)
            {
                // Redirect to ProductTypes
                return RedirectToAction("Index", "ProductTypes");
            }

            //loop to retrieve the total price of the products
            foreach (var product in model.CartProducts)
            {
                if (product.IsActive)
                    model.TotalPrice += product.Price;
            }

            return View(model);
        }

        /**
         * Purpose: Adds a product to a user's open order
         * Arguments:
         *      id - product id that creates a line item in an active order
         * Return:
         *      Redirects user to product detail view
         */
        public async Task<IActionResult> AddToCart([FromRoute]int id)
        {
            var user = await GetCurrentUserAsync();
            string userId = user.Id;

            // Find the product
            Product productQuery = await (
                from product in context.Product
                where product.ProductId == id
                select product).SingleOrDefaultAsync();

            if (productQuery == null)
            {
                return NotFound();
            }

            // Find the user's active order
            Order openOrderQuery = await (
                from order in context.Order
                where order.UserId == userId && order.DateCompleted == null
                select order).SingleOrDefaultAsync();

            Order openOrder = null;

            // If the user does not have an open order
            if (openOrderQuery == null)
            {
                // Creating a new Order
                openOrder = new Order
                {
                    UserId = userId
                };
                context.Order.Add(openOrder);
                await context.SaveChangesAsync();
            }
            else
            {
                openOrder = openOrderQuery;
            }

            // Create a new LineItem with the ProductId and OrderId
            LineItem lineItem = new LineItem() { OrderId = openOrder.OrderId, ProductId = id };

            context.LineItem.Add(lineItem);
            await context.SaveChangesAsync();

            return RedirectToAction("Detail", new RouteValueDictionary(
                     new { controller = "Products", action = "Detail", Id = id }));
        }

        /**
        * Purpose: Delete a LineItem from an open order
        * Arguments:
        *      id - product id taken from the route that deletes an LineItem from open order
        * Return:
        *      After the file deletion is complete, it redirects to the Cart/Index view.
        */
        public async Task<IActionResult> DeleteLineItem([FromRoute]int id)
        {
            var user = await GetCurrentUserAsync();
            // Gets the Open Order based on the ActiveUser
            Order OpenOrder = await (
                from order in context.Order
                where order.UserId == user.Id && order.DateCompleted == null
                select order).SingleOrDefaultAsync();

            // Retrieve the LineItem to be deleted
            LineItem deletedItem = await context.LineItem.FirstAsync(p => p.ProductId == id && p.OrderId == OpenOrder.OrderId);


            if (deletedItem == null)
            {
                return RedirectToAction("Index", new RouteValueDictionary(
                    new { controller = "Cart", action = "Index" }));
            }

            try
            {
                context.Remove(deletedItem);
                await context.SaveChangesAsync();
                return RedirectToAction("Index", new RouteValueDictionary(
                     new { controller = "Cart", action = "Index", Id = id }));
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }

        /**
        * Purpose: Get the open order, set the payment type if the user already has one and redirectes to
            confirmation view.
        * Arguments:
        *      OrderView orderView. When the payment has been selected gives the openOrder the the PaymentTypeId
        * Return:
        *      Redirects user to product detail view
        */

        public async Task<IActionResult> CompleteOrder(OrderView orderView)
        {
            //Instanciate an ActiveUser into a User user
            var user = await GetCurrentUserAsync();
            string userId = user.Id;

            //get the open order
            Order openOrder = await (
                from order in context.Order
                where order.UserId == userId && order.DateCompleted == null
                select order).SingleOrDefaultAsync();

            if (openOrder == null)
            {
                return RedirectToAction("Buy", "ProductTypes");
            }

            //check for a selected PaymentId, otherwise redirect
            if (orderView.selectedPaymentId > 0)
            {
                openOrder.PaymentTypeId = orderView.selectedPaymentId;
            }
            else
            {
                return RedirectToAction("Final", "Order");
            }

            try
            {
                //redirects to confirmation page
                openOrder.DateCompleted = DateTime.Now;
                context.Order.Update(openOrder);
                await context.SaveChangesAsync();
                return RedirectToAction("Confirmation", new RouteValueDictionary(
                     new { controller = "Cart", action = "Confirmation", Id = openOrder.OrderId }));

            }
            catch (DbUpdateException)
            {
                throw;
            }
        }

        /**
         * Purpose: get the payment type, the lineItems, and the complete order into CartView model to be displayed
         by Confirmation page.
         * Arguments:
         *      int id - Represents the openOrder.OrderId
         * Return:
         *      Order confirmation view if an open order exists
         */

        public async Task<IActionResult> Confirmation(int id)
        {
            //get the complete order
            Order CompleteOrder = await (
                from order in context.Order
                where order.OrderId == id
                select order).SingleOrDefaultAsync();

            if (CompleteOrder == null)
            {
                return RedirectToAction("Buy", "ProductTypes");
            }

            //get the line items for the order
            var LineItems = await (
                from product in context.Product
                from lineItem in context.LineItem
                    .Where(lineItem => lineItem.OrderId == id && lineItem.ProductId == product.ProductId && lineItem.Product.IsActive == true)
                select product).ToListAsync();

            var user = await GetCurrentUserAsync();
            //Instantiation of a New CartView Model
            var model = new CartView(context, user);

            model.PaymentType = await context.PaymentType.SingleAsync(t => t.PaymentTypeId == CompleteOrder.PaymentTypeId);
            model.LineItems = LineItems;
            model.Order = CompleteOrder;

            foreach (var product in LineItems)
            {
                if (product.IsActive)
                    model.TotalPrice += product.Price;
            }

            return View(model);
        }
    }
}