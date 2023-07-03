namespace Butterfly.system.objects.main
{
    public interface IInformation
    {
        /// <summary>
        /// Хранит адрес обьекта в системе.
        /// </summary>
        /// <returns></returns>
        public string GetExplorer();

        /// <summary>
        /// Хранит ID обьекта в нутри которого был создан.
        /// </summary>
        /// <returns></returns>
        public ulong GetID();

        /// <summary>
        /// Хранит ID узла в нутри которого был создан. 
        /// </summary>
        /// <returns></returns>
        public ulong GetNodeID();

        /// <summary>
        /// Возращает ключ по которому был создан обьект. 
        /// </summary>
        /// <returns></returns>
        public string GetKey();

        /// <summary>
        /// Получает доступ к менеджеру глобальных обьектов. 
        /// </summary>
        /// <returns></returns>
        public manager.IGlobalObjects GetGlobalObjectsManager();
    }
}