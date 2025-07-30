using Service;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;


public class DialogueSceneController : MonoBehaviour
{
    [SerializeField] private DialogueConfig _config;

    private IDialogueService _dialogueService;
    private IInputService _inputService;

    private bool _isShowing = false;

    [Inject]
    private void Construct(IDialogueService dialogueService, IInputService inputService)
    {
        _dialogueService = dialogueService;
        _inputService = inputService;
    }

    private void OnEnable()
    {
        _ = StartDialogue();
    }

    public async Task StartDialogue()
    {
        _isShowing = true;
        //_characterInput.Lock(_isShowing);
        _inputService.ChangeInputMap(InputMapType.UI);

        await _dialogueService.Show(_config, OnDialogueComplete);
    }

    private void OnDialogueComplete()
    {
        _isShowing = false;
        _inputService.ChangeInputMap(InputMapType.Player);
    }

    private void OnDisable()
    {
        if (_isShowing)
        {
            //_characterInput.Lock(_isShowing);
            _inputService.ChangeInputMap(InputMapType.Player);
            _dialogueService.Stop();
        }
    }
}
