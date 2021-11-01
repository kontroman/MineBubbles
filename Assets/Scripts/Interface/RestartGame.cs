using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public static RestartGame Instance { get; private set; }

    private void Start()
    {
        if (Instance != null) return;
        else Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            _RestartGame(false);
    }

    public void _RestartGame(bool _changeGameMode)
    {
        if (_changeGameMode)
            LevelDifficult.Instance.InvokeDifficult();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void _RestartGameWithWindow()
    {
        WindowManager.Instance.OpenWindow(WindowType.Restart);
    }
}
