using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MicroRabbit.Infra.Bus
{
    public sealed class RabbitMQBus : IEventBus
    {
        private readonly IMediator _mediator;
        private readonly Dictionary<string, List<Type>> handlers;
        private readonly List<Type> eventTypes;

        public RabbitMQBus(IMediator mediator)
        {
            _mediator = mediator;
            handlers = new Dictionary<string, List<Type>>();
            eventTypes = new List<Type>();
        }

        public Task SendCommand<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }

        public void Publish<T>(T @event) where T : Event
        {
            //Settar connection factory
            var factory = new ConnectionFactory() { HostName = "localhost" };

            //Open connection
            using (var connection = factory.CreateConnection())
            {
                //get a channel open to create queue and publish message
                using (var channel = connection.CreateModel())
                {
                    var eventName = @event.GetType().Name;

                    //Temos um open channel
                    //Vamos declarar uma queue
                    channel.QueueDeclare(eventName, false, false, false, null);

                    //Use @event parameter as message
                    var message = JsonConvert.SerializeObject(@event);
                    var body = Encoding.UTF8.GetBytes(message);

                    //Use channel to publush the message with Basic publish
                    channel.BasicPublish("", eventName, null, body);

                }
            }
        }

        public void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            //List<Type> eventTypes é uma lista de Event de vários tipos

            //Se o event do tipo T n existir em eventTypes = new List<Type>() -> adiciona-o à lista de Event eventTypes
            if (!eventTypes.Contains(typeof(T)))
            {
                eventTypes.Add(typeof(T));
            }

            //Dictionary<string, List<Type>> handlers contém uma key (eventName) e um eventTyoe (typeof(T))

            //Se o eventName, que é o nome do Type do Event, não existir em handlers = new Dictionary<string, List<Type>>()
            //-> adiciona-o ao dictionary
            if (!handlers.ContainsKey(eventName))
            {
                handlers.Add(eventName, new List<Type>());
            }

            //Se este event tiver um handler de um tipo que já existe, lança excepção
            if (handlers[eventName].Any(s => s.GetType() == handlerType))
            {
                throw new ArgumentException($"Handler type {handlerType.Name} already is registered" +
                    $"for '{eventName}'", nameof(handlerType));
            }

            //Se estiver tudo ok associa o nome do tipo de Event ao EventHandler
            handlers[eventName].Add(handlerType);

            StartBasicConsume<T>();
        }

        private void StartBasicConsume<T>() where T : Event
        {
            //Criar connection factory
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                DispatchConsumersAsync = true,
            };

            //Criar connection
            using (var connection = factory.CreateConnection())
            {
                //get an open channel
                using (var channel = connection.CreateModel())
                {
                    //Vai ser o nome da nossa queue
                    var eventName = typeof(T).Name;

                    //Declarar queue
                    channel.QueueDeclare(eventName, false, false, false, null);

                    //Criar CONSUMER OBJECT
                    var consumer = new AsyncEventingBasicConsumer(channel);

                    //Chamar o channel para consumir mensagem
                    //Usar um delegate, um pointer para um método
                    consumer.Received += Consumer_Received;

                    channel.BasicConsume(eventName, true, consumer);

                    Console.WriteLine("Press enter to exit the consumer");
                    Console.ReadLine();
                }
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            //Temos de processar o event, fazer o kick-off do event handler
            try
            {
                //Este process event sabe q handler está subscripto a este tipo de event
                await ProcessEvent(eventName, message).ConfigureAwait(false);
            }
            catch (Exception e)
            {
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            //Vamos ver no dicionário se contém a key eventName (que é o type eo event)
            if (handlers.ContainsKey(eventName))
            {
                //Se tivemos extraímos do dictionary uma lista de <Type> a partir da key
                var subscriptions = handlers[eventName];

                foreach (var subscription in subscriptions)
                {
                    var handler = Activator.CreateInstance(subscription);

                    if (handler == null)
                    {
                        continue;
                    }
                    else
                    {
                        var eventType = eventTypes.SingleOrDefault(t => t.Name == eventName);

                        if (eventType != null)
                        {
                            var @event = JsonConvert.DeserializeObject(message, eventType);

                            var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                            //Isto faz o kick-off do handle method dentro do nosso handler, passando o event
                            //Faz o routing para o handler certo
                            await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                        }
                    }
                }
            }
        }
    }
}

