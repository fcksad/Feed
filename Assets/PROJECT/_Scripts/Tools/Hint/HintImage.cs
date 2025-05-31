using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintImage : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;

    public void Set(string controlButton, Sprite icon)
    {
        if (icon == null)
        {
            _text.SetText(controlButton.ToUpperInvariant());
            _text.enabled = true;
            _image.enabled = false;
        }
        else
        {
            _image.sprite = icon;
            _image.enabled = true;
            _text.enabled = false;
        }
    }
}
