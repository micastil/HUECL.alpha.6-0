namespace HUECL.alpha._6_0.Models
{
    public class ProductRepositoryCustomException : Exception
    {
        public ProductRepositoryCustomException()
        {

        }

        public ProductRepositoryCustomException(string message)
            : base(message)
        {

        }

        public ProductRepositoryCustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
