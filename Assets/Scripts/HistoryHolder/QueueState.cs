using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class QueueState
{
    public int cellLimit;
    public int currentCellCount;
    public List<BallState> balls;
    public List<GameConfig.Colors> colors;

    public QueueState(int cellLimit, int currentCellCount, List<GameObject> cells, List<GameConfig.Colors> _colors)
    {
        this.cellLimit = cellLimit;
        this.currentCellCount = currentCellCount;
        this.balls = new List<BallState>();
        this.colors = _colors.ToList();

        foreach (GameObject cell in cells)
        {
            if (cell.transform.childCount == 1)
            {
                balls.Add(new BallState(cell.transform.GetChild(0).GetComponent<BallController>()));
            }
        }
    }
}
