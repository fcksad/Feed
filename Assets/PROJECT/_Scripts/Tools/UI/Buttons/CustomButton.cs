using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CustomButton : MonoBehaviour
{
    public Button Button => _button != null ? _button : _button = GetComponent<Button>();
    private Button _button;

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
    }
#endif

}
