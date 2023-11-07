namespace MinimalApiBuilder;

public class MultipartBindingException : Exception
{
    public MultipartBindingException()
    { }

    public MultipartBindingException(string message) : base(message)
    { }

    public MultipartBindingException(string message, Exception innerException) : base(message, innerException)
    { }
}
