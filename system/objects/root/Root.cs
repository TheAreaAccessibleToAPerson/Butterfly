namespace Butterfly.system.objects.root
{
    public class Object<ObjectType> : Controller.Board, IManager, description.ILife
        where ObjectType : main.Object, new()
    {
        /// <summary>
        /// Имя системы. 
        /// </summary>
        private string Name;

        /// <summary>
        /// Время необходимое на запуск системы. 
        /// </summary>
        private uint StartingTime;

        /// <summary>
        /// Настройки для событий. 
        /// </summary>
        private EventSetting[] EventSettings;

        private manager.ActionInvoke ActionInvokeManager;
        private manager.PollThreads PollThreads;

        void Construction()
        {
            PollThreads = obj<manager.PollThreads>
                ("PollThreads", EventSettings);

            ActionInvokeManager = obj<manager.ActionInvoke>
                ("ActionInvoke", StartingTime);
        }

        void Start() => obj<ObjectType>(Name);

        void IManager.ActionInvoke(Action action) 
            => ActionInvokeManager.Add(action);

        void IManager.AddSubscribeTickets(main.SubscribePollTicket[] tickets)
            => PollThreads.Add(tickets);

        void IManager.AddUnsubscribeTickets(main.UnsubscribePollTicket[] tickets)
            => PollThreads.Add(tickets);

        void description.ILife.Run(Gudron.Settings s)
        {
            Name = s.Name;
            StartingTime = s.StartingTime;

            EventSettings = s.Events;
            Hellper.ExpendArray(ref EventSettings, s.SystemEvent);

            ((main.description.IDOM)this).NodeDefine
                ("", 0, new ulong[0], this, this, this, new Dictionary<string, object>());

            ((main.description.IDOM)this).CreatingNode();

            global::System.Threading.Thread.CurrentThread.Priority = global::System.Threading.ThreadPriority.Lowest;

            while (true)
            {
                global::System.GC.Collect();

                if (StateInformation.IsStop) return;

                sleep (2000);
            }
        }
    }
}