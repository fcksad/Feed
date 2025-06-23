using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;


public class DialogueSceneController : MonoBehaviour
{
    [SerializeField] private DialogueConfig _config;

    private IDialogueService _dialogueService;
    private CharacterInput _characterInput;

    private bool _isShowing = false;

    [Inject]
    private void Construct(IDialogueService dialogueService, CharacterInput input)
    {
        _dialogueService = dialogueService;
        _characterInput = input;
    }

    private void OnEnable()
    {
        _ = StartDialogue();
    }

    public async Task StartDialogue()
    {
        _isShowing = true;
        _characterInput.Lock(_isShowing);

        await _dialogueService.Show(_config, OnDialogueComplete);
    }

    private void OnDialogueComplete()
    {
        _isShowing = false;
        _characterInput.Lock(_isShowing);
    }

    private void OnDisable()
    {
        if (_isShowing)
        {
            _characterInput.Lock(_isShowing);
            _dialogueService.Stop();
        }
    }
}
