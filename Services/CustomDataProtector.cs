using DocumentFormat.OpenXml.Office2010.Excel;
using HUECL.alpha._6_0.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace HUECL.alpha._6_0.Services
{
    public class CustomDataProtector : ICustomDataProtector
    {
        private readonly IDataProtector _protector;

        public CustomDataProtector(IDataProtectionProvider dataProtectionProvider)
        {
            _protector = dataProtectionProvider.CreateProtector("CustomProtector");
        }
        public string Protect(string value)
        {
            return _protector.Protect(value);
        }

        public string Unprotect(string protectedValue)
        {
            return _protector.Unprotect(protectedValue);
        }
    }
}
