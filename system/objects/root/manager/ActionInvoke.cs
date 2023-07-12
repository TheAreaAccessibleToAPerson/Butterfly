namespace Butterfly.system.objects.root.manager
{
    public sealed class ActionInvoke : Controller.LocalField<uint>
    {
        public const string TEG = "ActionInvokeTeg";

        private collection.Value<System.Action> _values
            = new collection.Value<System.Action>();

        private readonly object _locker = new object();

        public bool IsDestroy = false;

        public void Add(System.Action value)
        {
            if (StateInformation.IsStart && IsDestroy == false)
            {
                _values.Add(value);
            }
            else if (IsDestroy == false)
            {
                value.Invoke();
            }
            else
            {
                Process();

                Task.Run(value.Invoke);
            }
        }

        void Construction()
        {
            add_teg(TEG);

            add_event("SYSTEM", Process);
        }

        void Start()
        {
            //add_thread("", Process, 2, Thread.Priority.Highest);
        }

        void Process()
        {
            if (_values.TryExtractAll(out System.Action[] actions))
            {
                foreach (System.Action action in actions)
                {
                    action.Invoke();
                }
            }
        }
    }
}