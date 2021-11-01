using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallAnimator : MonoBehaviour
{
    public static BallAnimator Instance { get; private set; }

    public GameObject anchorPrefab;
    public GameObject canvas;

    void Awake()
    {
        if (Instance != null) return;
        else Instance = this;
    }

    public void DropBalls(List<GameObject> balls)
    {
        GameObject anchor = Instantiate(
            Resources.Load<GameObject>("Prefabs/FallingAnchor")
            , canvas.transform);

        anchor.transform.position = HighestItemPosition(balls);

        foreach (GameObject ball in balls)
        {
            ball.GetComponent<CircleCollider2D>().enabled = false;
            ball.GetComponent<BallController>().stationary = false;
            ball.GetComponent<TargetJoint2D>().enabled = false;
            ball.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            ball.layer = GameConfig.Layers.IGNORE_COLLISION;

            ball.transform.SetParent(anchor.transform, true);
        }

        StartCoroutine(DestroyAnimationFallingBalls(balls, 0.04f));

        var rb = anchor.GetComponent<Rigidbody2D>();
        var vx = UnityEngine.Random.Range(
            GameConfig.Limits.DROP_ANIM_VX_LEFT,
            GameConfig.Limits.DROP_ANIM_VX_RIGHT
        );
        var vy = UnityEngine.Random.Range(
            GameConfig.Limits.DROP_ANIM_VY_LOWER,
            GameConfig.Limits.DROP_ANIM_VY_UPPER
        );

        rb.AddForce(new Vector2(vx, vy));
    }

    IEnumerator DestroyAnimationFallingBalls(List<GameObject> balls, float delay)
    {
        foreach (var ball in balls)
        {
            if (ball == null) yield break;
            ball.layer = GameConfig.Layers.IGNORE_COLLISION;
            yield return new WaitForSeconds(delay);
            StartCoroutine(DestroyBallCoroutine(ball));
        }
    }

    public void DestroyBalls(List<GameObject> balls)
    {
        StartCoroutine(DestroyDelay(balls));
    }

    IEnumerator DestroyDelay(List<GameObject> balls)
    {
        foreach (var ball in balls)
        {
            if (ball == null) yield return null;
            Instantiate(Resources.Load<GameObject>("Prefabs/GoodAudio"), new Vector3(0, 0, 0), Quaternion.identity);
            yield return new WaitForSeconds(0.08f);
            ball.layer = GameConfig.Layers.IGNORE_COLLISION;
            StartCoroutine(DestroyBallCoroutine(ball));
        }
    }

    private IEnumerator DestroyBallCoroutine(GameObject ball)
    {
        GunDisabler.Instance.AddAction("RemovingBall");
        float expandTime = GameConfig.Durations.BALL_EXPLOSION_EXPAND_TIME;
        float shrinkTime = GameConfig.Durations.BALL_EXPLOSION_SHRINK_TIME;
        float elapsed = 0f;
        if (ball == null) { GunDisabler.Instance.RemoveAction("RemovingBall"); yield break; }
        Vector3 startScale = ball.transform.localScale;

        while (elapsed < expandTime)
        {
            if (ball == null) { GunDisabler.Instance.RemoveAction("RemovingBall"); yield break; }
            ball.transform.localScale = Vector3.Lerp(startScale, 1.4f * startScale, elapsed / expandTime);

            yield return null;
            elapsed += Time.deltaTime;
        }

        elapsed = 0f;
        if (ball == null) { GunDisabler.Instance.RemoveAction("RemovingBall"); yield break; };
        startScale = ball.transform.localScale;
        while (elapsed < shrinkTime)
        {
            if (ball == null) { GunDisabler.Instance.RemoveAction("RemovingBall"); yield break; }
            ball.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / shrinkTime);
            yield return null;
            elapsed += Time.deltaTime;
        }

        GunDisabler.Instance.RemoveAction("RemovingBall");
        if (ball == null) { GunDisabler.Instance.RemoveAction("RemovingBall"); yield break; }
        Destroy(ball);
    }

    private Vector2 HighestItemPosition(List<GameObject> items)
    {
        var max = items.Max(i => i.transform.position.y);
        return items.Find(i => i.transform.position.y == max).transform.position;
    }
}
