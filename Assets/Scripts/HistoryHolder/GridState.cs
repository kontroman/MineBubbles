using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridState
{
    public List<List<BallState>> balls;

    public GridState(List<List<BallState>> balls)
    {
        this.balls = balls;
    }
}
