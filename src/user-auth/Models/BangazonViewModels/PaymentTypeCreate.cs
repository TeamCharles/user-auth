using System.Collections.Generic;
using System.Linq;
using user_auth.Models;
using user_auth.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace user_auth.ViewModels
{
  /**
   * Class: PaymentTypeView
   * Purpose: Used when a customer creates a new payment type
   * Author: Anulfo Ordaz
   * Methods:
   *   Constructor OrderView(ctx) - Initiates a PaymentTypeView view model with a reference to the database context.
   */
  public class PaymentTypeView : BaseViewModel
  {
    public PaymentType NewPaymentType { get; set; }

    /**
     * Purpose: Saves the reference to the database to display information about the active order
     * Arguments:
     *      ctx - Database context reference
     */
    public PaymentTypeView(ApplicationDbContext ctx, ApplicationUser user) : base(ctx, user) { }
    public PaymentTypeView() { }
  }
}