using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Models;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbitt.Banking.Domain.Commands;
using MicroRabbitt.Banking.Domain.Interfaces;
using MicroRabbitt.Banking.Domain.Models;

namespace MicroRabbit.Banking.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository accountRepo;

        private readonly IEventBus _bus;

        public AccountService(IAccountRepository accountRepository, IEventBus bus)
        {
            accountRepo = accountRepository;
            _bus = bus;
        }

        public IEnumerable<Account> GetAccounts()
        {
            return accountRepo.GetAccounts();
        }

        public void Transfer(AccountTransfer accountTransfer)
        {
            var createTransferCommand = new CreateTransferCommand(
              accountTransfer.FromAccount,
              accountTransfer.ToAccount,
              accountTransfer.TransferAmount);

            //É um IventBus, deve mapear para rabbitMQBus
            _bus.SendCommand(createTransferCommand);

        }
    }
}
