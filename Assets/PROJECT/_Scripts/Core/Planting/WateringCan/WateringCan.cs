using System.Collections;
using UnityEngine;
using Zenject;

public class WateringCan : GrabbableObject
{
    [SerializeField] private AudioConfig _pourSound;
    [SerializeField] private float _pourMinAngle = 18f;
    [SerializeField] private float _pourMaxAngle = 170f;

    private bool _isPouring;
    private Coroutine _monitorRoutine;

    protected IAudioService _audioService;

    [Inject]
    public void Construct(IAudioService audioService)
    {
        _audioService = audioService;
    }

    public override void OnGrab()
    {
        _monitorRoutine = StartCoroutine(MonitorTransform());
    }

    public override void OnDrop()
    {
        if (_monitorRoutine != null)
        {
            StopCoroutine(_monitorRoutine);
            _monitorRoutine = null;
        }

        _audioService.Stop(_pourSound);
    }

    private IEnumerator MonitorTransform()
    {
        Vector3 last = transform.eulerAngles;

        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (transform.eulerAngles != last)
            {
                last = transform.eulerAngles;
                CheckPouringState();
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    private void CheckPouringState()
    {
        float xAngle = NormalizeAngle(transform.eulerAngles.x);
        bool shouldPour = xAngle > _pourMinAngle && xAngle < _pourMaxAngle;

        if (shouldPour != _isPouring)
        {
            SetPouring(shouldPour);
        }
    }

    private void SetPouring(bool value)
    {
        _isPouring = value;

        if (_isPouring)
        {
            _audioService.Play(_pourSound, loop: true, parent: transform);
        }
        else
        {
            _audioService.Stop(_pourSound);
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }



}
