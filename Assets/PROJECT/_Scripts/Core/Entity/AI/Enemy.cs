using Service;
using UnityEngine;
using Zenject;

public class Enemy : EntityBase
{
    [Header("Brain")]
    [field: SerializeField] public EntityBrain EntityBrain { get; private set; }
    [field: SerializeField] public EntityMoveController Controller {  get; private set; }


    [Inject]
    public void Construct(IAudioService audioService, ISurfaceAudioService surfaceAudioService)
    {
        _audioService = audioService;
    }

    protected override void Start()
    {
        base.Start();

        Controller.Initialize(_audioService, _surfaceAudioService);
    }

}
