using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ILocalizationService 
{
    string CurrentLanguageCode { get; }

    event Action OnLanguageChangedEvent;

    List<string> GetAvailableLanguages();
    public string GetLocalizedString(string key, string tableName = "DefaultTable");
    public Task SetLanguage(string localeCode);
}
