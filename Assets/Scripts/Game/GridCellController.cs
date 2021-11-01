using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCellController : MonoBehaviour
{
    private BoxCollider2D _collider;

    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    public void Setup()
    {
        _collider.edgeRadius = GameConfig.BALL_RADIUS;
    }
}
