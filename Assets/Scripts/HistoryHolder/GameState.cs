using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public GunState gunState;
    public GridState gridState;
    public QueueState queueState;
    public ScoreState scoreState;

    public GameState(GunState gunState, GridState gridState, QueueState queueState, ScoreState scoreState)
    {
        this.gunState = gunState;
        this.gridState = gridState;
        this.queueState = queueState;
        this.scoreState = scoreState;
    }
}
