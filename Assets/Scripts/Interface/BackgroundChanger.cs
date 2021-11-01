using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundChanger : MonoBehaviour
{
    public static BackgroundChanger Instance { get; private set; }

    public UnityEngine.UI.RawImage img;
    private Texture2D backgroundTex;

    private void Awake()
    {
        if (Instance != null) return;
        else Instance = this;

        Init();
    }

    private void Init()
    {
        backgroundTex = new Texture2D(1, 2);
        backgroundTex.wrapMode = TextureWrapMode.Clamp;
        backgroundTex.filterMode = FilterMode.Bilinear;

        Color fisrtColor = GetColorFromHex(PlayerPrefs.GetString("FirstColor", "EEEEEE"));
        Color secondColor = GetColorFromHex(PlayerPrefs.GetString("SecondColor", "EEEEEE"));

        // first at the bottom, second at the top
        ChangeColor(fisrtColor, secondColor);
    }

    private void ChangeColor(Color first, Color second)
    {
        backgroundTex.SetPixels(new Color[] { first, second });
        backgroundTex.Apply();
        img.texture = backgroundTex;
    }

    public void SetNewColors(string bottomColor, string topColor)
    {
        PlayerPrefs.SetString("FirstColor", bottomColor);
        PlayerPrefs.SetString("SecondColor", topColor);

        ChangeColor(GetColorFromHex(bottomColor), GetColorFromHex(topColor));
    }

    private int HexToDec(string hex)
    {
        return System.Convert.ToInt32(hex, 16);
    }

    private float HexToFloatNormalized(string hex)
    {
        return HexToDec(hex) / 255f;
    }

    private Color GetColorFromHex(string hex)
    {
        float red = HexToFloatNormalized(hex.Substring(0, 2));
        float green = HexToFloatNormalized(hex.Substring(2, 2));
        float blue = HexToFloatNormalized(hex.Substring(4, 2));

        return new Color(red, green, blue);
    }

}
