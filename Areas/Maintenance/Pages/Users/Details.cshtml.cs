using HUECL.alpha._6_0.Areas.Identity.Data;
using HUECL.alpha._6_0.Areas.Identity.Pages;
using HUECL.alpha._6_0.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;

namespace HUECL.alpha._6_0.Areas.Maintenance.Pages.Users
{
    [Authorize(Policy = "IsSuperUser")]
    public class DetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICustomDataProtector _dataProtector;

        public DetailsModel(
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            ICustomDataProtector customProtector)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataProtector = customProtector;
        }

        [BindProperty]
        public InputModel Input { get; set; } = null!;

        public string userId { get; set; } = string.Empty;
        public string userName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public class InputModel
        {
            [Display(Name = "Permiso Lectura")]
            public bool CanRead { get; set; }

            [Display(Name = "Permiso Escritura")]
            public bool CanWrite { get; set; }

            [Display(Name = "Permiso Borrar")]
            public bool CanDelete { get; set; }

            [Display(Name = "Rol")]
            public string roleId { get; set; } = string.Empty ;
        }

        public SelectList roleList { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try 
            {
                userId = _dataProtector.Unprotect(id);

                var appUser = await _userManager.FindByIdAsync(userId);

                if (appUser != null) 
                {
                    var roleListUser = await _userManager.GetRolesAsync(appUser);

                    var role = await _roleManager.FindByNameAsync(roleListUser.FirstOrDefault());

                    roleList = new SelectList(_roleManager.Roles, "Id", "Name");

                    userName = appUser.UserName;
                    Name = appUser.Name;
                    LastName = appUser.LastName;

                    IList<Claim> _claims = await _userManager.GetClaimsAsync(appUser);

                    Input = new InputModel();

                    var _read = _claims.FirstOrDefault(r => r.Type == GlobalPermissionType.CanRead);
                    if (_read == null) { Input.CanRead = false; }
                    else { Input.CanRead = true; }

                    var _write = _claims.FirstOrDefault(r => r.Type == GlobalPermissionType.CanWrite);
                    if (_write == null) { Input.CanWrite = false; }
                    else { Input.CanWrite = true; }

                    var _delete = _claims.FirstOrDefault(r => r.Type == GlobalPermissionType.CanDelete);
                    if (_delete == null) { Input.CanDelete = false; }
                    else { Input.CanDelete = true; }

                    Input.roleId = role.Id;

                    return Page();
                }

                return LocalRedirect(Url.Content("~/"));
                
            }
            catch(CryptographicException ex) 
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
