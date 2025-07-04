using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using Localization;

[RequireComponent(typeof(TMP_Text))]
public class TextButtonListener : MonoBehaviour
{
    [SerializeField] protected LocalizedName _localizedName;
    [SerializeField] private TMP_Text _targetText;

    private void Awake()
    {
        if (_localizedName != null)
        {
            _localizedName.Init(UpdateText);
        }
    }

    private void UpdateText(string localizedText)
    {
        if (_targetText != null)
            _targetText.text = localizedText;
    }

    private void OnDestroy()
    {
        if (_localizedName != null)
            _localizedName.Dispose();
    }

    private void OnValidate()
    {
        if (_targetText == null)
        {
            _targetText = GetComponent<TMP_Text>();
        }
    }
}
