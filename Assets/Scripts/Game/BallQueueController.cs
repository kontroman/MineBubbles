using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public delegate void CreateRowsDelegate(int amount, ReturnGameObjectDelegate addBall, bool deleteRows);
public delegate GameObject ReturnGameObjectDelegate();
public delegate void GameObjectDelegate(GameObject obj);
public delegate void SaveStateDelegate();

public class BallQueueController
{
    private CreateRowsDelegate _createNFullRows;

    private GridLayoutGroup _queueGrid;
    private List<GameConfig.Colors> _colors;

    private GameObject _queueObject;
    private GameObject _ballPrefab;
    private GameObject _cellPrefab;

    public List<GameObject> _cells = new List<GameObject>();
    private List<GameConfig.Colors> availableColors;
    private int _currentCellsCount = GameConfig.DEFAULT_QUEUE_BALL_COUNT;
    private int _cellLimit = GameConfig.DEFAULT_QUEUE_BALL_COUNT;

    public GameObject _dummy { get; private set; }

    public BallQueueController(
        GameObject ballPrefab,
        GameObject cellPrefab,
        GameObject queueObject,
        CreateRowsDelegate CreateNFullRows
        )
    {
        this._queueGrid = queueObject.GetComponent<GridLayoutGroup>();
        this._colors = GameConfig.Colors.GetValues(typeof(GameConfig.Colors)).Cast<GameConfig.Colors>().ToList();
        availableColors = new List<GameConfig.Colors>(_colors);
        availableColors = _colors.ToList();

        this._ballPrefab = ballPrefab;
        this._cellPrefab = cellPrefab;
        this._queueObject = queueObject;

        this._createNFullRows = CreateNFullRows;

        this._dummy = SpawnBall(_ballPrefab, null);

        SetupDummy();
        SetupCell();

        for (int i = 0; i < 6; i++)
            _cells.Add(SpawnCell(_cellPrefab, _queueObject.transform));

        AddBall();
        AddBall();
    }

    public GameObject AddBall()
    {
        Transform cell = _cells.Find(c => c.transform.childCount == 0).transform;

        GameObject ball = SpawnBall(_dummy, cell);

        Vector2 ballScale = new Vector2(
            GameConfig.Multipliers.QUEUE_BALL_SIZE_MULTIPLIER,
            GameConfig.Multipliers.QUEUE_BALL_SIZE_MULTIPLIER
            );

        ball.transform.localScale = ballScale;

        return ball;
    }

    public GameObject SpawnBall(GameObject prefab, Transform parent)
    {
        GameObject ball = GameObject.Instantiate(prefab, parent, false);
        BallController controller = ball.GetComponent<BallController>();

        ball.SetActive(true);
        ball.transform.localPosition = Vector2.zero;
        ball.GetComponent<CircleCollider2D>().enabled = false;

        controller.Initialize(
            GetRandomColor(),
            GetRandomType()
            );

        return ball;
    }

    public GameObject GetBall(bool spawnAnother)
    {
        GameObject cell = _cells.Find(c => c.transform.childCount == 1);

        if (cell is null)
            return null;

        GameObject ball = cell.transform.GetChild(0).gameObject;

        ball.transform.SetParent(null);

        ball.transform.localScale = Vector3.one;

        _cells.RemoveAt(cell.transform.GetSiblingIndex());
        GameObject.Destroy(cell);

        if (_cells.Count < _currentCellsCount)
            _cells.Add(SpawnCell(_cellPrefab, _queueObject.transform));

        if (spawnAnother)
            AddBall();

        return ball;
    }

    public void ChangePallete()
    {
        List<GameObject> balls = GridController.Instance.FindAllBallsOnLayer(GameConfig.Layers.DEFAULT);

        foreach (GameConfig.Colors color in availableColors.ToList())
        {
            var ball = balls.Find(b => b.GetComponent<BallController>().color == color);
            if (ball == null)
                availableColors.Remove(color);
        }

        CheckGunAndQueueBalls();
    }

    private void CheckGunAndQueueBalls()
    {
        GameObject ball = _cells.Find(c => c.transform.childCount == 1);
        BallController bc = ball.GetComponentInChildren<BallController>();
        GameConfig.Colors color = bc.color;

        if (!availableColors.Contains(color))
        {
            color = availableColors[Random.Range(0, availableColors.Count)];
            bc.color = color;
            bc.LoadNewSprite(color);
        }

        GameObject gunBall = GunController.Instance._heldBall;
        BallController gbc = gunBall.GetComponentInChildren<BallController>();
        GameConfig.Colors gunColor = gbc.color;

        if (!availableColors.Contains(gunColor))
        {
            gunColor = availableColors[Random.Range(0, availableColors.Count)];
            gbc.color = gunColor;
            gbc.LoadNewSprite(gunColor);
        }
    }

    private GameConfig.Colors GetRandomColor()
    {
        return (GameConfig.Colors)Random.Range(0, availableColors.Count);
    }

    private GameConfig.Types GetRandomType()
    {
        return (GameConfig.Types)Random.Range(0, GameConfig.Types.GetNames(typeof(GameConfig.Types)).Length);
    }

    private void SetupDummy()
    {
        RectTransform rect = _dummy.GetComponent<RectTransform>();
        CircleCollider2D coll = _dummy.GetComponent<CircleCollider2D>();
        float radius = GameConfig.BALL_RADIUS;

        rect.sizeDelta = new Vector2(
            radius * 2,
            radius * 2
        );
        coll.radius = radius * GameConfig.Multipliers.BALL_COLLIDER_MULTIPLIER;

        _dummy.name = "dummy";
        _dummy.SetActive(false);
    }

    private GameObject SpawnCell(GameObject prefab, Transform parent)
    {
        return GameObject.Instantiate(prefab, parent);
    }

    private void SetupCell()
    {
        GridLayoutGroup grid = _queueObject.GetComponent<GridLayoutGroup>();
        float radius = GameConfig.BALL_RADIUS;
    }

    internal void DeleteCell()
    {
        GameObject last = _cells.Last();

        _cells.RemoveAt(_cells.Count - 1);
        GameObject.Destroy(last);

        ChangeCurrentCellCount();
        for (int i = _cells.Count; i < _currentCellsCount; i++)
        {
            _cells.Add(SpawnCell(_cellPrefab, _queueObject.transform));
        }
    }

    private void ChangeCurrentCellCount()
    {
        _currentCellsCount--;
        if (_currentCellsCount <= 0)
        {
            _cellLimit -= 2;
            if (_cellLimit <= 0)
            {
                _cellLimit = GameConfig.DEFAULT_QUEUE_BALL_COUNT;
            }
            _currentCellsCount = _cellLimit;

            _createNFullRows?.Invoke(
                2,
                () => { return SpawnBall(_dummy, null); },
                true
            );
        }
    }

    public QueueState SaveState()
    {
        return new QueueState(_cellLimit, _currentCellsCount, _cells, availableColors);
    }

    public void RestoreFrom(QueueState state)
    {
        _cellLimit = state.cellLimit;
        _currentCellsCount = state.currentCellCount;
        availableColors = state.colors;
        //_cells = new List<GameObject>();

        foreach (var cell in _cells)
        {
            GameObject.Destroy(cell);
        }
        _cells.Clear();

        for (int i = 0; i < _currentCellsCount; i++)
        {
            _cells.Add(SpawnCell(_cellPrefab, _queueObject.transform));
        }


        int j = 0;
        foreach (var ballState in state.balls)
        {
            GameObject ball = GameObject.Instantiate(_dummy, _cells[j++].transform, false);
            BallController controller = ball.GetComponent<BallController>();
            Vector2 newScale = new Vector2(
                GameConfig.Multipliers.QUEUE_BALL_SIZE_MULTIPLIER,
                GameConfig.Multipliers.QUEUE_BALL_SIZE_MULTIPLIER
            );

            ball.SetActive(true);
            ball.transform.localPosition = Vector2.zero;
            ball.transform.localScale = newScale;
            ball.GetComponent<CircleCollider2D>().enabled = false;

            controller.Initialize(ballState.color, ballState.type);
        }
    }
}