
namespace Orders.Domain.Exceptions
{
    public class ExceptionBase : Exception
    {
        public string customMessage { get; set; }

        public ExceptionBase()
        {
        }

        public ExceptionBase(string request, string message) : base(message)
        {
        }
        
        public ExceptionBase(string message) : base(message)
        {
            customMessage = message;
        }
        
        public ExceptionBase(string message, Exception exception) : base(message)
        {
            customMessage = message + exception.Message;
        }
    }
}
