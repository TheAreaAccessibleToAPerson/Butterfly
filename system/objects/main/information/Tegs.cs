namespace Butterfly.system.objects.main.information
{
    public class Tegs : Informing 
    {
        private readonly information.State _stateInformation;

        public Tegs(information.State stateInformation, informing.IMain informing)
            : base("TegsInformation", informing)
                => _stateInformation = stateInformation;

        private string[] _tegs = new string[0];

        public void Add(string teg)
        {
            if (_stateInformation.IsContruction)
            {
                for(int i = 0; i < _tegs.Length; i++)
                {
                    if (teg == _tegs[i])
                    {
                        Exception($"Вы дважды добавили один и тот же тег {teg}.");

                        return;
                    }
                }

                Hellper.ExpendArray(ref _tegs, teg);
            }
            else 
                Exception($"Добавить тег {teg} можно только в методе Construction.");
        }

        public bool Contains(string teg)
        {
            for (int i = 0; i < _tegs.Length; i++)
                if (teg == _tegs[i]) return true;

            return false;
        }
    }
}