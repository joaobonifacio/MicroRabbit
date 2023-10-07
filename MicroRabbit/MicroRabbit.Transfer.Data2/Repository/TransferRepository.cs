using MicroRabbit.Transfer.Data.Context;
using MicroRabbit.Transfer.Domain.Interfaces;
using MicroRabbit.Transfer.Domain.Models;

namespace MicroRabbit.Transfer.Data.Repository
{
    public class TransferRepository : ITransferRepository
    {
        private TransferDbContext ctx;

        public TransferRepository(TransferDbContext dbContext)
        {
            this.ctx = dbContext;
        }

        IEnumerable<TransferLog> ITransferRepository.GetTransferLogs()
        {
            return ctx.TransferLogs;
        }

        public void Add(TransferLog transferLog)
        {
            ctx.TransferLogs.Add(transferLog);
            ctx.SaveChanges();
        }
    }
}
