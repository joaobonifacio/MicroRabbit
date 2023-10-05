namespace MicroRabbitt.Banking.Domain.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountType { get; set; } = "";
        public int AccountBalance { get; set; }
        public string AccountName { get; set; } = "";
    }
}
