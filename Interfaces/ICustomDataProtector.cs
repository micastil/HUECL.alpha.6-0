namespace HUECL.alpha._6_0.Interfaces
{
    public interface ICustomDataProtector
    {
        string Protect(string value);
        string Unprotect(string protectedValue);
    }
}
