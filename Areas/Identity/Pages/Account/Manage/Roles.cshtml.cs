using HUECL.alpha._6_0.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HUECL.alpha._6_0.Areas.Identity.Pages.Account.Manage
{
    public class RolesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public List<string> RolesList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"No es posible cargar usuario con ID: '{_userManager.GetUserId(User)}'.");
            }

            var rolesAsync = await _userManager.GetRolesAsync(user);
            if (rolesAsync == null)
            {
                return NotFound($"No existen Roles para el usuario con ID: '{_userManager.GetUserId(User)}'.");
            }

            RolesList = rolesAsync.ToList();

            return Page();
        }
    }
}
