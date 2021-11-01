using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallState
{
    public GameConfig.Colors color;
    public GameConfig.Types type;

    public BallState(BallController ball)
    {
        this.color = ball.color;
        this.type = ball.type;
    }
}
