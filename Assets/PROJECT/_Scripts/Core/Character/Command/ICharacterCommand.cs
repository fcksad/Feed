public interface ICharacterCommand
{
    void Execute();
    bool IsFinished { get; }
}
