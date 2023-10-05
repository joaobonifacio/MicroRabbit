namespace MicroRabbit.Domain.Core.Events
{
    public abstract class Event
    {
        public DateTime TimeStamp { set; protected get; }

        protected Event()
        {
            TimeStamp = DateTime.Now;
        }
    }
}
