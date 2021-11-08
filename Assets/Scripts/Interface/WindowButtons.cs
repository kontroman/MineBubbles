using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class WindowButtons : MonoBehaviour
{
    public void Start()
    {
        SetupWindosState();
    }

    private void SetupWindosState()
    {
        if (gameObject.name == "Settings(Clone)")
        {
            UpdateHardModeIcon();
            UpdateVolumeIcon();
        }
    }

    public void Restart(int _changeDifficult) //0 if no change, 1 if change
    {
        RestartGame.Instance._RestartGame(Convert.ToBoolean(_changeDifficult));
    }

    public void Close()
    {
        WindowManager.Instance.CloseWindow(gameObject.GetComponent<Window>());
    }

    public void Quit()
    {
        WindowManager.Instance.CloseWindow(gameObject.GetComponent<Window>());
        Application.Quit();
    }

    public void PurchaseDisableAd()
    {
        //if (!Advertising.IsAdRemoved())
        //    InAppPurchasing.Purchase(EM_IAPConstants.Product_DisableAds);
    }

    public void RestorePurchases()
    {
        //InAppPurchasing.RestorePurchases();
    }

    public void Dis_EnableSound()
    {
        AudioController.Instance.Un_MuteAudio();

        UpdateVolumeIcon();
    }

    public void RateGameButton()
    {
        RateGame.Instance.ForceShowRatePopup();
    }

    public void HardmodeToggle()
    {
        //LevelState.Instance._State = LevelState.State.Restart;
        WindowManager.Instance.OpenWindow(WindowType.HardMod);

        UpdateHardModeIcon();
    }

    private void UpdateHardModeIcon()
    {
        if (LevelDifficult.Instance._Difficult == Difficult.Hard)
            GameObject.Find("HardmodeIcon").GetComponent<Image>().sprite =
                Resources.Load<Sprite>("setting/setting/ico_hard_on");
        else
            GameObject.Find("HardmodeIcon").GetComponent<Image>().sprite =
                Resources.Load<Sprite>("setting/setting/ico_hard_off");
    }

    private void UpdateVolumeIcon()
    {
        if (AudioController.Instance.IsMuted())
            GameObject.Find("SoundIcon").GetComponent<Image>().sprite =
                Resources.Load<Sprite>("setting/setting/ico_sound_off");
        else
            GameObject.Find("SoundIcon").GetComponent<Image>().sprite =
                Resources.Load<Sprite>("setting/setting/ico_sound_on");
    }

    public void PlayClickSound()
    {
        AudioController.Instance.PlaySound(SoundType.Click);
    }

    public void ShowTop10()
    {
        //EasyMobileController.Instance.ShowTop10UI();
        //GameServices.ShowLeaderboardUI();
    }

    public void OpenCandyCatLink()
    {
#if UNITY_IOS
        Application.OpenURL("itms-apps://itunes.apple.com/us/app/candy-cat-arcade-game/id1527859163");
return;
#endif
        Application.OpenURL("market://details?id=com.obodev.matcher");
    }


    public void UpdateLanguage()
    {
        int languageIndex = PlayerPrefs.GetInt("languageIndex", 0);

        languageIndex++;
        if (languageIndex >= 2) languageIndex = 0;

        string newLanguage = GetLanguage(languageIndex);

        LocalizationManager.Instance.SetNewLanguage(newLanguage);

        PlayerPrefs.SetInt("languageIndex", languageIndex);

        Flags.Instance.UpdateFlag(GetLanguage(languageIndex));
    }

    public void UpdateTexteurePack()
    {
        int texturePackIndex = PlayerPrefs.GetInt("TexturePackIndex", 1);

        texturePackIndex++;

        if (texturePackIndex >= 3) texturePackIndex = 1;

        PlayerPrefs.SetInt("TexturePackIndex", texturePackIndex);

        List<GameObject> balls = GridController.Instance.FindAllBallsOnLayer(GameConfig.Layers.DEFAULT);

        SpriteManager.Instance.ChangeDirectory();

        foreach (GameObject ball in balls)
        {
            ball.GetComponent<BallController>().LoadNewSprite(
                ball.GetComponent<BallController>().color
                );
        }
    }

    private string GetLanguage(int index)
    {
        return Flags.Instance._Flags.ElementAt(index).Key;
    }
}
