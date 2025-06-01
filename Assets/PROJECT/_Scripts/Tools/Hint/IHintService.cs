using System.Collections.Generic;

namespace Service
{
    public interface IHintService 
    {
        public void ShowHint(List<CharacterAction> actions);
        void HideHint(List<CharacterAction> actions);
        void HideAll();
    }
}
  
