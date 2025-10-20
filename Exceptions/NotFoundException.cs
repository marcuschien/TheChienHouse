namespace TheChienHouse.Exceptions
{
    public sealed class NotFoundException : Exception //This is just a sample. Probably shouldn't really use a NotFoundException. Throwing exceptions is costly (builds a stack trace) so should be really used for rare or catastrophic failures. 
    {
        public NotFoundException(string? message = null) : base(message) { }
    }
}