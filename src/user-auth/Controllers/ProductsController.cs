using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using user_auth.ViewModels;
using user_auth.Models;
using user_auth.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace user_auth.Controllers
{
    /**
     * Class: ProductsController
     * Purpose: Allows users to view, create and edit products
     * Author: Garrett/Anulfo
     * Methods:
     *   Task<IActionResult> Index() - Returns a view of all active products in the database.
     *   Task<IActionResult> Detail(int id) - Returns Detail view for an individual product.
     *          - int id: ProductId for the Product being viewed.
     *   Task<IActionResult> Edit(int id) - Returns a form view that allows you to edit an existing Product.
     *          - int id: ProductId for the Product being edited.
     *   Task<IActionResult> Edit(ProductEdit product) - Executes a Product edit within the database and returns that Product's Detail view.
     *          - ProductEdit product: ProductEdit viewmodel posted on form submission.
     *   Task<IActionResult> Create() - Returns a form view that allows a user to create a new product.
     *   Task<IActionResult> Create(ProductCreate product) - Posts a new product to the database and returns the Detail view for that Product.
     *          - ProductCreate product: ProductCreate viewmodel posted on form submission.
     *   Task<IActionResult> Delete(int id) - Sets the IsActive property on a Product to false and commits to the database. Redirects a user to the ProductTypes List page.
     *          - int id: ProductId of the Product being updated.
     */

    [Authorize]
    public class ProductsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext context;

        /**
         * Purpose: Constructor for ProductsController that passes database context to the base controller
         * Arguments:
         *      ctx - The current database connection
         * Return:
         *      ProductsController instance
         */
        public ProductsController(UserManager<ApplicationUser> userManager, ApplicationDbContext ctx)
        {
            _userManager = userManager;
            context = ctx;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        /**
         * Purpose: Returns list view of products in the database
         * Return:
         *      Returns view model of all products to user
         */
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var model = new ProductList(context, user);
            model.Products = await context.Product.Where(s => s.IsActive == true).OrderBy(s => s.Name).ToListAsync();
            return View(model);
        }

        /**
         * Purpose: Provides a view with all product details for the product with the id passed in to the method
         * Arguments:
         *      id - the product id from the route used to display the product details
         * Return:
         *      A detail model of the product with the passed in id or not found if no id was passed
         */
        public async Task<IActionResult> Detail(int? id)
        {
            // If no id was in the route, return 404
            if (id == null)
            {
                return NotFound();
            }

            var product = await context.Product
                    .Include(s => s.User)
                    .SingleOrDefaultAsync(m => m.ProductId == id);

            // If product not found, return 404
            if (product == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var model = new ProductDetail(context, user);
            model.CurrentProduct = product;
            return View(model);
        }

        /**
         * Purpose: Returns edit product form with populated product data from route id
         * Arguments:
         *      id - product id for the currently edited product
         * Return:
         *      Redirects user to product edit view if product located in database otherwise returns not found
         */
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute]int? id)
        {
            // If no id was in the route, return 404
            if (id == null)
            {
                return NotFound();
            }

            var product = await context.Product
                    .Include(s => s.User)
                    .SingleOrDefaultAsync(m => m.ProductId == id);

            var productSubTypes = context.ProductSubType
                    .OrderBy(l => l.Label)
                    .AsEnumerable()
                    .Where(t => t.ProductTypeId == product.ProductTypeId)
                    .Select(li => new SelectListItem
                    {
                        Text = li.Label,
                        Value = li.ProductSubTypeId.ToString()
                    });

            // If product not found, return 404
            if (product == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var model = new ProductEdit(context, user);
            model.CurrentProduct = product;
            model.ProductSubTypes = productSubTypes;
            return View(model);
        }

        /**
         * Purpose: Adds an updated product to the database or sends back sub type selections when triggered by onchange event
         * Arguments:
         *      product - The product submitted from the form or onchange event
         * Return:
         *      Redirects user to detail route of edited product if state is valid or updates sub type options if triggered with onchange
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEdit product)
        {
            Product originalProduct = await context.Product.SingleAsync(p => p.ProductId == product.CurrentProduct.ProductId);

            if (ModelState.IsValid && product.CurrentProduct.ProductSubTypeId > 0)
            {
                originalProduct.ProductId = product.CurrentProduct.ProductId;
                originalProduct.Price = product.CurrentProduct.Price;
                originalProduct.Description = product.CurrentProduct.Description;
                originalProduct.Name = product.CurrentProduct.Name;
                originalProduct.ProductTypeId = product.CurrentProduct.ProductTypeId;
                originalProduct.ProductSubTypeId = product.CurrentProduct.ProductSubTypeId;

                context.Entry(originalProduct).State = EntityState.Modified;

                context.Update(originalProduct);
                context.SaveChanges();


                return RedirectToAction("Detail", new RouteValueDictionary(
                     new { controller = "Products", action = "Detail", Id = originalProduct.ProductId }));
            }

            var user = await GetCurrentUserAsync();
            var model = new ProductEdit(context, user);
            model.CurrentProduct = product.CurrentProduct;

            model.ProductSubTypes = context.ProductSubType
                .OrderBy(l => l.Label)
                .AsEnumerable()
                .Where(t => t.ProductTypeId == model.CurrentProduct.ProductTypeId)
                .Select(li => new SelectListItem
                {
                    Text = li.Label,
                    Value = li.ProductSubTypeId.ToString()
                });

            return View(model);
        }
        /**
         * Purpose: Route for product creation that send back empty form and model
         * Return:
         *      Sends a model for the create form view
         */

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await GetCurrentUserAsync();
            var model = new ProductCreate(context, user);
            return View(model);
        }

        /**
         * Purpose: Adds a product to the database with the logged in users id if the product is valid or updates
         *          form with sub product types if onchange event triggers the method
         * Arguments:
         *      product - The product submitted from the from to be added to the database
         * Return:
         *      Redirects user to product detail view if product is added to the database or sends back updated view model
         *      with product sub types if triggered by onchange event of product type selection
         */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreate product)
        {
            var user = await GetCurrentUserAsync();

            if (ModelState.IsValid && product.NewProduct.ProductTypeId > 0 && product.NewProduct.ProductSubTypeId > 0)
            {
                product.NewProduct.IsActive = true;
                product.NewProduct.UserId = user.Id;
                context.Add(product.NewProduct);
                await context.SaveChangesAsync();
                return RedirectToAction("Detail", new RouteValueDictionary(
                     new { controller = "Products", action = "Detail", Id = product.NewProduct.ProductId }));
            }

            var model = new ProductCreate(context, user);
            model.NewProduct = product.NewProduct;
            if (product.NewProduct.ProductTypeId > 0)
            {
                model.ProductSubTypes = context.ProductSubType
                    .OrderBy(l => l.Label)
                    .AsEnumerable()
                    .Where(t => t.ProductTypeId == product.NewProduct.ProductTypeId)
                    .Select(li => new SelectListItem
                    {
                        Text = li.Label,
                        Value = li.ProductSubTypeId.ToString()
                    });
            }
            return View(model);
        }
        /**
         * Purpose: Sets a product as inactive
         * Arguments:
         *      id - product id of product to set as inactive
         * Return:
         *      Redirects user to list of products
         */
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            Product originalProduct = await context.Product.SingleAsync(p => p.ProductId == id);

            if (originalProduct == null)
            {
                return RedirectToAction("List", new RouteValueDictionary(
                    new { controller = "ProductSubTypes", action = "List", Id = originalProduct.ProductSubTypeId }));
            }
            else
            {

                try
                {
                    originalProduct.IsActive = false;
                    context.Update(originalProduct);
                    await context.SaveChangesAsync();
                    return RedirectToAction("Products", new RouteValueDictionary(
                        new { controller = "ProductSubTypes", action = "Products", Id = originalProduct.ProductSubTypeId }));
                }
                catch (DbUpdateException)
                {
                    throw;
                }
            }
        }
    }
}


