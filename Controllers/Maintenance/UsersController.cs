using HUECL.alpha._6_0.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HUECL.alpha._6_0.Controllers.Maintenance
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Policy = "IsSuperUser")]
        public IActionResult Index()
        {
            return View(_userManager.Users);
        }

        [Authorize(Policy = "IsSuperUser")]
        [HttpPost]
        public async Task<IActionResult> GetUsers() 
        {
            return Ok();
        }
    }
}
