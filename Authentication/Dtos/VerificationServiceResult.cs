namespace Authentication.Dtos
{
    public class VerificationServiceResult
    {
        public bool Succeeded { get; set; }
        public string? Error { get; set; }
    }

    public class VerificationServiceResult<T> : VerificationServiceResult
    {
        public T? Result { get; set; }
    }
}
