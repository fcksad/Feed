using UnityEngine;
using TMPro;
using UnityEngine.Localization;

[RequireComponent(typeof(TMP_Text))]
public class TextButtonListener : MonoBehaviour
{
    [SerializeField] private LocalizedString _localizedString;
    [SerializeField] private TMP_Text _targetText;

    private void Awake()
    {
        if (_localizedString != null)
        {
            _localizedString.StringChanged += UpdateText;
            _localizedString.RefreshString();
        }
        else
        {
            Debug.LogWarning("LocalizedString is not assigned!", this);
        }
    }

    private void UpdateText(string localizedText)
    {
        if (_targetText != null)
            _targetText.text = localizedText;
    }

    private void OnDestroy()
    {
        if (_localizedString != null)
            _localizedString.StringChanged -= UpdateText;
    }

    private void OnValidate()
    {
        if (_targetText == null)
        {
            _targetText = GetComponent<TMP_Text>();
        }
    }
}
