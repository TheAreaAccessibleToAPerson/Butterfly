using System.Runtime.InteropServices.ComTypes;
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
            public const string BEGIN_CONFIGURATE = "ConfigurateObject";

            public const string BEGIN_STARTING = "StartingObject";

            /// <summary>
            /// Продолжаем запуск обьекта. 
            /// </summary>
            public const string BEGIN_START = "BeginStart";

            /// <summary>
            /// Приступает к остановки обьектов. 
            /// </summary>
            public const string BEGIN_STOPPING = "StoppingObject";
        }

        private readonly information.State.Manager _stateInformationManager;
        private readonly information.State _stateInformation;
        private readonly information.Header _headerInformation;
        private readonly information.DOM _DOMInformation;
        private readonly information.Tegs _tegsInformation;

        private readonly manager.BranchObjects _branchObjectsManager;
        private readonly manager.NodeObjects _nodeObjectsManager;

        private readonly IDispatcher _dispatcher;

        public LifeCyrcle(informing.IMain mainInforming, information.Header headerInformation,
            information.State stateInformation, information.State.Manager stateInformationManager,
                information.DOM DOMInformation, information.Tegs tegsInformation,
                    manager.BranchObjects branchObjectsManager, manager.NodeObjects nodeObjectsManager,
                        IDispatcher dispatcher)
            : base("LifeCyrcleManager", mainInforming)
        {
            _stateInformationManager = stateInformationManager;
            _headerInformation = headerInformation;
            _stateInformation = stateInformation;
            _tegsInformation = tegsInformation;
            _DOMInformation = DOMInformation;

            _branchObjectsManager = branchObjectsManager;
            _nodeObjectsManager = nodeObjectsManager;

            _dispatcher = dispatcher;
        }

        void dispatcher.ILifeCyrcle.Contruction()
        {
            lock (_stateInformation.Locker)
            {
                lock (_DOMInformation.NearBoardNodeObject.StateInformation.Locker)
                {
                    if (_stateInformation.IsDestroying == false &&
                        _DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                    {
                        if (_stateInformationManager.Replace(information.State.Data.CONSTRUCTION, information.State.Data.OCCUPERENCE))
                        {
                            if (Hellper.GetSystemMethod("Construction", _DOMInformation.CurrentObject.GetType(),
                            out System.Reflection.MethodInfo oSystemMethodConstruction))
                                oSystemMethodConstruction.Invoke(_DOMInformation.CurrentObject, null);

                            if (_stateInformation.IsDestroying == false &&
                                _DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                            {
                                _branchObjectsManager.LifeCyrcle(Data.BEGIN_BRANCH_OBJECT_CONTRUCTION);

                                if (_stateInformation.IsDestroying == false &&
                                    _DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                                {
                                    if (_headerInformation.IsNodeObject())
                                        _dispatcher.Process(manager.Dispatcher.Command.CONFIGURATE_OBJECT);

                                    return;
                                }
                                else _stateInformationManager.Set(information.State.Data.CALL_DESTROY_IN_BRANCH_OBJECTS_CONTRUCTION);
                            }
                            else _stateInformationManager.Set(information.State.Data.CALL_DESTROY_IN_CONTRUCTION);
                        }
                        else throw new Exception();
                    }
                    else _stateInformationManager.Set(information.State.Data.INTERRUPTED_CONSTRUCTION);

                    if (_headerInformation.IsNodeObject())
                        ((manager.INodeObjects)_DOMInformation.ParentObject).InformingCollected();
                }
            }
        }

        void dispatcher.ILifeCyrcle.Configurate()
        {
            lock (_stateInformation.Locker)
            {
                if (_stateInformation.IsDestroying == false &&
                    _DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                {
                    if (_stateInformationManager.Replace(information.State.Data.CONFIGURATE, information.State.Data.CONSTRUCTION))
                    {
                        if (Hellper.GetSystemMethod("Configurate", _DOMInformation.CurrentObject.GetType(),
                        out System.Reflection.MethodInfo oSystemMethodConfigurate))
                            oSystemMethodConfigurate.Invoke(_DOMInformation.CurrentObject, null);

                        if (_stateInformation.IsDestroying == false &&
                            _DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                        {
                            _branchObjectsManager.LifeCyrcle(Data.BEGIN_CONFIGURATE);

                            if (_stateInformation.IsDestroying == false &&
                                _DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                            {
                                if (_headerInformation.IsNodeObject())
                                    _dispatcher.Process(manager.Dispatcher.Command.STARTING_OBJECT);

                                return;
                            }
                            else _stateInformationManager.Set(information.State.Data.CALL_DESTROY_IN_BRANCH_OBJECTS_CONFIGURATE);
                        }
                        else _stateInformationManager.Set(information.State.Data.CALL_DESTROY_IN_CONFIGURATE);
                    }
                    else throw new Exception();
                }
                else _stateInformationManager.Set(information.State.Data.INTERRUPTED_CONFIGURATE);

                if (_headerInformation.IsNodeObject())
                    ((manager.INodeObjects)_DOMInformation.ParentObject).InformingCollected();
            }
        }

        void dispatcher.ILifeCyrcle.Starting()
        {
            lock (_stateInformation.Locker)
            {
                if (_stateInformation.IsDestroying == false &&
                    _DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                {
                    if (_stateInformationManager.Replace(information.State.Data.STARTING, information.State.Data.CONFIGURATE))
                    {
                        _branchObjectsManager.LifeCyrcle(Data.BEGIN_STARTING);

                        if (_stateInformation.IsDestroying == false ||
                            _DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                        {
                            if (Hellper.GetSystemMethod("Start", _DOMInformation.CurrentObject.GetType(),
                            out System.Reflection.MethodInfo oSystemMethodConfigurate))
                                oSystemMethodConfigurate.Invoke(_DOMInformation.CurrentObject, null);

                            if (_stateInformation.IsDestroying == false ||
                                _DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                            {
                                if (_stateInformationManager.Replace(information.State.Data.SUBSCRIBE, information.State.Data.STARTING))
                                {
                                    _dispatcher.Process(manager.Dispatcher.Command.START_SUBSCRIBE);

                                    return;
                                }
                                else throw new Exception();
                            }
                            else _stateInformationManager.Set(information.State.Data.INTERRUPTED_CALL_DESTROY_IN_START);
                        }
                        else _stateInformationManager.Set(information.State.Data.INTERRUPTED_CALL_DESTROY_IN_BRANCH_OBJECTS_STARTING);
                    }
                    else throw new Exception();
                }
                else _stateInformationManager.Set(information.State.Data.INTERRUPTED_STARTING);

                if (_headerInformation.IsNodeObject())
                    ((manager.INodeObjects)_DOMInformation.ParentObject).InformingCollected();
            }
        }

        void dispatcher.ILifeCyrcle.Start()
        {
            lock (_stateInformation.Locker)
            {
                if (_stateInformation.IsDestroying == false ||
                    _DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false)
                {
                    if (_stateInformationManager.Replace(information.State.Data.START, information.State.Data.SUBSCRIBE))
                    {
                        _branchObjectsManager.LifeCyrcle(Data.BEGIN_START);

                        _dispatcher.Process(manager.Dispatcher.Command.STARTING_THREAD);
                    }
                    else throw new Exception();
                }
                else
                    _stateInformationManager.Set("***");

                if (_headerInformation.IsNodeObject())
                    ((manager.INodeObjects)_DOMInformation.ParentObject).InformingCollected();

                if (_stateInformation.IsDeferredDestroying)
                {
                    ((manager.ILifeCyrcle)_DOMInformation.CurrentObject).ContinueDestroy();
                }
            }

        }

        void dispatcher.ILifeCyrcle.Stopping()
        {
            if (_stateInformation.IsDestroying == false) throw new Exception();

            lock (_stateInformation.Locker)
            {
                if (_stateInformation.IsStopping) throw new Exception();

                _stateInformationManager.Set(information.State.Data.STOPPING);

                if (_stateInformation.IsInterrupted)
                {
                    if (_stateInformation.Interrupted > 3)
                    {
                        if (Hellper.GetSystemMethod("Stop", _DOMInformation.CurrentObject.GetType(),
                            out System.Reflection.MethodInfo oSystemMethodStart))
                            oSystemMethodStart.Invoke(_DOMInformation.CurrentObject, null);
                    }
                }

                if (_headerInformation.IsNodeObject())
                {
                    // Вызывается синхронно.
                    _dispatcher.Process(manager.Dispatcher.Command.START_UNSUBSCRIBE);
                }
                else
                {
                    _dispatcher.Process(manager.Dispatcher.Command.CONTINUE_STOPPING);
                }
            }
        }

        // Поуберить или

        void dispatcher.ILifeCyrcle.ContinueStopping()
        {
            lock (_stateInformation.Locker)
            {
                if (_stateInformation.IsDestroying == false) throw new Exception();
                if (_stateInformation.IsStop) throw new Exception();

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
                        if (_tegsInformation.Contains(root.manager.PollThreads.TEG))
                        {
                            _nodeObjectsManager.StoppingAllObject();
                        }
                        else
                        {
                            _DOMInformation.RootManager.ActionInvoke
                                (_nodeObjectsManager.StoppingAllObject);
                        }
                    }
                }
            }
        }

        public void Destroy()
        {
            if (_stateInformation.IsStop)
                SystemInformation($"Вы вызвали метод destroy() у обьекта {_headerInformation.Explorer}" +
                    "который уже был удален из системы.");

            //************************Грязная проверка**********************

            // Данный обьект может быть выставлен на отложеное уничтожение если 
            // в данный момент он находится в состоянии Subscribe.
            // После того как подписка будет окончена обьект начнет уничтожатся.
            if (_stateInformation.IsDeferredDestroying == false)
            {
                //...
            }
            // 1) Обьект уже начал свое уничтожние.
            // 2) Обьект уже начал процесс остановки.
            // 3) Обьект уже значет что после создания ему нужно начать свое уничтожение.
            else if (_stateInformation.IsDestroying || _stateInformation.IsStopping ||
                _stateInformation.IsDeferredDestroying)
                return;

            //**************************************************************

            lock (_stateInformation.Locker)
            {

                // Обьект находится в состоянии подписки и мы в первый раз сообщаем
                // ему о том что после ее окончания нужно преступить к уничтожению.
                if (_stateInformation.IsSubscribe &&
                    _stateInformation.IsDeferredDestroying == false)
                {
                    _stateInformationManager.DeferredDestroy();

                    return;
                }

                //if (_stateInformation.IsSubscribe) return;

                //************************Чистая проверка****************************
                if (_stateInformation.IsDeferredDestroying == false)
                {
                    //...
                }
                else if (_stateInformation.IsDestroying || _stateInformation.IsStopping ||
                    _stateInformation.IsDeferredDestroying)
                    return;
                //********************************************************************

                // Обьект начинаем процедуру уничтожения.
                if (_stateInformation.IsDestroying == false)
                {
                    _stateInformationManager.Destroy();

                    _DOMInformation.RootManager.ActionInvoke(() =>
                    {
                        if (_headerInformation.IsBoard())
                        {
                            ((manager.IDispatcher)_DOMInformation.CurrentObject).
                                Process(manager.Dispatcher.Command.STOPPING_OBJECT);
                        }
                        else if (_DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false &&
                            _DOMInformation.NearBoardNodeObject.StateInformation.IsDeferredDestroying == false)
                        {
                            _DOMInformation.NearBoardNodeObject.destroy();
                        }
                        else if (_DOMInformation.NodeObject.StateInformation.IsDestroying == false &&
                            _DOMInformation.NodeObject.StateInformation.IsDeferredDestroying == false)
                        {
                            _DOMInformation.NodeObject.destroy();
                        }
                    });
                }
            }
        }

        public void ContinueDestroy()
        {
            lock (_stateInformation.Locker)
            {
                _stateInformationManager.Destroy();

                if (_headerInformation.IsBoard())
                {
                    ((manager.IDispatcher)_DOMInformation.CurrentObject).
                        Process(manager.Dispatcher.Command.STOPPING_OBJECT);
                }
                else if (_DOMInformation.NearBoardNodeObject.StateInformation.IsDestroying == false &&
                    _DOMInformation.NearBoardNodeObject.StateInformation.IsDeferredDestroying == false)
                {
                    _DOMInformation.NearBoardNodeObject.destroy();
                }
                else if (_DOMInformation.NodeObject.StateInformation.IsDestroying == false &&
                    _DOMInformation.NodeObject.StateInformation.IsDeferredDestroying == false)
                {
                    _DOMInformation.NodeObject.destroy();
                }
                else
                {
                    ((manager.IDispatcher)_DOMInformation.CurrentObject).
                        Process(manager.Dispatcher.Command.STOPPING_OBJECT);
                }
            }
        }
    }
}