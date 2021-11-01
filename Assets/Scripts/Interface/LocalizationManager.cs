using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    private Dictionary<string, string> currentLanguageTexts;

    private string defaultLanguage = "English";
    private string currentLanguage;

    public delegate void LanguageChangedEventHandler();
    public event LanguageChangedEventHandler languageChanged;

    private void Awake()
    {
        Instance = this;

        Init();
    }


    private void Init()
    {
        if (PlayerPrefs.HasKey("LastLanguage"))
        {
            string newLanguage = PlayerPrefs.GetString("LastLanguage");
            try
            {
                SetNewLanguage(newLanguage);
            }
            catch (Exception e)
            {
                SetNewLanguage(defaultLanguage);
            }
        }
        else
        {
            SaveLanguage();
            SetNewLanguage(defaultLanguage);
        }
    }

    public string GetText(string _text)
    {
        if (!currentLanguageTexts.ContainsKey(_text))
            return null;

        return currentLanguageTexts[_text];
    }

    public void SetNewLanguage(string _language)
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Locale_" + _language);
        if (textAsset != null)
        {
            currentLanguageTexts = JsonConvert.DeserializeObject<Dictionary<string, string>>(textAsset.text);
            currentLanguage = _language;
            SaveLanguage();
            onLanguageChanged();
        }
        else
        {
            throw new Exception("Localization Error!: " + _language + " does not have a .txt resource!");
        }
    }

    private void SaveLanguage()
    {
        PlayerPrefs.SetString("LastLanguage", currentLanguage);
    }

    private void OnApplicationQuit()
    {
        SaveLanguage();
    }

    protected virtual void onLanguageChanged()
    {
        if (languageChanged != null)
            languageChanged();
    }
}
