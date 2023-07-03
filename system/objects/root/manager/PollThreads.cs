using System.Threading;
namespace Butterfly
{
    public class Events
    {
        public class PollInformation
        {
            public string Name { get; init; }
            public uint TimeDelay { get; init; }
            public Thread.Priority ThreadPriority { get; init; }
            public bool IsDestroy { get; init; } = true;
            public uint Size { get; init; }
        }

        private static Dictionary<string, PollInformation> _values = new Dictionary<string, PollInformation>();

        public static void AddSetting(string name, uint timeDelay, uint Size,
            bool isDestroy, Thread.Priority threadPriority)
            => _values.Add
                (name, new PollInformation()
                {
                    Name = name,
                    TimeDelay = timeDelay,
                    Size = Size,
                    IsDestroy = isDestroy,
                    ThreadPriority = threadPriority
                });

        public static bool GetSetting(string name, out PollInformation information)
            => _values.TryGetValue(name, out information);
    }
}

namespace Butterfly.system.objects.root.manager
{
    public sealed class PollThreads : Controller.LocalField<EventSetting[]>
    {
        private readonly Dictionary<string, Poll> _values
            = new Dictionary<string, Poll>();
        private readonly object _locker = new object();

        private readonly Dictionary<string, EventSetting> _eventSettings
            = new Dictionary<string, EventSetting>();

        void Construction()
        {
            foreach (EventSetting eventSetting in Field)
            {
                if (_eventSettings.Keys.Contains(eventSetting.Name))
                {
                    Exception($"Вы дважды указали настройки для одного и тогоже события {eventSetting.Name}");
                }
                else
                {
                    _eventSettings.Add(eventSetting.Name, eventSetting);
                }
            }
        }

        /// <summary>
        /// Передаем в нужные потоки регистрационые билеты.
        /// </summary>
        public void Add(main.SubscribePollTicket[] tickets)
        {
            lock (_locker)
            {
                for (int i = 0; i < tickets.Length; i++)
                {
                    if (_values.TryGetValue(tickets[i].Name, out Poll poll))
                    {
                        poll.AddTicket(tickets[i]);
                    }
                    else
                    {
                        if (_eventSettings.TryGetValue(tickets[i].Name, out EventSetting eventSetting))
                        {
                            Poll newPoll = obj<Poll>(eventSetting.Name,
                                    new poll.Fields()
                                    {
                                        ID = UniqueID++,
                                        Name = eventSetting.Name,
                                        Size = eventSetting.Size,
                                        TimeDelay = eventSetting.TimeDelay,
                                        ThreadPriority = eventSetting.Priority,
                                        IsDestroy = eventSetting.IsDestroy,
                                        Destroy = Destroy
                                    });
                            
                            newPoll.DefineSize(eventSetting.Size);

                            newPoll.AddTicket(tickets[i]);

                            _values.Add(tickets[i].Name, newPoll);
                        }
                        else
                            Exception($"Вы не задали настройки для события {tickets[i].Name}.");
                    }
                }
            }
        }

        /// <summary>
        /// Передаем в нужные потоки регистрационые билеты.
        /// </summary>
        public void Add(main.UnsubscribePollTicket[] tickets)
        {
            lock (_locker)
            {
                for (int i = 0; i < tickets.Length; i++)
                {
                    if (_values.TryGetValue(tickets[i].Name, out Poll poll))
                    {
                        poll.AddTicket(tickets[i]);
                    }
                }
            }
        }

        private bool Destroy(Poll poll)
        {
            return false;
        }

        private ulong UniqueID = 0;
    }
}