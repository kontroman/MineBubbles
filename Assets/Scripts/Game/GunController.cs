using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

using UnityEngine.SceneManagement;

public class GunController : MonoBehaviour
{
    public static GunController Instance { get; private set; }

    #region GunVariables
    private GameObject _anchor;
    private GameObject _centerPoint;
    private GameObject _queueHolder;
    public GameObject _heldBall;
    private Vector2 _centerPos;
    private Vector2 _direction = Vector2.up;
    private Vector2 _PrevDirection = Vector2.up;
    private float time = 0.1f;
    private float _ballSpeed = GameConfig.DEFAULT_BALL_SPEED;
    public bool _canShoot = false;
    private int _counter = 0;
    public GameObject ballPrefab;
    public GameObject queueCellPrefab;
    #endregion

    #region OtherVariables
    private GameObject _canvas;
    private RayRenderer _rayRenderer;
    public BallQueueController _queueController;
    private GridController _gridController;
    private BubbleShooter _bubbleShooter;
    private SaveStateDelegate _saveState;
    public RayRenderer _Ray { get { return _rayRenderer; } }
    public BallQueueController _QueueController { get { return _queueController; } }
    #endregion

    public void Setup(GameObject grid, SaveStateDelegate saveState)
    {
        this._gridController = grid.GetComponent<GridController>();
        this._saveState = saveState;
        this.gameObject.SetActive(true);
        this._bubbleShooter = GameObject.Find("BubbleShooter").GetComponent<BubbleShooter>();
    }

    void Awake()
    {
        if (Instance != null) return;
        else Instance = this;

        Init();
    }

    void Init()
    {
        this._canvas = GameObject.Find("Canvas");
        this._anchor = this.gameObject;
        this._centerPoint = _anchor.transform.Find("CenterPoint").gameObject;
        this._queueHolder = GameObject.Find("QueueHolder");
        this._centerPos = this._centerPoint.transform.position;

        this._rayRenderer = new RayRenderer(this.GetComponent<UILineRenderer>());

        this._queueController = new BallQueueController(
            ballPrefab,
            queueCellPrefab,
            _queueHolder,
            _gridController.CreateFullRows
        );

        GetBallFromQueue(false);
        _Ray.SetupRay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // TODO: delete
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (Input.GetMouseButtonUp(0))
        {
            if (!GunDisabler.Instance.CanShoot()) return;

            Shoot();
            GunDisabler.Instance.AddAction("Reloading");
        }

            time -= Time.deltaTime;

        if (time <= 0)
        {
            _PrevDirection = _direction;
            time = 0.1f;
        }

        _direction = ÑlippedDirection((Vector2)Input.mousePosition - _centerPos);

        if (_direction.y < 50f)
            _direction = _PrevDirection;

        _rayRenderer.CastRay(_centerPos, _direction, 0);
    }

    private Vector2 ÑlippedDirection(Vector2 direction)
    {
        float angle = Vector2.SignedAngle(direction, Vector2.up);
        float clipAngle = GameConfig.Angles.MIN_SHOOTING_ANGLE;

        if (angle > 90 - clipAngle)
        {
            clipAngle *= Mathf.Deg2Rad;
            return new Vector2((float)Mathf.Cos(clipAngle), (float)Mathf.Sin(clipAngle));
        }
        if (angle < -90 + clipAngle)
        {
            clipAngle *= Mathf.Deg2Rad;
            return new Vector2(-(float)Mathf.Cos(clipAngle), (float)Mathf.Sin(clipAngle));
        }

        return direction;
    }

    private void Shoot()
    {
        AudioController.Instance.PlaySound(SoundType.Shoot);

        this._heldBall.GetComponent<CircleCollider2D>().radius /= 2;

        Vector2 velocity = _PrevDirection.normalized * _ballSpeed;
        GameObject ball = this._heldBall;
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        BallController controller = ball.GetComponent<BallController>();

        _saveState?.Invoke();

        rb.velocity = velocity;
        ball.transform.position = _centerPos;
        ball.transform.SetParent(_canvas.transform, true);
        ball.GetComponent<CircleCollider2D>().enabled = true;

        controller.Setup(velocity, BallCallback);

        ball.name = ball.name + " " + _counter++.ToString();
    }

    private void GetBallFromQueue(bool spawnAnother)
    {
        this._heldBall = _queueController.GetBall(spawnAnother);
        this._heldBall.transform.SetParent(this.transform);

        StartCoroutine(MoveBall(this._heldBall.transform, 0.4f, _centerPos));
    }

    IEnumerator MoveBall(Transform _ball, float overTime, Vector3 target)
    {
        float startTime = Time.time;

        while (Time.time < startTime + overTime)
        {
            _ball.transform.position = Vector3.Lerp(_ball.transform.position, target, (Time.time - startTime) / overTime);
            yield return null;
        }

        _ball.transform.position = target;
    }

    private void BallCallback(GameObject ball)
    {
        this._heldBall.layer = GameConfig.Layers.DEFAULT;

        if (_gridController.LastRowIsNotEmpty())
        {
            Debug.Log("Game Over, Last row contains bubble(s).");
        }

        StartCoroutine(CheckForWin());

        int destroyed = ball.GetComponent<BallController>().DestroySameColorOverLimit(GameConfig.COLORS_TO_DESTROY);
        if (destroyed == 0)
        {
            ScoreController.Instance.AddScore(-1);
            GetBallFromQueue(false);
            this._queueController.DeleteCell();
            this._queueController.AddBall();
        }
        else
        {
            //AudioController.Instance.PlaySound(SoundType.Good);
            ScoreController.Instance.AddScore(destroyed);
            GetBallFromQueue(true);
        }

        GunDisabler.Instance.RemoveActionWithDelay("Reloading", 0.25f);

        _QueueController.ChangePallete(); //Ìîæåò íå çàõâàòèòü øàðèêè îñòàâøèåñÿ â âîçäóõå
    }

    IEnumerator CheckForWin()
    {
        yield return new WaitForSeconds(0.6f);
        _gridController?.CheckForWin();
    }

    public void CheckForGameOver()
    {
        if (_gridController.LastRowIsNotEmpty())
            LevelState.Instance._State = LevelState.State.GameOver;
    }

    public GunState SaveState()
    {
        return new GunState(_heldBall.GetComponent<BallController>());
    }

    public void RestoreFrom(GunState state)
    {
        GameObject.Destroy(this._heldBall);

        this._heldBall = GameObject.Instantiate(_queueController._dummy);
        this._heldBall.transform.SetParent(this.transform);
        this._heldBall.transform.position = _centerPos;
        this._heldBall.transform.localScale = Vector3.one;

        this._heldBall.SetActive(true);
        this._heldBall.GetComponent<BallController>().Initialize(state.ballState.color, state.ballState.type);
    }

}
