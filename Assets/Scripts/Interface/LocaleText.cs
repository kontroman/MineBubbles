using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocaleText : MonoBehaviour {

    [SerializeField]
    private string textID;
    [SerializeField]
    private bool autoUpdate = true;
    private Text textComponent;

    private void Awake () 
    {
        textComponent = GetComponent<Text>();

        if (autoUpdate == true) {
            LocalizationManager.Instance.languageChanged += UpdateLocale;
        }
    }

    private void Start () {
        UpdateLocale();
    }

    public void UpdateLocale () {
        try {
            string response = LocalizationManager.Instance.GetText(textID);
            if (response != null) {
                Debug.LogError("NOT NULL SOOKA: " + GetComponent<Text>().text);
                GetComponent<Text>().text = response;
            }
        }
        catch (NullReferenceException e) {
            Debug.LogError("NOT NULL SOOKA: " + GetComponent<Text>().text);
            Debug.Log(e);
        }
    }
}
