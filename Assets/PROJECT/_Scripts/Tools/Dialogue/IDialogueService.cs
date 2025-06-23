using System;
using System.Threading.Tasks;

public interface IDialogueService
{
    Task Show(DialogueConfig config, Action onCompleted);
}
