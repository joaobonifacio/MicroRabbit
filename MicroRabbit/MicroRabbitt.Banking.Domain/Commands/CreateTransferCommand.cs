using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRabbitt.Banking.Domain.Commands
{
    public class CreateTransferCommand: TransferCommand
    {
        public CreateTransferCommand(int from, int to, int ammount) 
        {
            From = from; 
            To = to;
            Amount = ammount;
        }
    }
}
