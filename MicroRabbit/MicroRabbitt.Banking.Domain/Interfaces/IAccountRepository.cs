using MicroRabbitt.Banking.Domain.Models;

namespace MicroRabbitt.Banking.Domain.Interfaces
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAccounts();
    }
}
