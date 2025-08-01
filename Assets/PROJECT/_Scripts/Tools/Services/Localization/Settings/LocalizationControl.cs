using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class LocalizationControl : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _languageDropdown;

    private ILocalizationService _localizationService;
    private List<Locale> _availableLocales = new();

    [Inject]
    public void Construct(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    private void Awake()
    {
        _languageDropdown.ClearOptions();

        _availableLocales = LocalizationSettings.AvailableLocales.Locales;

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (var locale in _availableLocales)
        {
            options.Add(new TMP_Dropdown.OptionData(locale.LocaleName));
        }

        _languageDropdown.AddOptions(options);

        var currentLocale = LocalizationSettings.SelectedLocale;
        int currentIndex = _availableLocales.IndexOf(currentLocale);
        if (currentIndex >= 0)
        {
            _languageDropdown.value = currentIndex;
            _languageDropdown.RefreshShownValue();
        }

        _languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    private async void OnLanguageChanged(int selectedIndex)
    {
        if (selectedIndex >= 0 && selectedIndex < _availableLocales.Count)
        {
            string selectedLanguageCode = _availableLocales[selectedIndex].Identifier.Code;
            await _localizationService.SetLanguage(selectedLanguageCode);
        }
    }

    private void OnDestroy()
    {
        _languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
    }
}
