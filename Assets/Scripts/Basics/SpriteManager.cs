using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance { get; private set; }

    private string _dir = "Balls/ColoredBalls/";

    private void Awake()
    {
        if (Instance != null) return;
        else Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public Sprite LoadBall(GameConfig.Colors color)
    {
        return Resources.Load<Sprite>(_dir + GameConfig.ColorNames[(int)color]);
    }
}
