namespace HUECL.alpha._6_0.Models.CustomExceptions
{
    public class ProjectRepositoryCustomException : Exception
    {
        public ProjectRepositoryCustomException()
        {

        }

        public ProjectRepositoryCustomException(string message)
            : base(message)
        {

        }

        public ProjectRepositoryCustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
