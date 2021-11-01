using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelState : MonoBehaviour
{
    public static LevelState Instance { get; private set; }

    public delegate void OnStateChangedDelegae(State newState);
    public event OnStateChangedDelegae OnStateChanged;

    public bool isGameOver = false;

    public enum State
    {
        Default,
        InProgress,
        Restart,
        GameOver,
        GameWin
    }

    private State _state;
    public State _State
    {
        get { return _state; }
        set
        {
            _state = value;
            if (OnStateChanged != null)
                OnStateChanged(_state);
        }
    }

    private void Start()
    {
        if (Instance != null) return;
        else Instance = this;

        this.OnStateChanged += StateChangeHandler;

        Init();
    }

    public void Init()
    {
        isGameOver = false;
        _State = State.Default;
    }

    private void StateChangeHandler(State newState)
    {
        switch (_state)
        {
            case State.Default:
                Time.timeScale = 1;
                break;
            case State.GameOver:
                isGameOver = true;
                StartCoroutine(ResetLevelGrid());
                ScoreController.Instance.CheckAndSetNewMaxScore();
                AudioController.Instance.PlaySound(SoundType.Lose);
                WindowManager.Instance.OpenWindow(WindowType.GameOver);
                break;
            case State.Restart:
                Time.timeScale = 1;
                WindowManager.Instance.OpenWindow(WindowType.Restart);
                break;
            case State.GameWin:
                isGameOver = true;
                ScoreController.Instance.CheckAndSetNewMaxScore();
                StartCoroutine(ResetLevelGrid());
                AudioController.Instance.PlaySound(SoundType.Sgo);
                WindowManager.Instance.OpenWindow(WindowType.GameWin);
                break;
        }
    }

    IEnumerator ResetLevelGrid()
    {
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 0;
    }

    public State GetLevelState()
    {
        return _state;
    }
}
