using HUECL.alpha._6_0.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HUECL.alpha._6_0.Controllers.Maintenance
{
    [Authorize(Policy = "IsSuperUser")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        
        public IActionResult Index()
        {
            return View(_userManager.Users);
        }

        public IActionResult Details(string id) 
        {

            return View();
        }
    }
}
