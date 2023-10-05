using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbitt.Banking.Domain.Interfaces;
using MicroRabbitt.Banking.Domain.Models;

namespace MicroRabbit.Banking.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository accountRepo;

        public AccountService(IAccountRepository accountRepository)
        {
            accountRepo = accountRepository;
        }

        public IEnumerable<Account> GetAccounts()
        {
            return accountRepo.GetAccounts();
        }
    }
}
