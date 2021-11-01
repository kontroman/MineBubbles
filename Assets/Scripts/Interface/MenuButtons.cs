using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public WindowManager windowManager;

    public GameObject menu;

    public Image hardModButtonIcon;

    public Sprite hardModOn;
    public Sprite hardModOff;

    [SerializeField]
    private List<string> languages;
    private int index = 0;


    private void Update()
    {
    }

    public void RateButtonClick()
    {
        RateGame.Instance.ShowRatePopup();
    }

    public void HardModButtonClick()
    {
        windowManager.OpenWindow(WindowType.HardMod);  
    }

    public void ResumeGame()
    {
        menu.SetActive(false);
    }

    public void SwitchHardModButtonIcon()
    {
        if (LevelDifficult.Instance._Difficult == Difficult.Hard)
            hardModButtonIcon.sprite = hardModOn;
        else
            hardModButtonIcon.sprite = hardModOff;
    }
}
