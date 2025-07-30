using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CustomButton : MonoBehaviour
{
    public Button Button => _button;
    private Button _button;

    private void OnValidate()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
    }


}
