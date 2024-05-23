using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using HUECL.alpha._6_0.Areas.Identity.Data;
using HUECL.alpha._6_0.Areas.Identity.Pages;
using HUECL.alpha._6_0.Interfaces;
using HUECL.alpha._6_0.Models;
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

        public string userName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public SelectList roleList { get; set; } = null!;
        
        public class InputModel
        {
            [Display(Name = "Permiso Lectura")]
            public bool CanRead { get; set; }

            [Display(Name = "Permiso Escritura")]
            public bool CanWrite { get; set; }

            [Display(Name = "Permiso Borrar")]
            public bool CanDelete { get; set; }

            [Display(Name = "Rol")]
            public string roleId { get; set; } = string.Empty;

            public string token { get; set; } =string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try 
            {
                Input = new InputModel();
                Input.token = id;

                var unprotectedToken = _dataProtector.Unprotect(id);
                
                var appUser = await _userManager.FindByIdAsync(unprotectedToken);

                if (appUser != null) 
                {
                    var roleListUser = await _userManager.GetRolesAsync(appUser);

                    var role = await _roleManager.FindByNameAsync(roleListUser.FirstOrDefault());

                    roleList = new SelectList(_roleManager.Roles, "Id", "Name");

                    userName = appUser.UserName;
                    Name = appUser.Name;
                    LastName = appUser.LastName;

                    IList<Claim> _claims = await _userManager.GetClaimsAsync(appUser);

                    var _read = _claims.FirstOrDefault(r => r.Type == GlobalPermissionType.CanRead);
                    if (_read != null && _read.Value == "True") 
                    { Input.CanRead = true; }
                    else 
                    { Input.CanRead = false; }

                    var _write = _claims.FirstOrDefault(r => r.Type == GlobalPermissionType.CanWrite);
                    if (_write != null && _write.Value == "True") 
                    { Input.CanWrite = true; }
                    else 
                    { Input.CanWrite = false; }

                    var _delete = _claims.FirstOrDefault(r => r.Type == GlobalPermissionType.CanDelete);
                    if (_delete != null && _delete.Value == "True") 
                    { Input.CanDelete = true; }
                    else 
                    { Input.CanDelete = false; }

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

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null) 
        {
            try
            {
                returnUrl ??= Url.Content("/Maintenance/Users");

                var userId = _dataProtector.Unprotect(Input.token);

                var detailUser = await _userManager.FindByIdAsync(userId);

                if (detailUser != null) 
                {
                    var claimsList = await _userManager.GetClaimsAsync(detailUser);
                    var removeClaimsResult = await _userManager.RemoveClaimsAsync(detailUser, claimsList);
                    if(removeClaimsResult.Succeeded) 
                    {
                        await _userManager.AddClaimAsync(detailUser, new Claim(GlobalPermissionType.CanRead, Input.CanRead.ToString()));
                        await _userManager.AddClaimAsync(detailUser, new Claim(GlobalPermissionType.CanWrite, Input.CanWrite.ToString()));
                        await _userManager.AddClaimAsync(detailUser, new Claim(GlobalPermissionType.CanDelete, Input.CanDelete.ToString()));

                        var roleListUser = await _userManager.GetRolesAsync(detailUser);
                        var role = await _roleManager.FindByNameAsync(roleListUser.FirstOrDefault());
                        var remveRoleResult = await _userManager.RemoveFromRoleAsync(detailUser, role.Name);
                        if (remveRoleResult.Succeeded)
                        {
                            var newRole = await _roleManager.FindByIdAsync(Input.roleId);
                            var newRoleResult = await _userManager.AddToRoleAsync(detailUser, newRole.Name);
                            if (newRoleResult.Succeeded)
                            {
                                return LocalRedirect(returnUrl);
                            }
                            else 
                            { 
                                return BadRequest("No fue posible editar el Rol del Usuario");
                            }
                        }
                        else 
                        { 
                            return BadRequest("No fue posible editar el Rol del Usuario");
                        }
                    }
                    return BadRequest("No fue posible editar los Permisos del Usuario");
                }

                return BadRequest("No fue posible encontrar el Usuario");
            }
            catch (CryptographicException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
