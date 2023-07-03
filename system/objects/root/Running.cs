namespace Butterfly
{
    public class Gudron
    {
        /// <summary>
        /// Запускает систему.
        /// </summary>
        /// <typeparam name="ObjectType">Обьект преставляющий систему. 
        /// Обьект должен быть унаследован от абстрактного класса Controller.
        /// </typeparam>
        public static void run<ObjectType>(Gudron.Settings settings)
            where ObjectType : system.objects.main.Object, new()
        {
            ((system.objects.root.description.ILife)
                new system.objects.root.Object<ObjectType>()).Run(settings);
        }

        public class Settings 
        {
            /// <summary>
            /// Имя проекта. По умолчанию имя будет пустым.
            /// </summary>
            /// <value></value>
            public string Name {get; init;} = "";

            /// <summary>
            /// Время необходимое для запуска проекта. По умолчанию 1 секунда.
            /// </summary>
            /// <value></value>
            public uint StartingTime {get; init;} = 1000;

            /// <summary>
            /// Настройки для системного события.
            /// По истечению времени заданому в полe StartingTime, дальнейшую
            /// работу будет обеспечивать системное событие с именем заданым в поле Name.
            /// Вы можете использовать системное событие указав его имя.
            /// </summary>
            /// <value></value>
            public EventSetting SystemEvent {get;init;} 
                = new EventSetting("", 5, 1024, false, Thread.Priority.Normal);

            /// <summary>
            /// Настройки для events которые будут использоваться. 
            /// </summary>
            /// <value></value>
            public EventSetting[] Events {get;init;} = new EventSetting[0];
        }
    }

    public class EventSetting
    {
        /// <summary>
        /// Имя события. 
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// TimeDelay для события.
        /// </summary>
        public readonly uint TimeDelay;

        /// <summary>
        /// Максимальный размер учаснтиков для события. 
        /// </summary>
        public readonly uint Size;

        /// <summary>
        /// Нужно ли уничтожать поток который обрабатывает 
        /// событие если все учаники отпишутся от него.
        /// </summary>
        public readonly bool IsDestroy;

        /// <summary>
        /// Приоритет для потока который будет обрабатывать данное событие. 
        /// </summary>
        public readonly Thread.Priority Priority;

        /// <summary>
        /// Создает настройки для событий которые будут использоваться в проекте. 
        /// </summary>
        /// <param name="name">Имя события.</param>
        /// <param name="timeDelay">TimeDelay для события.</param>
        /// <param name="size">Максимальный размер учасников.</param>
        /// <param name="isDestroy">Нужно ли уничтожить поток обрабатывающий событие если учасников не станет.</param>
        /// <param name="priority">Приоритет для потока.</param>
        public EventSetting(string name, uint timeDelay, uint size = 1024, bool isDestroy = false,
            Thread.Priority priority = Thread.Priority.Normal)
        {
            Name = name;
            TimeDelay = timeDelay;
            Size = size;
            IsDestroy = isDestroy;
            Priority = priority;
        }
    }
}