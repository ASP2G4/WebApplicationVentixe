namespace Authentication.Dtos
{
    public class AccountServiceResult
    {
        public bool Succeeded { get; set; }
        public string? Error { get; set; }
    }

    public class AccountServiceResult<T> : AccountServiceResult
    {
        public T? Result { get; set; }
    }
}
