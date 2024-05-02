using HUECL.alpha._6_0.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HUECL.alpha._6_0.Areas.Maintenance.Pages.Users
{
    [Authorize(Policy = "IsSuperUser")]
    public class DetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string userId { get; set; } = string.Empty;

        public void OnGet(string id)
        {

            userId = id;
        }
    }
}
