using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingAnchorController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private float yOffset;

    void Awake()
    {
        GunDisabler.Instance.AddAction("FallingBalls");

        this._rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        yOffset = GameConfig.BALL_RADIUS * 2;
    }

    void Update()
    {
        if (this.transform.position.y < 0 - yOffset)
        {
            GunDisabler.Instance.RemoveAction("FallingBalls");
            GunDisabler.Instance.RemoveAction("RemovingBall");
            GameObject.Destroy(this.gameObject);
        }
    }
}
