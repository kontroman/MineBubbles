using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BubbleShooter : MonoBehaviour
{
    public static BubbleShooter Instance { get; private set; }

    #region LayoutObjects
    public GameObject canvas;
    public GameObject grid;
    public GameObject ceiling;
    public GameObject leftwall;
    public GameObject rightWall;
    public GameObject gunAnchor;
    public GameObject queueHolder;
    public GameObject resetButton;
    public GameObject restartButton;
    public GameObject settingsButton;
    public GameObject scoreText;
    public GameObject border;
    #endregion

    private ILayout _layout;
    private GunController _gunController;
    public GunController _GunController { get { return _gunController; } }

    public HistoryHolder _historyHolder;

    private void Awake()
    {
        if (Instance != null) return;
        else Instance = this;

        Init();
    }

    private void Init()
    {
        _layout = LayoutFactory.Create(
            canvas,
            grid,
            ceiling,
            leftwall,
            rightWall,
            gunAnchor,
            queueHolder,
            resetButton,
            restartButton,
            settingsButton,
            scoreText,
            border
        );

        _historyHolder = new HistoryHolder();
        _layout.Setup();
        _gunController = gunAnchor.GetComponent<GunController>();
        _gunController.Setup(grid, _historyHolder.SaveState);
    }
    void Start()
    {
        RateGame.Instance.ShowRatePopup();

        _historyHolder.Setup(_gunController, grid);
    }

    public void UndoMove()
    {
        _historyHolder.RestoreLastState();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenSettings()
    {
        WindowManager.Instance.OpenWindow(WindowType.Settings);
    }
}
