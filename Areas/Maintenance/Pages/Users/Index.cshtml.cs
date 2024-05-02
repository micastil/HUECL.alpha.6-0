using HUECL.alpha._6_0.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HUECL.alpha._6_0.Areas.Maintenance.Pages.Users
{
    [Authorize(Policy = "IsSuperUser")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IQueryable<ApplicationUser> userList { get; set; } = null!;


        public IActionResult OnGet()
        {
            userList = _userManager.Users;
            return Page();
        }
    }
}
