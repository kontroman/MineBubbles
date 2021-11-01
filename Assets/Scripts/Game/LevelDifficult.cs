using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficult
{
    Normal,
    Hard
}

public class LevelDifficult : MonoBehaviour
{
    public static LevelDifficult Instance { get; private set; }

    public delegate void OnDiffictulChangeDelegate(Difficult newDifficult);
    public event OnDiffictulChangeDelegate OnDifficultChange;

    public Difficult _difficult;
    public Difficult _Difficult
    {
        get { return _difficult; }
        set
        {
            _difficult = value;
            if (OnDifficultChange != null)
                OnDifficultChange(_difficult);
        }
    }

    private void Awake()
    {
        if (Instance != null) return;
        else Instance = this;

        this.OnDifficultChange += DifficultChangeHandler;

        LoadDifficult();
    }

    private void LoadDifficult()
    {
        _Difficult = (Difficult)PlayerPrefs.GetInt("Difficult", 0);
    }

    private void DifficultChangeHandler(Difficult newDifficult)
    {
        SaveDifficult();
    }
    private void SaveDifficult()
    {
        PlayerPrefs.SetInt("Difficult", (int)_Difficult);
    }

    public void InvokeDifficult()
    {
        if (_Difficult == Difficult.Hard)
            _Difficult = Difficult.Normal;
        else _Difficult = Difficult.Hard;
    }
}
