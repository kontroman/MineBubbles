using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunState
{
    public BallState ballState;

    public GunState(BallController heldBall)
    {
        ballState = new BallState(heldBall);
    }
}
