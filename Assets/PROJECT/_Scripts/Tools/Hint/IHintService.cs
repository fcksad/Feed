namespace Service
{
    public interface IHintService 
    {
        public void ShowHint(CharacterAction action);
        void HideHint(CharacterAction action);
        void HideAll();
    }
}
  
