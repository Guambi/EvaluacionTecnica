namespace AuthService.Exceptions
{
    public class EmptyConfigException : Exception
    {
        public EmptyConfigException()
        {
        }

        public EmptyConfigException(string message) : base(message)
        {
        }

        public EmptyConfigException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}