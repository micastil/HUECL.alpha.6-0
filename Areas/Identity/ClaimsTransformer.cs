using HUECL.alpha._6_0.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HUECL.alpha._6_0.Areas.Identity
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly IUserStore<ApplicationUser> _UserStore;

        public ClaimsTransformer(IUserStore<ApplicationUser> userStore)
        {
            _UserStore = userStore;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var clonedPrincipal = principal.Clone();

            if(clonedPrincipal.Identity == null) 
            {
                return clonedPrincipal;
            }

            var identity = (ClaimsIdentity)clonedPrincipal.Identity;
            
            var existingClaim = identity.Claims.FirstOrDefault(
                c => c.Type == GlobalClaimTypes.Name
                );
            if(existingClaim != null) { return clonedPrincipal; }

            var nameIdClaim = identity.Claims.FirstOrDefault( c => c.Type == ClaimTypes.NameIdentifier );
            if(nameIdClaim == null) { return clonedPrincipal; }

            var user = await _UserStore.FindByIdAsync(nameIdClaim.Value, CancellationToken.None);
            //if (user != null)
            //{
            //    identity.AddClaim(new Claim(GlobalClaimTypes.Name, user.Name));
            //}

            return clonedPrincipal;
        }
    }
}
