using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocalizationLanguageManager : MonoBehaviour
{
    [SerializeField] private Image usFlagMaterial;
    [SerializeField] private Image brazilFlagMaterial;

    public void SetEnglishLanguage()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        SetTransparency(usFlagMaterial, false);
        SetTransparency(brazilFlagMaterial, true);
    }

    public void SetPortugueseLanguage()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        SetTransparency(brazilFlagMaterial, false);
        SetTransparency(usFlagMaterial, true);
    }

    public static string GetLocalizedName(string tableName, string key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key);
    }

    private void SetTransparency(Image image, bool isTransparent)
    {
        Color color = image.color;
        color.a = isTransparent ? 0.5f: 1f;
        image.color = color;
    }
}
