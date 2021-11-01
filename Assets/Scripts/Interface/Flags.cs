using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flags : MonoBehaviour
{
    public static Flags Instance { get; private set; }

    public Dictionary<string, Sprite> _Flags = new Dictionary<string, Sprite>(7);

    void Awake()
    {
        if (Instance != null) return;
        else Instance = this;

        Init();
    }

    private void Init()
    {
        _Flags.Add("English", Resources.Load<Sprite>("Flags/usa"));
        _Flags.Add("Russian", Resources.Load<Sprite>("Flags/russia"));
        _Flags.Add("German", Resources.Load<Sprite>("Flags/germany"));
        _Flags.Add("French", Resources.Load<Sprite>("Flags/france"));
        _Flags.Add("Italy", Resources.Load<Sprite>("Flags/italy"));
        _Flags.Add("Spanish", Resources.Load<Sprite>("Flags/spain"));
        _Flags.Add("Portugese", Resources.Load<Sprite>("Flags/portygal"));

        UpdateFlag(PlayerPrefs.GetString("LastLanguage"));
    }

    public void UpdateFlag(string _flag)
    {
        GetComponent<Image>().sprite = _Flags[_flag];
    }
}
