using System;

namespace SharedKernel.Exceptions;

public class DomainException : Exception
{
    private readonly bool _showStackTrace = true;
    
    public DomainException()
    { }

    public DomainException(string message)
        : base(message)
    { }

    public DomainException(string message, Exception innerException, bool showStackTrace = true)
        : base(message, innerException)
    {
        _showStackTrace = showStackTrace;
    }
    
    public override string? StackTrace => _showStackTrace ? base.StackTrace : string.Empty;
}