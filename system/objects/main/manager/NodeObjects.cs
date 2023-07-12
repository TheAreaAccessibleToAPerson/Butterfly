namespace Butterfly.system.objects.main.manager
{
    public sealed class NodeObjects : Informing
    {
        private readonly informing.IMain _mainInforming;

        private readonly Dictionary<string, object> _globalObjects;

        private readonly information.Header _headerInformation;
        private readonly information.State _stateInformation;
        private readonly information.DOM _DOMInformation;

        public NodeObjects(informing.IMain mainInforming, information.Header headerInformation, information.State stateInformation,
            information.DOM DOMInformation, Dictionary<string, object> globalObject)
            : base("NodeObjectsManager", mainInforming)
        {
            _mainInforming = mainInforming;

            _headerInformation = headerInformation;
            _stateInformation = stateInformation;
            _DOMInformation = DOMInformation;

            _globalObjects = globalObject;
        }

        private Dictionary<string, main.Object> _values;
        private Dictionary<string, main.Object> _dublicatValues;

        /// <summary>
        /// Количесво Node обьектов. 
        /// </summary>
        /// <value></value>
        public int Count
        {
            get
            {
                lock (_stateInformation.Locker) return _values == null ? 0 : _values.Count;
            }
        }

        /// <summary>
        /// Текущее количесво собираемых обьектов.
        /// </summary>
        private int _collectedCount = 0;

        /// <summary>
        /// Начат процесс остановки всех обьектов. 
        /// </summary>
        private bool StoppingAllObjectRunning = false;

        /// <summary>
        /// Инкрементируем количесво собраных объектов.
        /// </summary>
        public void IncrementCollectedCount()
        {
            lock (_stateInformation.Locker)
            {
                if (_values == null)
                {
                    //...
                }
                else
                {
                    if (_headerInformation.IsSystemController())
                    {
                        if ((--_collectedCount) == -1)
                        {
                            _collectedCount = 0;

                            if (_stateInformation.IsDestroying)
                            {
                                StoppingAllObject();
                            }
                        }
                    }
                    else
                    {
                        if ((--_collectedCount) == 0)
                        {
                            if (_stateInformation.IsDestroying)
                            {
                                _DOMInformation.RootManager.ActionInvoke(StoppingAllObject);
                            }
                        }
                    }
                }
            }
        }

        public NodeObjectType Add<NodeObjectType>(string key, params object[] localFields)
            where NodeObjectType : main.Object, main.description.IDOM, new()
        {
            lock (_stateInformation.Locker)
            {
                if (_stateInformation.IsDestroying == false &&
                    (_stateInformation.IsStart || _stateInformation.IsStarting || _stateInformation.IsSubscribe))
                {
                    if (_values is null) _values = new Dictionary<string, Object>();

                    if (_values.TryGetValue(key, out main.Object value))
                    {
                        if (value is NodeObjectType valueReduse)
                        {
                            lock (valueReduse.StateInformation.Locker)
                            {
                                if (valueReduse.StateInformation.IsDestroying == false &&
                                    valueReduse.StateInformation.IsStart ||
                                    valueReduse.StateInformation.IsOccuperence ||
                                    valueReduse.StateInformation.IsConfigurate ||
                                    valueReduse.StateInformation.IsSubscribe ||
                                    valueReduse.StateInformation.IsStarting)
                                {
                                    return valueReduse;
                                }
                                else
                                {
                                    if (_dublicatValues is null)
                                        _dublicatValues = new Dictionary<string, main.Object>();

                                    NodeObjectType nodeObject = Define<NodeObjectType>(key, localFields);

                                    _dublicatValues.Remove(key);

                                    _dublicatValues.Add(key, nodeObject);

                                    return nodeObject;
                                }
                            }
                        }
                        else
                            Exception(data.BranchManager.x100002, typeof(NodeObjectType).FullName,
                                key, value.GetType().FullName);
                    }
                    else
                    {
                        NodeObjectType nodeObject = Define<NodeObjectType>(key, localFields);

                        _values.Add(key, nodeObject);

                        _collectedCount++;
                        _DOMInformation.RootManager.ActionInvoke(nodeObject.CreatingNode);

                        return nodeObject;
                    }
                }

                return default;
            }
        }

        private NodeObjectType Define<NodeObjectType>(string key, params object[] localFields)
            where NodeObjectType : main.Object, main.description.IDOM, new()
        {
            NodeObjectType nodeObject = new NodeObjectType();

            if (localFields.Length == 1)
            {
                if (nodeObject is ILocalField objectLocalFieldReduse)
                {
                    objectLocalFieldReduse.SetField(localFields[0]);
                }
                else
                    Exception("Вы передали значение для локального поля обьекта, не не подключили его.");
            }
            else if (localFields.Length > 1)
            {
                if (nodeObject is ILocalField objectLocalFieldReduse)
                {
                    objectLocalFieldReduse.SetField(localFields);
                }
                else
                    Exception("Вы передали значение для локального поля обьекта, не не подключили его.");
            }

            nodeObject.NodeDefine(key, _DOMInformation.NestingNodeNamberInTheSystem + 1,
                    _DOMInformation.ParentObjectsID, _DOMInformation.CurrentObject,
                        _DOMInformation.NearBoardNodeObject, _DOMInformation.RootManager,
                            _globalObjects);

            return nodeObject;
        }

        public void StoppingAllObject()
        {
            lock (_stateInformation.Locker)
            {
                if (_collectedCount == 0 && StoppingAllObjectRunning == false)
                {
                    StoppingAllObjectRunning = true;

                    if (_values == null || _values.Count == 0)
                    {
                        if (_stateInformation.IsDestroying)
                        {
                            _DOMInformation.RootManager.ActionInvoke
                                (((manager.ILifeCyrcle)_DOMInformation.CurrentObject).ContinueDestroy);
                        }
                    }
                    else
                    {
                        foreach (main.Object nodeObject in _values.Values)
                        {
                            // Если обьект ялвяется Branch и уже находится в процессе
                            // остановки или же должен быть уничтожен отложено после окончания
                            // подписки, то нам не нужно дублировать его уничтожение.
                            if (nodeObject.HeaderInformation.IsBranchObject() &&
                                (nodeObject.StateInformation.IsDestroying == false &&
                                nodeObject.StateInformation.IsDeferredDestroying == false))
                            {
                                //...
                            }
                            else
                            {
                                // Если обьект уже был обьявлен как уничтожаемый, то приступаем
                                // к его остановки.
                                if (nodeObject.StateInformation.IsDestroying)
                                {
                                    ((manager.IDispatcher)nodeObject).
                                        Process(manager.Dispatcher.Command.STOPPING_OBJECT);
                                }
                                else
                                // Иначе нужно запустить процедуру уничтожения.
                                {
                                    ((ILifeCyrcle)nodeObject).ContinueDestroy();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Remove(string key)
        {
            lock (_stateInformation.Locker)
            {
                if (_values.Remove(key))
                {
                    if (_dublicatValues != null && _dublicatValues.Count > 0)
                    {
                        if (_dublicatValues.Remove(key, out main.Object nodeObject))
                        {
                            _values.Add(key, nodeObject);

                            _collectedCount++;

                            ((main.description.IDOM)nodeObject).CreatingNode();
                        }
                    }

                    //SystemInformation($"Count:{_values.Count} NAME {key}");

                    if (_stateInformation.IsStop == false && _collectedCount == 0 && (_stateInformation.IsDestroying
                        || _stateInformation.IsStopping) && _values.Count == 0)
                    {
                        ((manager.IDispatcher)_DOMInformation.CurrentObject).
                           Process(manager.Dispatcher.Command.CONTINUE_STOPPING);
                    }
                }
                else
                {
                    if (_headerInformation.IsSystemController())
                    {
                    }
                    else
                    {
                        Exception("EXCEPTION:" + key);
                    }
                }
            }
        }
    }
}