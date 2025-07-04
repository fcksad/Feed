using UnityEngine;
using Zenject;

public class Character : EntityBase
{
    [Header("Model")]
    [field: SerializeField] public Transform HeadRoot { get; private set; }
    [field: SerializeField] public Transform CameraRoot { get; private set; }
    [field: SerializeField] public Camera Camera { get; private set; }

    [Header("Move")]
    public CharacterInput CharacterInput;
    [field: SerializeField] public CharacterController Controller { get; private set; }

    [Header("Interact")]
    [field: SerializeField] public InteractController InteractController { get; private set; }
    [field: SerializeField] public GrabController GrabController { get; private set; }
    [field: SerializeField] public ItemController ItemController { get; private set; }
    [field: SerializeField] public FlashlightController FlashlightController { get; private set; }
    [field: SerializeField] public HandAnimationController HandAnimationController { get; private set; }

    private ISaveService _saveService;

    [Inject]
    public void Construct(CharacterInput characterInput, IAudioService audioService, ISaveService saveService)
    {
        CharacterInput = characterInput;
        _audioService = audioService;
        _saveService = saveService;
    }

    protected override void Start()
    {
        base.Start();

        InteractController.Initialize(GrabController, ItemController, Camera);
        ItemController.Initialize(Camera, HandAnimationController);
        FlashlightController.Initialize();
        CharacterInput.Initialize();
        Controller.Initialize(_audioService, this, _saveService.SettingsData.CharacterSettingsData.Sensitivity);

        CharacterInput.Bind(Controller);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        InteractController.Dispose();
        ItemController.Dispose();
        FlashlightController.Dispose();
        CharacterInput.Dispose();
    }

}
