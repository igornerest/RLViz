using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum Language
{
    English,
    Portuguese,
};

public class LocalizationLanguageManager : MonoBehaviour
{
    [SerializeField] private Image usFlagMaterial;
    [SerializeField] private Image brazilFlagMaterial;

    private LocalizationTable localizationTable = new LocalizationTable();

    private static Language selectedLanguage = Language.English;

    private static LocalizationLanguageManager current;

    public void Awake()
    {
        current = this;
    }

    public void SetEnglishLanguage()
    {
        selectedLanguage = Language.English;
        SetTransparency(usFlagMaterial, false);
        SetTransparency(brazilFlagMaterial, true);
    }

    public void SetPortugueseLanguage()
    {
        selectedLanguage = Language.Portuguese;
        SetTransparency(brazilFlagMaterial, false);
        SetTransparency(usFlagMaterial, true);
    }

    public static string Localize(string key)
    {
        LocalizationEntry entry = current.localizationTable[key];
        return selectedLanguage == Language.Portuguese ? entry.PortugueseEntry : entry.EnglishEntry;
    }

    private void SetTransparency(Image image, bool isTransparent)
    {
        Color color = image.color;
        color.a = isTransparent ? 0.5f: 1f;
        image.color = color;
    }
}
