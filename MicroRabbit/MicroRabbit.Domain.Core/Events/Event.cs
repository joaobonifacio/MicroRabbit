namespace MicroRabbit.Domain.Core.Events
{
    public abstract class Event
    {
        public DateTime TimeSatmp { set; protected get; }

        protected Event()
        {
            TimeSatmp = DateTime.Now;
        }


    }
}
