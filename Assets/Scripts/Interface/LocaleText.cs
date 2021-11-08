using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocaleText : MonoBehaviour
{
    [SerializeField]
    private string textID;

    private bool autoUpdate = true;

    private Text textComponent;
    private LocalizationManager _manager;

    private void Awake()
    {
        if (TryGetComponent(out Text text))
        {
            textComponent = text;
        }

        _manager = GameObject.Find("EventSystem").GetComponent<LocalizationManager>();

        if (autoUpdate)
            _manager.languageChanged += UpdateLocale;
    }

    private void Start()
    {
        UpdateLocale();
    }

    public void UpdateLocale()
    {
        try
        {
            string response = _manager.GetText(textID);
            if (response != null && textComponent != null)
                textComponent.text = response;
        }
        catch (NullReferenceException e)
        {
            Debug.Log(e);
        }
    }
}
