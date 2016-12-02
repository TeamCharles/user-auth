using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using user_auth.ViewModels;
using user_auth.Data;

namespace user_auth.Models.ManageViewModels
{
    public class IndexViewModel : BaseViewModel
    {
        public bool HasPassword { get; set; }

        public IList<UserLoginInfo> Logins { get; set; }

        public string PhoneNumber { get; set; }

        public bool TwoFactor { get; set; }

        public bool BrowserRemembered { get; set; }

        public IndexViewModel(ApplicationDbContext ctx, ApplicationUser user) : base(ctx, user) {}

        public IndexViewModel() {}
    }
}
