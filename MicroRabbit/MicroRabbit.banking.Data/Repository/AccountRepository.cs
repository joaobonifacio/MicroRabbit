using MicroRabbit.Banking.Data.Context;
using MicroRabbitt.Banking.Domain.Interfaces;
using MicroRabbitt.Banking.Domain.Models;

namespace MicroRabbit.Banking.Data.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private BankingDbContext ctx;

        public AccountRepository(BankingDbContext dbContext)
        {
            this.ctx = dbContext;
        }

        public IEnumerable<Account> GetAccounts()
        {
            return ctx.Accounts;
        }
    }
}
