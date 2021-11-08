using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BallController : MonoBehaviour
{
    private CircleCollider2D _collider;
    private Rigidbody2D _rb;
    private TargetJoint2D _joint;
    private Vector2 _velocity;
    private GameObjectDelegate _onCollisionCallback;
    private List<Collider2D> _cells = new List<Collider2D>();
    private Image _image;
    private RectTransform rect;

    public bool stationary = false;
    public GameConfig.Types type { get; private set; }
    public GameConfig.Colors color { get; set; }

    public float radius
    {
        get { return _collider.radius; }
        set { _collider.radius = value; }
    }

    public void Setup(Vector2 initialVelocity, GameObjectDelegate onCollisionCallback)
    {
        this._velocity = initialVelocity;
        this._onCollisionCallback = onCollisionCallback;
    }

    public void Initialize(GameConfig.Colors color, GameConfig.Types type)
    {
        this._image = GetComponent<Image>();
        this.color = color;
        this.type = type;
        this.rect = GetComponent<RectTransform>();
        this.gameObject.name = GameConfig.ColorNames[(int)color];

        this._image.sprite = SpriteManager.Instance.LoadBall(color);

        this.rect.sizeDelta = new Vector2(rect.sizeDelta.x * 1.363f, rect.sizeDelta.y * 1.363f);
    }

    public List<Collider2D> GetNeighbors()
    {
        var list = Physics2D.OverlapCircleAll(transform.parent.position, 2 * GameConfig.BALL_RADIUS, 1 << GameConfig.Layers.DEFAULT)
                  .Where(coll => coll.tag == GameConfig.Tags.BALL && coll.gameObject != this.gameObject).ToList();
        return list;
    }

    public int DestroySameColorOverLimit(int limit)
    {
        List<Collider2D> balls = GetNeighbors().Where(b => b.GetComponent<BallController>().color == this.color).ToList();
        Queue<Collider2D> queue = new Queue<Collider2D>(balls);
        int destroyed = 0;
        balls.Add(this._collider);

        while (queue.Count != 0)
        {
            var ball = queue.Dequeue();

            List<Collider2D> neighbors = ball.GetComponent<BallController>().GetNeighbors().
                                         Where(n => n.GetComponent<BallController>().color == this.color
                                               && !balls.Contains(n)).ToList();

            balls.AddRange(neighbors);

            neighbors.ForEach(n => queue.Enqueue(n));
        }

        if (balls.Count >= limit)
        {
            destroyed = balls.Count;
            BallAnimator.Instance.DestroyBalls(balls.Select(b => b.gameObject).ToList());
          //  BallAnimator.Instance.DropBalls(balls.Select(a => a.gameObject).ToList());
        }

        return destroyed;
    }

    void Awake()
    {
        this._collider = GetComponent<CircleCollider2D>();
        this._rb = GetComponent<Rigidbody2D>();
        this._joint = GetComponent<TargetJoint2D>();
        this._velocity = _rb.velocity;
    }

    void Update()
    {
        if (stationary)
        {
            this._joint.target = this.transform.parent.position;

            if ((Vector2)transform.localPosition != Vector2.zero)
                transform.localPosition = Vector2.zero;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (stationary)
        {
            return;
        }
        switch (collision.gameObject.tag)
        {
            case GameConfig.Tags.BALL:
                Stop();
                AttachToClosest();
                _onCollisionCallback?.Invoke(this.gameObject);
                _onCollisionCallback = null;
                return;

            case GameConfig.Tags.WALL:
                ChangeTrajectory();
                return;

            case GameConfig.Tags.CEILING:
                Stop();
                AttachToClosest();
                _onCollisionCallback?.Invoke(this.gameObject);
                _onCollisionCallback = null;
                return;
        }
    }

    public void LoadNewSprite(GameConfig.Colors color)
    {
        this._image.sprite = SpriteManager.Instance.LoadBall(color);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        _cells.Add(other);
    }

    private void ChangeTrajectory()
    {
        _rb.velocity = new Vector2(-_velocity.x, _velocity.y);
        _velocity = _rb.velocity;
    }

    private void Stop()
    {
        GetComponent<CircleCollider2D>().radius = GameConfig.BALL_RADIUS;

        _velocity = Vector2.zero;
        _rb.velocity = Vector2.zero;
        _rb.rotation = 0f;
        _rb.angularVelocity = 0f;
    }

    private void AttachToClosest()
    {
        AudioController.Instance.PlaySound(SoundType.Fail);

        GameObject cell = ClosestEmptyCell();

        transform.SetParent(cell.transform);
        stationary = true;

        this._joint.enabled = true;
        this._joint.target = cell.transform.position;
    }

    private GameObject ClosestEmptyCell()
    {
        List<Collider2D> emptyCells = new List<Collider2D>(_cells.Where(c => c.transform.childCount == 0));

        return ClosestCell(emptyCells);
    }

    private GameObject ClosestCell(List<Collider2D> cells)
    {
        Collider2D closest = cells[0];

        float minDist = _collider.Distance(closest).distance;

        foreach (Collider2D coll in cells)
        {
            float dist = _collider.Distance(coll).distance;
            if (dist < minDist)
            {
                closest = coll;
                minDist = dist;
            }
        }

        return closest.gameObject;
    }
    public BallState SaveState()
    {
        return new BallState(this);
    }

    public void RestoreFrom(BallState state)
    {
        this.Initialize(state.color, state.type);
    }
}
