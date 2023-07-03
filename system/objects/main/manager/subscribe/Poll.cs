namespace Butterfly.system.objects.main.controller
{
    public class Poll : Informing
    {
        private readonly manager.description.ISubscribe _subscribeManager;

        private readonly information.State _stateInformation;
        private readonly information.Header _headerInformation;
        private readonly information.DOM _DOMInformation;

        /// <summary>
        /// Текущее состояние.
        /// </summary>
        private string State = StateType.CREATING_TICKET;

        private SubscribePollTicket[] _subscribeTickets = new SubscribePollTicket[0];

        /// <summary>
        /// Сдесь хранятся индексы пуллов в которых работают наши Action.
        /// </summary>
        private ulong[] _pointerToThePollID;
        /// <summary>
        /// Указан индекс в массиве в менеджере пуллов где хранится наш обрабатываемый Action.
        /// </summary>
        private int[] _pointerIndexInArrayToThePoll;

        /// <summary>
        /// Количесво подписаных билетов. Это значение нужно для того что бы при уничтожении
        /// обьекта дождатся пока мы отключимся от всех пулов на которые подписались.
        /// </summary>
        private int _subscribeTicketCount = 0;

        private readonly object _locker = new object();

        public Poll(informing.IMain mainInforming, information.State stateInformation,
            information.Header headerInformation, information.DOM DOMInformation,
                manager.description.ISubscribe subscribeManager)
            : base("SubscribeManager", mainInforming)
        {
            _stateInformation = stateInformation;
            _headerInformation = headerInformation;
            _DOMInformation = DOMInformation;

            _subscribeManager = subscribeManager;
        }

        /// <summary>
        /// Ссылка на метод в PollManager с помощью которого пулл будет общатся со своим подписоным обьектом. 
        /// 1) Тип информирования.
        /// 2) Номер индекса в массиве SubsribePollManager.
        /// 3) ID пулла.
        /// 4) Номер индекса в массиве RootPollClients.
        /// </summary>
        public void ToInforming(root.poll.InformingType informingType, int indexSubsribePollManager,
            ulong idPoll, int indexRootPollClients)
        {
            lock (_locker)
            {
                // Сообщает что мы отовсюду отписались;
                if (informingType.HasFlag(root.poll.InformingType.EndUnsubscribe))
                {
                    if (State == StateType.REGISTER_UNSUBSCRIBE)
                    {
                        if (_pointerToThePollID[indexSubsribePollManager] == idPoll &&
                            _pointerIndexInArrayToThePoll[indexSubsribePollManager] == indexRootPollClients)
                        {
                            if ((--_subscribeTicketCount) == 0)
                            {
                                State = StateType.END_UNSUBSCRIBE;

                                _subscribeManager.EndUnsubscribe();
                            }
                        }
                    }
                    else
                        throw new Exception("111111111111111111111111111111111111111111111111111111");
                }
                else if (informingType.HasFlag(root.poll.InformingType.ChangeOfIndex))
                {
                    _pointerToThePollID[indexSubsribePollManager] = idPoll;
                    _pointerIndexInArrayToThePoll[indexSubsribePollManager] = indexRootPollClients;

                    // В момент отписки наше место положение изменилось, повторное информирование
                    // с новым местом лежит на нас.
                    if (State == StateType.REGISTER_UNSUBSCRIBE)
                    {
                        _DOMInformation.RootManager.ActionInvoke(() =>
                        {
                            _DOMInformation.RootManager.AddUnsubscribeTickets(new UnsubscribePollTicket[]
                            {
                                new UnsubscribePollTicket()
                                {
                                    Name = _subscribeTickets[indexSubsribePollManager].Name,
                                    PollID = _pointerToThePollID[indexSubsribePollManager],
                                    IDObject = _DOMInformation.ID,
                                    IndexInRootPoll = _pointerIndexInArrayToThePoll[indexSubsribePollManager],
                                    IndexInSubscribePollManager = indexSubsribePollManager
                                }
                            });
                        });
                    }
                }
                else if (informingType.HasFlag(root.poll.InformingType.EndSubscribe))
                {
                    if (State == StateType.REGISTER_SUBSCRIBE)
                    {
                        // Запишем куда зарегестрировался наш тикет.
                        _pointerToThePollID[indexSubsribePollManager] = idPoll;
                        _pointerIndexInArrayToThePoll[indexSubsribePollManager] = indexRootPollClients;

                        // Дожидаемся пока все билеты откликрутся о завершении регистрации.
                        if ((++_subscribeTicketCount) == _pointerToThePollID.Length)
                        {
                            State = StateType.END_SUBSCRIBE;

                            _subscribeManager.EndSubscribe();
                        }
                    }
                    else
                        throw new Exception("333333333333333333333333333333333333333333333");
                }
            }
        }

        /// <summary>
        /// Добавляет регистриационый билет для пулла потоков. 
        /// </summary>
        /// <param name="name">Имя пулла потоков куда нам будет неоходимо произвести подписку.</param>
        /// <param name="action">Action который нам предстоит обрабатывать.</param>
        public void Add(string name, Action action, string type)
        {
            //lock (_locker)
            {
                if (_headerInformation.IsNodeObject())
                {
                    SubscribePollTicket ticket = new SubscribePollTicket()
                    {
                        Name = name,
                        Action = action,
                        Informing = ToInforming,
                        IDObject = _DOMInformation.ID,
                        IndexInSubscribePollManager = _subscribeTickets.Length
                    };

                    Hellper.ExpendArray(ref _subscribeTickets, ticket);
                }
                else
                    ((main.description.IPoll)_DOMInformation.NodeObject).Add(name, action, type);
            }
        }

        public void Subscribe()
        {
            //lock (_locker)
            {
                if (State == StateType.CREATING_TICKET)
                {
                    State = StateType.REGISTER_SUBSCRIBE;

                    // Сюда запишутся данные о том в каком пуле зарегестрирован наш билет.
                    // Если в последсвии билет будет передан в другой пулл, то
                    // он обязан будет сообщить о том где он в данный момент работает.
                    _pointerToThePollID = new ulong[_subscribeTickets.Length];

                    _pointerIndexInArrayToThePoll = new int[_subscribeTickets.Length];
                    // Поставим в очередь на регистрацию.

                    _DOMInformation.RootManager.ActionInvoke(() =>
                    {
                        _DOMInformation.RootManager.AddSubscribeTickets(_subscribeTickets);
                    });
                }
            }
        }

        /// <summary>
        /// Регистрируемся на отписку из пуллов.
        /// </summary>
        public void Unsubscribe()
        {
            //lock (_locker)
            {
                if (_subscribeTicketCount > 0)
                {
                    State = StateType.REGISTER_UNSUBSCRIBE;

                    UnsubscribePollTicket[] unsubscribePollTicket =
                        new UnsubscribePollTicket[_subscribeTickets.Length];

                    for (int i = 0; i < _subscribeTickets.Length; i++)
                    {
                        unsubscribePollTicket[i] = new UnsubscribePollTicket()
                        {
                            Name = _subscribeTickets[i].Name,
                            PollID = _pointerToThePollID[i],
                            IDObject = _DOMInformation.ID,
                            IndexInRootPoll = _pointerIndexInArrayToThePoll[i],
                            IndexInSubscribePollManager = i
                        };
                    }

                    _DOMInformation.RootManager.ActionInvoke(() =>
                    {
                        _DOMInformation.RootManager.AddUnsubscribeTickets(unsubscribePollTicket);
                    });
                }
            }
        }
    }
}