using UnityEngine;
using Zenject;


public class DialogueSceneController : MonoBehaviour
{
    [SerializeField] private DialogueConfig _config;

    private IDialogueService _dialogueService;

    private bool _isShowing = false;

    [Inject]
    public void Construct(IDialogueService dialogueService)
    {
        _dialogueService = dialogueService;
    }

    [ContextMenu("Start Dialogue")]
    public void StartDialogue()
    {
        _isShowing = true;

        _dialogueService.Show(_config, OnDialogueComplete);
    }

    private void OnDialogueComplete()
    {
        _isShowing = false;
    }

    private void OnDisable()
    {
        if (_isShowing)
        {
            _dialogueService.Stop();
        }
    }
}
