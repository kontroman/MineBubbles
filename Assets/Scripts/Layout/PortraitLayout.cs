using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitLayout : ILayout
{
    private GameObject _canvas;
    private GameObject _grid;
    private GameObject _ceiling;
    private GameObject _leftWall;
    private GameObject _rightWall;
    private GameObject _gunAnchor;
    private GameObject _ballQueue;
    private GameObject _resetButton;
    private GameObject _restartButton;
    private GameObject _settingsButton;
    private GameObject _scoreText;

    public PortraitLayout(
        GameObject canvas,
        GameObject grid,
        GameObject ceiling,
        GameObject leftWall,
        GameObject rightWall,
        GameObject gunAnchor,
        GameObject ballQueue,
        GameObject resetButton,
        GameObject restartButton,
        GameObject settingsButton,
        GameObject scoreText
        )
    {
        this._canvas = canvas;
        this._grid = grid;
        this._ceiling = ceiling;
        this._leftWall = leftWall;
        this._rightWall = rightWall;
        this._gunAnchor = gunAnchor;
        this._ballQueue = ballQueue;
        this._resetButton = resetButton;
        this._restartButton = restartButton;
        this._settingsButton = settingsButton;
        this._scoreText = scoreText;
    }
    private int CalculateRowCount()
    {
        return (int)((Screen.height - (Screen.height / 10)) / (Screen.width / GameConfig.MAX_BALL_IN_ROW));
    }

    public void Setup()
    {
        if (Mathf.Abs(Camera.main.aspect - 0.75f) < 0.0001f) //if tablet
        {
            GameConfig.START_FILLER_ROW = 11;
            GameConfig.MAX_BALL_IN_ROW = 17;
            GameConfig.START_ROW_COUNT = CalculateRowCount();
        }
        else
        {
            GameConfig.START_FILLER_ROW = 12;
            GameConfig.MAX_BALL_IN_ROW = 13;
            GameConfig.START_ROW_COUNT = CalculateRowCount();
        }

        GameConfig.BALL_RADIUS = CalculateRadius();
        SetupGunAnchor();
        SetupCeilingAndWalls();
        SetupGrid();
        SetupBallQueue();
        SetupIntefaceButtons();
    }

    private void SetupGunAnchor()
    {
        RectTransform _rect = _gunAnchor.GetComponent<RectTransform>();

        _rect.anchorMin = new Vector2(0.5f, 0);
        _rect.anchorMax = new Vector2(0.5f, 0);

        _rect.pivot = new Vector2(0.5f, 0.5f);

        _rect.anchoredPosition = new Vector2(0, 230);
    }

    private void SetupCeilingAndWalls()
    {
        RectTransform _ceilingRect = _ceiling.GetComponent<RectTransform>();
        RectTransform _leftWallRect = _leftWall.GetComponent<RectTransform>();
        RectTransform _rightWallRect = _rightWall.GetComponent<RectTransform>();

        _ceilingRect.anchorMin = new Vector2(0.5f, 1);
        _ceilingRect.anchorMax = new Vector2(0.5f, 1);
        _ceilingRect.pivot = new Vector2(0.5f, 0.5f);
        _ceilingRect.anchoredPosition = new Vector2(0, 0);

        BoxCollider2D ceilingCollider = _ceiling.GetComponent<BoxCollider2D>();
        ceilingCollider.enabled = true;
        ceilingCollider.size = new Vector2(Screen.width, GameConfig.WALLS_COLLIDER_SIZE);

        /* -------- */ 

        _leftWallRect.anchorMin = new Vector2(0, 0.5f);
        _leftWallRect.anchorMax = new Vector2(0, 0.5f);
        _leftWallRect.pivot = new Vector2(0.5f, 0.5f);
        _leftWallRect.anchoredPosition = new Vector2(0, 0);

        BoxCollider2D leftWallCollider = _leftWall.GetComponent<BoxCollider2D>();
        leftWallCollider.enabled = true;
        leftWallCollider.size = new Vector2(GameConfig.WALLS_COLLIDER_SIZE, Screen.height);

        /* -------- */

        _rightWallRect.anchorMin = new Vector2(1, 0.5f);
        _rightWallRect.anchorMax = new Vector2(1, 0.5f);
        _rightWallRect.pivot = new Vector2(0.5f, 0.5f);
        _rightWallRect.anchoredPosition = new Vector2(0, 0);

        BoxCollider2D rightWallCollider = _rightWall.GetComponent<BoxCollider2D>();
        rightWallCollider.enabled = true;
        rightWallCollider.size = new Vector2(GameConfig.WALLS_COLLIDER_SIZE, Screen.height);

    }

    private void SetupGrid()
    {
        RectTransform gridRect = _grid.GetComponent<RectTransform>();

        gridRect.anchorMin = new Vector2(0, 1);
        gridRect.anchorMax = new Vector2(0, 1);
        gridRect.pivot = new Vector2(0, 1);

        gridRect.anchoredPosition = new Vector2(0,0);
    }

    private void SetupBallQueue()
    {
        RectTransform _rect = _ballQueue.GetComponent<RectTransform>();
        UnityEngine.UI.GridLayoutGroup grid = _ballQueue.GetComponent<UnityEngine.UI.GridLayoutGroup>();

        grid.cellSize = new Vector3(GameConfig.BALL_RADIUS * 2, GameConfig.BALL_RADIUS * 2, 0);

        _rect.anchorMin = Vector2.zero;
        _rect.anchorMax = Vector2.zero;

        _rect.pivot = new Vector2(0.5f, 0.5f);

        _rect.sizeDelta = new Vector2(0, 0);

        _rect.anchoredPosition = new Vector2(10, GameConfig.BALL_RADIUS * 2 + 10);
    }

    private void SetupIntefaceButtons()
    {
        RectTransform _reset = _resetButton.GetComponent<RectTransform>();
        RectTransform _restart = _restartButton.GetComponent<RectTransform>();
        RectTransform _settings = _settingsButton.GetComponent<RectTransform>();
        RectTransform _score = _scoreText.GetComponent<RectTransform>();

        _reset.anchoredPosition = new Vector2(_reset.sizeDelta.x / 2 +10, _reset.sizeDelta.x + GameConfig.BALL_RADIUS + 10);

        _restart.anchoredPosition = new Vector2(-_restart.sizeDelta.x / 2 - 10, _restart.sizeDelta.x / 2 + 10);
        _settings.anchoredPosition = new Vector2(-_settings.sizeDelta.x * 1.5f - 20, _settings.sizeDelta.x / 2 + 10);

        _score.anchoredPosition = new Vector2(-110, _restart.sizeDelta.x + 50);
    }

    public Vector2 ScreenScale()
    {
        return Vector2.zero;
    }

    public float CalculateRadius()
    {
        int ballCount = GameConfig.MAX_BALL_IN_ROW;

        RectTransform leftRect = _leftWall.GetComponent<RectTransform>();
        RectTransform rightRect = _rightWall.GetComponent<RectTransform>();

        return (Screen.width - leftRect.rect.width - rightRect.rect.width - (5 * GameConfig.MAX_BALL_IN_ROW)) / 2 / (ballCount + 0.5f);
    }
}
