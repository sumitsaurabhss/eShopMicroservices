namespace CartAPI.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string name, object key) : base($"{name} was not found for {key}.")
        {

        }
    }
}
