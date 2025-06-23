using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using static UnityEngine.Rendering.STP;

public class DialogueSceneController : MonoBehaviour
{
    [SerializeField] private DialogueConfig _config;

    private IDialogueService _dialogueService;
    private CharacterInput _characterInput;

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
        _characterInput.Lock(true);

        await _dialogueService.Show(_config, OnDialogueComplete);
    }

    private void OnDialogueComplete()
    {
        _characterInput.Lock(false);
    }
}
