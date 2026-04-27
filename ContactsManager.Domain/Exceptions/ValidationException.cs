namespace ContactsManager.Domain.Exceptions;

public class DomainValidationException : Exception
{
    public IEnumerable<string> Errors { get; }

    public DomainValidationException(IEnumerable<string> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
