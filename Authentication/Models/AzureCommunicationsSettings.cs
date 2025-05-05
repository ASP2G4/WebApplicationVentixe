namespace Authentication.Models
{
    public class AzureCommunicationsSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string SenderAddress { get; set; } = null!;
        public string SenderName { get; set; } = null!;
    }
}
