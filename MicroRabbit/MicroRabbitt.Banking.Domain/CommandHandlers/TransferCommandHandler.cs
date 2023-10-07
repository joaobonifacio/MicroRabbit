using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbitt.Banking.Domain.Commands;
using MicroRabbitt.Banking.Domain.Events;

namespace MicroRabbitt.Banking.Domain.CommandHandlers
{
    public class TransferCommandHandler : IRequestHandler<CreateTransferCommand, bool>
    {
        private readonly IEventBus _bus;

        public TransferCommandHandler(IEventBus bus) 
        {
            _bus = bus;
        }

        public Task<bool> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
        {
            //Publish event to RabbitMQ
            _bus.Publish(new TransferCreatedEvent(request.To, request.From, request.Amount));

            return Task.FromResult(true);
        }
    }
}
