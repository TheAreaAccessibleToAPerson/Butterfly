namespace Butterfly.system.objects.root.manager
{
    public sealed class ActionInvoke : Controller.LocalField<uint>
    {
        private readonly collection.Value<Action> _values
            = new collection.Value<Action>();

        public void Add(Action value)
            => _values.Add(value);

        void Construction()
        {
            start_timer();

            Task.Run(() =>
            {
                while (true)
                {
                    if (step_timer() > Field) return;

                    Process();

                    sleep(10);
                }
            });

            add_event("", Process);
        }

        void Process()
        {
            if (_values.TryExtractAll(out Action[] actions))
                foreach(Action action in actions) action.Invoke();
        }
    }
}