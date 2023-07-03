namespace Butterfly.system.objects.main.manager
{
    public class LifeCyrcle : Informing, dispatcher.ILifeCyrcle
    {
        public struct Data
        {
            /// <summary>
            /// Вызываем команду в диспетчере которая произведет конструирование обьекта
            /// c помощью метода LifeCyrcleManager.Construction.
            /// </summary>
            public const string BEGIN_BRANCH_OBJECT_CONTRUCTION = "ContructionObject";

            /// <summary>
            /// Приступаем к запуску обьектов.
            /// </summary>
            public const string BEGIN_STARTING = "StartingObject";

            /// <summary>
            /// Продолжаем запуск обьекта. 
            /// </summary>
            public const string CONTINUE_STARTING = "ContinueStartingObject";

            /// <summary>
            /// Приступает к остановки обьектов. 
            /// </summary>
            public const string BEGIN_STOPPING = "StoppingObject";
        }

        private readonly information.State.Manager _stateInformationManager;
        private readonly information.State _stateInformation;
        private readonly information.Header _headerInformation;
        private readonly information.DOM _DOMInformation;

        private readonly manager.BranchObjects _branchObjectsManager;
        private readonly manager.NodeObjects _nodeObjectsManager;

        private readonly IDispatcher _dispatcher;

        public LifeCyrcle(informing.IMain mainInforming, information.Header headerInformation,
            information.State stateInformation, information.State.Manager stateInformationManager,
                information.DOM DOMInformation, manager.BranchObjects branchObjectsManager,
                    manager.NodeObjects nodeObjectsManager, IDispatcher dispatcher)
            : base("LifeCyrcleManager", mainInforming)
        {
            _stateInformationManager = stateInformationManager;
            _stateInformation = stateInformation;
            _headerInformation = headerInformation;
            _DOMInformation = DOMInformation;

            _branchObjectsManager = branchObjectsManager;
            _nodeObjectsManager = nodeObjectsManager;

            _dispatcher = dispatcher;
        }

        /// <summary>
        /// В данном методе реализуется переход от жизненого состояния OCCUPERENCE в CONSTRUCTION.
        /// После смены состояния вызовется системный метод void Contruction(), в данном методе обьект
        /// узнает о своих Branch обьектах, установим связь с глобальными обьектами определеными в 
        /// родительских обьектах и укажим на какие события нужно подписаться. Так как Branch 
        /// обьекты являются статическими для своего родителя, и их сущесвование намертво будет связано 
        /// с родителем, то перед тем как текущий обьект продолжит свою сборку, ему необходимо вызвать 
        /// текущий метод в своих Branch обьектах, а те в свою очередь вызовут в своих и тд...
        /// Предпологается что данный метод должен 100% вызватся во всех обьектах.
        /// Подписка/отписка на события в Branch обьектах будет осущесвлятся через его Node обьект.
        /// </summary>
        void dispatcher.ILifeCyrcle.Contruction()
        {
            lock (_stateInformation.Locker)
                if (_stateInformationManager.Replace
                    (information.State.Data.CONSTRUCTION, information.State.Data.OCCUPERENCE) == false)
                    throw new Exception();

            if (Hellper.GetSystemMethod("Construction", _DOMInformation.CurrentObject.GetType(),
                out System.Reflection.MethodInfo oSystemMethodConstruction))
                oSystemMethodConstruction.Invoke(_DOMInformation.CurrentObject, null);

            _branchObjectsManager.LifeCyrcle(Data.BEGIN_BRANCH_OBJECT_CONTRUCTION);

            _stateInformationManager.Set(information.State.Data.SUBSCRIBE);

            _dispatcher.Process(manager.Dispatcher.Command.START_SUBSCRIBE);
        }

        /// <summary>
        /// После того как Node обьект подписался на все события вызовется данный метод.
        /// Сменим жизненый цикл с CONSTRUCTION на STARTING. Проверим не был ли выставлен
        /// ближайший Board обьект на уничтожение. После чего вызовем системный метод 
        /// void Configurate(). После чего проверим не был ли ближайший Board обьект выставлен
        /// на уничтожение, если все нормально то вызовем данный метод во всех Branch обьектах.
        /// После чего запустится системный метод Start().
        /// </summary>
        void dispatcher.ILifeCyrcle.Starting()
        {
            lock (_stateInformation.Locker)
            {
                if (_stateInformationManager.Replace(information.State.Data.CONFIGURATE, information.State.Data.SUBSCRIBE))
                {
                    if (_DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                    {
                        if (Hellper.GetSystemMethod("Configurate", _DOMInformation.CurrentObject.GetType(),
                            out System.Reflection.MethodInfo oSystemMethodConfigurate))
                            oSystemMethodConfigurate.Invoke(_DOMInformation.CurrentObject, null);

                        if (_DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                        {
                            if (_stateInformationManager.Replace(information.State.Data.STARTING, information.State.Data.CONFIGURATE))
                            {
                                _branchObjectsManager.LifeCyrcle(Data.BEGIN_STARTING);

                                if (_DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                                {
                                    if (Hellper.GetSystemMethod("Start", _DOMInformation.CurrentObject.GetType(),
                                        out System.Reflection.MethodInfo oSystemMethodStart))
                                        oSystemMethodStart.Invoke(_DOMInformation.CurrentObject, null);

                                    if (_headerInformation.IsNodeObject() &&
                                        _DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                                    {
                                        _dispatcher.Process(manager.Dispatcher.Command.CONTINUE_STARTING_OBJECT);

                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                if (_headerInformation.IsNodeObject())
                    ((manager.INodeObjects)_DOMInformation.ParentObject).InformingCollected();
            }
        }

        void dispatcher.ILifeCyrcle.ContinueStarting()
        {
            lock (_stateInformation.Locker)
            {
                if (_DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                {
                    _branchObjectsManager.LifeCyrcle(Data.CONTINUE_STARTING);

                    _dispatcher.Process(manager.Dispatcher.Command.STARTING_THREAD + __.AND +
                        manager.Dispatcher.Command.CREATING_DEFERRED_NODE_OBJECT);

                    _stateInformationManager.Replace
                        (information.State.Data.START, information.State.Data.STARTING);
                }

                if (_headerInformation.IsNodeObject())
                    ((manager.INodeObjects)_DOMInformation.ParentObject).InformingCollected();

                if (_stateInformation.IsDeferredDestroying)
                {
                    _DOMInformation.CurrentObject.destroy();
                }
            }
        }

        void dispatcher.ILifeCyrcle.Stopping()
        {
            lock (_stateInformation.Locker)
            {
                if (_stateInformation.IsStart || _stateInformation.IsStarting || _stateInformation.IsConfigurate)
                {
                    if (Hellper.GetSystemMethod("Stop", _DOMInformation.CurrentObject.GetType(),
                        out System.Reflection.MethodInfo oSystemMethodStart))
                        oSystemMethodStart.Invoke(_DOMInformation.CurrentObject, null);
                }

                _stateInformationManager.Set(information.State.Data.STOPPING);

                if (_headerInformation.IsNodeObject())
                {
                    _dispatcher.Process(manager.Dispatcher.Command.START_UNSUBSCRIBE);
                }
                else
                {
                    _dispatcher.Process(manager.Dispatcher.Command.CONTINUE_STOPPING);
                }
            }
        }

        void dispatcher.ILifeCyrcle.ContinueStopping()
        {
            lock (_stateInformation.Locker)
            {
                if (_nodeObjectsManager.Count == 0 && _branchObjectsManager.Count == 0)
                {
                    _dispatcher.Process(manager.Dispatcher.Command.REMOVE_GLOBAL_OBJECTS);

                    _stateInformationManager.Set(information.State.Data.STOP);

                    if (_headerInformation.IsNodeObject())
                        ((manager.INodeObjects)_DOMInformation.ParentObject).
                            Remove(_DOMInformation.KeyObject);
                    else
                        ((manager.IBranchObjects)_DOMInformation.ParentObject).
                            Remove(_DOMInformation.KeyObject);
                }
                else
                {
                    if (_nodeObjectsManager.Count == 0)
                    {
                        _branchObjectsManager.LifeCyrcle(Data.BEGIN_STOPPING);
                    }
                    else
                    {
                        _nodeObjectsManager.StoppingAllObject();
                    }
                }
            }
        }

        public void Destroy()
        {
            /// Если обьект уже останавливается. 
            if (_stateInformation.IsStopping) return;

            /// Обьект уже выставлен на уничтожение.
            if (_stateInformation.IsDestroying) return;

            // Обьект уже выставлен на отложеное уничтжение.
            if (_stateInformation.IsDeferredDestroying &&
                (_stateInformation.IsOccuperence || _stateInformation.IsContruction || _stateInformation.IsSubscribe))
            {
                return;
            }

            lock (_stateInformation.Locker)
            {
                // Пока обьект собирается мы не можем его уничтожить.
                if (_stateInformation.IsDeferredDestroying == false &&
                    (_stateInformation.IsOccuperence || _stateInformation.IsContruction || _stateInformation.IsSubscribe))
                {
                    _stateInformationManager.DeferredDestroy();
                }
                else if (_stateInformation.IsOccuperence || _stateInformation.IsContruction || _stateInformation.IsSubscribe)
                {
                    return;
                }
                else if (_stateInformation.IsDestroying == false)
                {
                    _stateInformationManager.Destroy();

                    if (_headerInformation.IsBoard())
                        _DOMInformation.RootManager.ActionInvoke(()
                            => _dispatcher.Process(manager.Dispatcher.Command.STOPPING_OBJECT));
                    else
                        _DOMInformation.RootManager.ActionInvoke
                            (_DOMInformation.NearBoardNodeObject.destroy);
                }
            }
        }

        public void ContinueDestroy()
        {
            lock (_stateInformation.Locker)
            {
                if (_stateInformation.IsDestroying == false &&
                _DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying)
                {
                    _stateInformationManager.Destroy();

                    _dispatcher.Process(manager.Dispatcher.Command.STOPPING_OBJECT);
                }
                if (_stateInformation.IsDestroying && _stateInformation.IsStopping)
                {
                    _dispatcher.Process(manager.Dispatcher.Command.CONTINUE_STOPPING);
                }
            }
        }
    }
}