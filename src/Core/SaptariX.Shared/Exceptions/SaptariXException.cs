namespace SaptariX.Shared.Exceptions;

public class SaptariXException : Exception
{
    public SaptariXException(string message)
        : base(message)
    {
    }

    public SaptariXException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
