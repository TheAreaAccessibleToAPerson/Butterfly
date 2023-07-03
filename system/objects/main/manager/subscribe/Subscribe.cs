namespace Butterfly.system.objects.main.manager
{
    namespace description
    {
        /// <summary>
        /// Описывает метод который будет вызываться в менеджерах подписак
        /// после того как придет ответ о подписании в нужное место.
        /// /// </summary>
        public interface ISubscribe
        {
            /// <summary>
            /// Сообщим менеджеру отвечаещего за все подписки о том что нас подписали.
            /// в нужные места.
            /// /// </summary>
            public void EndSubscribe();

            /// <summary>
            /// Сообщим менеджеру отвечающего за все подписки о том что на описали. 
            /// </summary>
            public void EndUnsubscribe();
        }
    }

    public class Subscribe : Informing, dispatcher.ISubscribe, description.ISubscribe
    {
        public struct Type
        {
            public const string POLL = "Poll";
        }

        private readonly information.State _stateInformation;
        private readonly information.DOM _DOMInformation;

        private readonly controller.Poll _pollController;

        private readonly manager.IDispatcher _dispatcherManager;

        private readonly object _locker = new object();

        /// <summary>
        /// Общее количесво регистраций на подписки. 
        /// </summary>
        private bool RegisterToPoll = false;

        /// <summary>
        /// Когда нас подпишут, нам об этом сообщат, и мы инкременириует данное значение.
        /// Если оно станет равно количесву регистраци, то мы сообщим диспетчеру.
        /// </summary>
        private uint SubscribeCount = 0;

        public Subscribe(informing.IMain mainInforming, information.State stateInformation,
            information.Header headerInformation, information.DOM DOMInformation,
                manager.Dispatcher dispatcherManager)
            : base("SubscribeManager", mainInforming)
        {
            _stateInformation = stateInformation;
            _DOMInformation = DOMInformation;
            _dispatcherManager = dispatcherManager;

            _pollController = new controller.Poll
                (mainInforming, stateInformation, headerInformation,
                    DOMInformation, this);
        }

        /// <summary>
        /// Добавляется данные для регистрации в пулл потоков.
        /// </summary>
        /// <param name="name">Имя пулла на который нужно подписаться.</param>
        /// <param name="action">Action который мы выставляем в пулл потоков для обработки.</param>
        public void Add(string name, Action action, string type)
        {
            //lock (_locker)
            {
                if (_stateInformation.IsContruction)
                {
                    if (type == Subscribe.Type.POLL)
                        RegisterToPoll = true;

                    _pollController.Add(name, action, type);
                }
                else
                    Exception($"Вы пытатесь добавить событие {name} не в методe Contruction().");
            }
        }

        void dispatcher.ISubscribe.StartSubscribe()
        {
            //lock (_locker)
            {
                if (RegisterToPoll)
                {
                    _pollController.Subscribe();
                }
                else
                {
                    _DOMInformation.RootManager.ActionInvoke(() => 
                    {
                        _dispatcherManager.Process(manager.Dispatcher.Command.STARTING_OBJECT);
                    });
                }
            }
        }

        void dispatcher.ISubscribe.StartUnsubscribe()
        {
            //lock (_locker)
            {
                if (RegisterToPoll)
                {
                    _pollController.Unsubscribe();
                }
                else
                {
                    _DOMInformation.RootManager.ActionInvoke(() => 
                    {
                        _dispatcherManager.Process(manager.Dispatcher.Command.END_UNSUBSCRIBE);
                    });
                }
            }
        }

        void description.ISubscribe.EndSubscribe()
        {
            //lock (_locker)
            {
                if (RegisterToPoll)
                {
                    _DOMInformation.RootManager.ActionInvoke(()
                        => _dispatcherManager.Process(manager.Dispatcher.Command.STARTING_OBJECT));
                }
                else
                    throw new Exception();
            }
        }

        void description.ISubscribe.EndUnsubscribe()
        {
            //lock (_locker)
            {
                if (RegisterToPoll)
                {
                    RegisterToPoll = false;

                    _DOMInformation.RootManager.ActionInvoke(()
                        => _dispatcherManager.Process(manager.Dispatcher.Command.END_UNSUBSCRIBE));
                }
                else
                    throw new Exception();
            }
        }
    }
}