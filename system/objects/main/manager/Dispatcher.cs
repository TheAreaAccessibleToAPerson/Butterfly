namespace Butterfly.system.objects.main.manager
{
    public interface IDispatcher
    {
        public void Process(string command);
    }

    namespace dispatcher
    {
        /// <summary>
        /// Описывает сборос работы диспетчера с LifeCyrcleManager'ом.
        /// </summary>
        public interface ILifeCyrcle
        {
            public void Contruction();
            public void Starting();
            public void ContinueStarting();
            public void Stopping();
            public void ContinueStopping();
        }

        /// <summary>
        /// Описывает способ работы диспетчера с SubscribeManager'ом.
        /// </summary>
        public interface ISubscribe
        {
            /// <summary>
            /// Запускает процесс отписки. 
            /// </summary>
            void StartSubscribe();

            /// <summary>
            /// Запускает процесс подписки. 
            /// </summary>
            void StartUnsubscribe();
        }

        public interface IThreads
        {
            void Start();
            void Stop();
        }

        public interface INode
        {
            /// <summary>
            /// Создает отложеные обьекты которые были добавлены в методе Start(). 
            /// </summary>
            void CreatingDeferredObject();
        }

        public interface IGlobalObjects
        {
            /// <summary>
            /// Удаляем все глобальные обьекты созданые в нем. 
            /// </summary>
            void Remove();
        }
    }

    public sealed class Dispatcher : main.Informing, IDispatcher
    {
        public struct Command
        {
            /// <summary>
            /// Базовое состояние. 
            /// </summary>
            public const string NONE = "NONE";

            /// <summary>
            /// Создание Node обьекта:
            ///     Вызывается метод LifeCyrcleManager.Construction в котором запустится системный метод
            /// void Construction(). В данном методе Node обьект узнает о своих Branch обьектах, 
            /// о событиях на которые нужно будет подписаться. Так же установит связь с глобальными
            /// обьектами в своих родителях.
            /// Создание Branch обьекта: 
            ///     Данная команда будет доставлена из BranchObjectsManager родительского обьекта.
            /// Произадет все тоже самое что и с Node обьектом, за исключением того что события
            /// на которое нужно пописаться будут записаны в ближайший Node обьект. Именно он 
            /// произведет регистрацию.
            /// summary>
            public const string CONSTRUCTION_OBJECT = "CallContructionInLifeCyrcleManager";

            /// <summary>
            /// Node обьект сконструирован. Branch обьекты сконструированы, установлена связь с глобальными
            /// обьектами, все билеты для подписки на события созданы.
            /// Node обьект отвечает и за свои подписки на события и за подписки своих Branch обьектов.
            /// </summary>
            public const string START_SUBSCRIBE = "StartSubscribe";

            /// <summary>
            /// После того как Node обьект получен ответ от пулла, о том что все его регистрационые билеты
            /// для регистрации на события были обработаны, запустится метод LifeCyrcleManager.Starting. 
            /// </summary>
            public const string STARTING_OBJECT = "StartingObject";

            /// <summary>
            /// Первая стадия запуска подошла к концу.
            /// В ней мы вызвали метод Configurate, и если в данном методе обьект не был 
            /// выставлен на уничтожение, то запустился метод Start(). 
            /// </summary>
            public const string CONTINUE_STARTING_OBJECT = "TheFirstStageStartingIsClosed.";

            /// <summary>
            /// Запускаем потоки.
            /// </summary>
            public const string STARTING_THREAD = "StartingThread";

            /// <summary>
            /// Создать отложеные узлы, которые были добавлены в методе Start().
            /// </summary>
            public const string CREATING_DEFERRED_NODE_OBJECT = "CreatingDeferredNodeObject";

            /// <summary>
            /// Переводит обьект в состояния остановки. Первым делом останавливает поток.
            /// </summary>
            public const string STOPPING_OBJECT = "StoppingObject";

            /// <summary>
            /// Запускает процесс отписки в Node обьектах.
            /// </summary>
            public const string START_UNSUBSCRIBE = "StartUnsubscribe";

            /// <summary>
            /// Прекращает процесс остановки обьектов. 
            /// </summary>
            public const string CONTINUE_STOPPING = "ContinueStopping";

            /// <summary>
            /// Процесс отписки окончен. 
            /// </summary>
            public const string END_UNSUBSCRIBE = "EndUnsubscribe";

            /// <summary>
            /// Запускает в текущем обьекты процесс удаления всех созданых в нем глобальных обьектов. 
            /// </summary>
            public const string REMOVE_GLOBAL_OBJECTS = "RemoveGlobalObjects";
        }

        private readonly information.Header _headerInformation;

        /// <summary>
        /// Текущая команда которую выполняет диспетчер. 
        /// </summary>
        private string CurrentProcess = Command.NONE;

        private dispatcher.IGlobalObjects _globalObjectsDispatcher;
        private dispatcher.ILifeCyrcle _lifeCyrcleDispatcher;
        private dispatcher.ISubscribe _subscribeDispatcher;
        private dispatcher.IThreads _threadsDispatcher;
        private dispatcher.INode _nodeDispatcher;

        public Dispatcher(informing.IMain mainInforming, information.Header headerInformation)
                : base("DispatcherManager", mainInforming)
        {
            _headerInformation = headerInformation;
        }

        public void Initialize(dispatcher.IGlobalObjects globalObjectsDispatcher,
            dispatcher.ILifeCyrcle lifeCyrcleDispathcer, dispatcher.ISubscribe subscribeDispatcher,
                dispatcher.IThreads threadsDispatcher, dispatcher.INode nodeDispatcher)
        {
            _globalObjectsDispatcher = globalObjectsDispatcher;
            _lifeCyrcleDispatcher = lifeCyrcleDispathcer;
            _subscribeDispatcher = subscribeDispatcher;
            _threadsDispatcher = threadsDispatcher;
            _nodeDispatcher = nodeDispatcher;
        }
        /***************************************************************************************


        ***************************************************************************************/
        public void Process(string command)
        {
            switch (command)
            {
                case Command.CONSTRUCTION_OBJECT:

                    CurrentProcess = Command.CONSTRUCTION_OBJECT;

                    _lifeCyrcleDispatcher.Contruction();

                    break;

                case Command.START_SUBSCRIBE:

                    CurrentProcess =  Command.START_SUBSCRIBE;

                    if (_headerInformation.IsNodeObject())
                        _subscribeDispatcher.StartSubscribe();

                    break;

                case Command.STARTING_OBJECT:

                    CurrentProcess = Command.STARTING_OBJECT;

                    _lifeCyrcleDispatcher.Starting();

                    break;

                case Command.CONTINUE_STARTING_OBJECT:

                    CurrentProcess = Command.CONTINUE_STARTING_OBJECT;

                    _lifeCyrcleDispatcher.ContinueStarting();

                    break;

                case Command.STARTING_THREAD + __.AND + Command.CREATING_DEFERRED_NODE_OBJECT:

                    CurrentProcess = Command.STARTING_THREAD + __.AND + Command.CREATING_DEFERRED_NODE_OBJECT;

                    _threadsDispatcher.Start();

                    _nodeDispatcher.CreatingDeferredObject();

                    break;

                case Command.STOPPING_OBJECT:

                    CurrentProcess = Command.STOPPING_OBJECT;

                    _threadsDispatcher.Stop();

                    _lifeCyrcleDispatcher.Stopping();

                    break;

                case Command.START_UNSUBSCRIBE:

                    CurrentProcess = Command.START_UNSUBSCRIBE;

                    _subscribeDispatcher.StartUnsubscribe();

                    break;

                case Command.CONTINUE_STOPPING:
                case Command.END_UNSUBSCRIBE:

                    CurrentProcess = Command.CONTINUE_STOPPING;

                    _lifeCyrcleDispatcher.ContinueStopping();

                    break;

                case Command.REMOVE_GLOBAL_OBJECTS:

                    CurrentProcess = Command.REMOVE_GLOBAL_OBJECTS;

                    _globalObjectsDispatcher.Remove();

                    break;

                default: 
                    Console(command);
                    throw new Exception();
            }
        }

        /// <summary>
        /// Запустить диспетчер, начать сборку Node обьекта.
        /// </summary>
        public void Start()
        {
            if (_headerInformation.IsNodeObject())
            {
                ((IDispatcher)this).Process(Command.CONSTRUCTION_OBJECT);
            }
            else 
                Exception("Метод Start вызывается в Node обьекте.");
        }
    }
}