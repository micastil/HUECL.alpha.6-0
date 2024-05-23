namespace HUECL.alpha._6_0.Models.CustomExceptions
{
    public class SaleDeliveryRepositoryCustomException : Exception
    {
        public SaleDeliveryRepositoryCustomException()
        {

        }

        public SaleDeliveryRepositoryCustomException(string message)
            : base(message)
        {

        }

        public SaleDeliveryRepositoryCustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
