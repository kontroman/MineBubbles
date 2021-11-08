using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class GridController : MonoBehaviour
{
    public static GridController Instance { get; private set; }

    public GameObject gridRowPrefab;

    public int startRowCount = GameConfig.START_ROW_COUNT;

    public int maxCellCount { get { return GameConfig.MAX_BALL_IN_ROW; } }

    private int _counter = 0;

    private void CreateFilledRowsAfterNewGame()
    {
        CreateFullRows(
            GameConfig.START_FILLER_ROW,
            () => { return GunController.Instance._QueueController.SpawnBall(
                GunController.Instance._QueueController._dummy,
                null
                ); },
            true);
    }

    void Awake()
    {
        if (Instance != null) return;
        else Instance = this;

        InvokeRepeating("DropAllDetached", 1f, 0.5f);
    }

    void Start()
    {
        CreateRows(GameConfig.START_ROW_COUNT);
        InvokeRepeating("CheckForUselessColors", 5f, 1f);
        CreateFilledRowsAfterNewGame();
    }
    private void CheckForUselessColors()
    {
        GunController.Instance._QueueController.ChangePallete();
    }

    public void CheckForWin()
    {
        if (AllRowsAreEmpty() && !LevelState.Instance.isGameOver)
        {
            LevelState.Instance._State = LevelState.State.GameWin;
        }
    }

    public void CreateFullRows(int amount, ReturnGameObjectDelegate spawnBall, bool deleteRows = true)
    {
        for (int i = 0; i < amount; i++)
        {
            ShiftRows(GameConfig.BALL_RADIUS * 2f);
            GameObject row = AddRow(_counter++ % 2);
            row.transform.SetAsFirstSibling();
            FillRow(row, spawnBall);

        }

        if (deleteRows)
            DeleteRows(amount);
    }

    public void DropAllDetached()
    {
        Queue<GameObject> queue = new Queue<GameObject>();
        List<GameObject> attached = new List<GameObject>();

        List<Collider2D> top = GetTopRowBalls();

        foreach (Collider2D coll in top)
        {
            attached.Add(coll.gameObject);
            queue.Enqueue(coll.gameObject);
        }

        while (queue.Count != 0)
        {
            GameObject ball = queue.Dequeue();
            List<Collider2D> neighbors = ball.GetComponent<BallController>().GetNeighbors();

            foreach (Collider2D n in neighbors)
            {
                if (n != ball.GetComponent<Collider2D>() && !attached.Contains(n.gameObject))
                {
                    attached.Add(n.gameObject);
                    queue.Enqueue(n.gameObject);
                }
            }
        }

        List<GameObject> balls = FindAllBallsOnLayer(GameConfig.Layers.DEFAULT);
        List<GameObject> toDrop = balls.Where(x => !attached.Contains(x)).ToList();

        if (toDrop.Count > 0)
        {
            ScoreController.Instance.AddScore(toDrop.Count);
            BallAnimator.Instance.DestroyBalls(toDrop);
        }
    }

    public bool LastRowIsNotEmpty()
    {
        Transform lastRow = gameObject.transform.GetChild(gameObject.transform.childCount - 1);

        foreach (Transform cell in lastRow)
        {
            if (cell.childCount != 0)
                return true;
        }

        return false;
    }

    public bool AllRowsAreEmpty()
    {
        List<Transform> rows = new List<Transform>();
        rows = GetAllChild(this.gameObject);

        if (rows.Count == 0)
            return true;

        foreach (Transform _go in rows)
        {
            List<Transform> cells = new List<Transform>();
            cells = GetAllChild(_go.gameObject);
            foreach (Transform _goc in cells)
            {
                if (_goc.transform.childCount != 0)
                    return false;
            }
        }
        return true;
    }

    public static List<Transform> GetAllChild(GameObject _go)
    {
        List<Transform> list = new List<Transform>();
        for (int i = 0; i < _go.transform.childCount; i++)
        {
            list.Add(_go.transform.GetChild(i).gameObject.transform);
        }
        return list;
    }

    private void FillRow(GameObject row, ReturnGameObjectDelegate spawnBall)
    {
        foreach (Transform cell in row.transform)
        {
            GameObject ball = spawnBall?.Invoke();
            TargetJoint2D joint = ball.GetComponent<TargetJoint2D>();

            ball.GetComponent<CircleCollider2D>().enabled = true;
            ball.GetComponent<BallController>().stationary = true;
            ball.layer = GameConfig.Layers.DEFAULT;

            StartCoroutine(EnableJointOnNextFrame(joint));

            joint.target = cell.position;

            ball.transform.SetParent(cell);
            ball.transform.position = cell.transform.position;
            ball.transform.localPosition = Vector2.zero;
        }
    }

    private IEnumerator EnableJointOnNextFrame(TargetJoint2D joint)
    {
        yield return null;

        joint.enabled = true;
    }

    private void DeleteRows(int amount)
    {
        int childCount = transform.childCount;

        for (int row = childCount - 1; amount > 0; amount--)
        {
            GameObject rowObj = transform.GetChild(row).gameObject;
            GameObject.Destroy(rowObj);
            row--;
        }
    }

    private void CreateRows(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject row = AddRow(_counter++ % 2);
            RectTransform rect = row.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(
                (i % 2 == 1) ? GameConfig.BALL_RADIUS : 0f,
                -(GameConfig.BALL_RADIUS * 2 + GameConfig.Spacings.Y_GRID_SPACING) * i
            );
        }
    }

    private GameObject AddRow(int offset)
    {
        GameObject row = GameObject.Instantiate(gridRowPrefab, this.gameObject.transform);

        row.transform.position = new Vector2(
            row.transform.position.x + (offset != 0 ? 0f : GameConfig.BALL_RADIUS),
            row.transform.position.y
        );

        row.GetComponent<GridRowController>().Setup(maxCellCount);

        StartCoroutine(CheckGameOverOnNextFrame());
        
        return row;
    }

    IEnumerator CheckGameOverOnNextFrame()
    {
        yield return null;
        GunController.Instance.CheckForGameOver();

        yield return new WaitForSeconds(0.1f);
        DropAllDetached();
    }

    public void ShiftRows(float yOffset)
    {
        foreach (Transform row in transform)
        {
            if (row == null) return;

            Vector3 target = new Vector2(
                row.transform.position.x,
                row.transform.position.y - yOffset
            );

            row.transform.position = target;
        }
    }

    private List<Collider2D> GetTopRowBalls()
    {
        List<Collider2D> balls = new List<Collider2D>();

        Transform topRow = transform.GetChild(0);

        foreach (Transform cell in topRow)
        {
            if (cell.childCount != 0)
            {
                Transform ball = cell.GetChild(0);
                if (ball.gameObject.layer == GameConfig.Layers.DEFAULT)
                {
                    balls.Add(ball.GetComponent<Collider2D>());
                }
            }
        }

        return balls;
    }

    public List<GameObject> FindAllBallsOnLayer(int index)
    {
        List<GameObject> balls = GameObject.FindGameObjectsWithTag(GameConfig.Tags.BALL).ToList();

        return balls.Where(b => b.layer == index).ToList();
    }

    public GridState SaveState()
    {
        var balls = new List<List<BallState>>();

        foreach (Transform rowTransform in transform)
        {
            var rowList = new List<BallState>();
            foreach (Transform cellTransform in rowTransform)
            {
                if (cellTransform.childCount == 1)
                {
                    var ball = cellTransform.GetChild(0).GetComponent<BallController>();
                    rowList.Add(ball.SaveState());
                }
                else
                {
                    rowList.Add(null);
                }
            }
            balls.Add(rowList);
        }

        return new GridState(balls);
    }

    public void RestoreFrom(GridState state, GameObject ballDummy)
    {
        int rowIdx = 0;
        foreach (Transform rowTransform in transform)
        {
            var rowList = state.balls[rowIdx++];
            int cellIdx = 0;
            foreach (Transform cellTransform in rowTransform)
            {
                BallState ballState = rowList[cellIdx++];
                if (cellTransform.childCount == 1)
                {
                    GameObject oldBall = cellTransform.GetChild(0).gameObject;
                    GameObject.Destroy(oldBall);
                }

                if (ballState is null)
                {
                    continue;
                }

                GameObject newBall = GameObject.Instantiate(ballDummy, cellTransform);
                BallController controller = newBall.GetComponent<BallController>();

                controller.RestoreFrom(ballState);
                controller.stationary = true;

                newBall.GetComponent<Collider2D>().enabled = true;
                newBall.GetComponent<TargetJoint2D>().enabled = true;
                newBall.layer = GameConfig.Layers.DEFAULT;
                newBall.SetActive(true);
            }
        }
    }

}