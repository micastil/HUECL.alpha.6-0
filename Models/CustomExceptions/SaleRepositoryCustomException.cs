namespace HUECL.alpha._6_0.Models
{
    public class SaleRepositoryCustomException : Exception
    {
        public SaleRepositoryCustomException()
        {
            
        }

        public SaleRepositoryCustomException(string message)
            : base(message) 
        {
            
        }

        public SaleRepositoryCustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
