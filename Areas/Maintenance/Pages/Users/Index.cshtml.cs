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

        //public IQueryable<ApplicationUser> userList { get; set; } = null!;
        public List<UserListViewModel> UserList { get; set; } = null!;

        public async Task<IActionResult> OnGet()
        {
            
            UserList = new List<UserListViewModel>();

            var _users = _userManager.Users.ToList();

            if(_users != null) 
            {
                foreach (var item in _users)
                {
                    UserListViewModel model = new()
                    { 
                        ApplicationUser = item,
                        UserRoles = await _userManager.GetRolesAsync(item)
                    };

                    UserList.Add(model);
                }
                return Page();
            }

            return NotFound();
        }
    }

    public class UserListViewModel 
    {
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public IList<string> UserRoles { get; set; } = null!;
    }
}
