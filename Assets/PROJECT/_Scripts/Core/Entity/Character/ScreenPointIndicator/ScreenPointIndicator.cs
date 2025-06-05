using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenPointIndicator : MonoBehaviour
{
    [SerializeField] private Image _dot;
    [SerializeField] private TextMeshProUGUI _targetName;
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _targetColor = Color.red;

    public void SetDefault()
    {
        _dot.color = _defaultColor;
        _targetName.SetText("");
    }
    public void SetTargeted(string targetName)
    {
        _dot.color = _targetColor;
        _targetName.SetText(targetName);
    }
}
