using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class RayRenderer
{
    public float length = 2000f;
    public int maxSteps = GameConfig.MAX_RAY_REFLECTIONS;

    private UILineRenderer _lineRenderer;
    public UILineRenderer _uiline { get { return _lineRenderer; } }
    private List<Vector2> _points = new List<Vector2>();
    private Material lineMat;

    public RayRenderer(UILineRenderer lineRenderer)
    {
        this._lineRenderer = lineRenderer;
        lineMat = _lineRenderer.GetComponent<UILineRenderer>().material;
    }

    public void CastRay(Vector2 origin, Vector2 direction, float yOffset = 50f)
    {
        ClearPoints();

        float radius = GameConfig.BALL_RADIUS / 2;
        int steps = 0;
        Vector2 offset = new Vector2(0, yOffset);
        Vector2 lastPoint = origin;


        AddPoint(lastPoint + offset - origin);

        while (steps++ < maxSteps)
        {
            var hit = Physics2D.CircleCast(lastPoint, radius, direction);
            lastPoint = hit.centroid;
            AddPoint(lastPoint + offset - origin);

            if (InvalidCollider(hit.collider))
            {
                break;
            }

            direction = Vector2.Reflect(direction, hit.normal);
        }

        _lineRenderer.Points = _points.ToArray();
    }

    private bool InvalidCollider(Collider2D collider)
    {
        return collider == null || collider.gameObject.tag == GameConfig.Tags.CEILING || collider.gameObject.tag == GameConfig.Tags.BALL;
    }

    public void AddPoint(Vector2 point)
    {
        if (LevelDifficult.Instance._Difficult == Difficult.Hard)
        {
            if (_points.Count >= 2)
                return;
        }

        _points.Add(point);

        if (LevelDifficult.Instance._Difficult == Difficult.Hard)
        {
            if (_points.Count > 1)
                _points[1] = Vector2.ClampMagnitude(point, 500f);
        }
    }

    public void ClearPoints()
    {
        _points.Clear();
    }

    public void SetupRay()
    {
        if (LevelDifficult.Instance._Difficult == Difficult.Normal)
        {
            _lineRenderer.material = Resources.Load<Material>("RaySettings/DotMaterial");
            _lineRenderer.sprite = Resources.Load<Sprite>("RaySettings/Circle");
        }
        else
        {
            _lineRenderer.material = Resources.Load<Material>("RaySettings/Arrow");
            _lineRenderer.sprite = Resources.Load<Sprite>("RaySettings/Arrow2");
        }
    }
}
