using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPadLayout : PortraitLayout
{
    public IPadLayout(
        GameObject canvas, 
        GameObject grid, 
        GameObject ceiling,
        GameObject leftWall,
        GameObject rightWall,
        GameObject gunAnchor,
        GameObject ballQueue,
        GameObject resetButton,
        GameObject restartButton,
        GameObject settingsButton,
        GameObject scoreText,
        GameObject border
        ) : base(canvas, grid, ceiling, leftWall, rightWall, gunAnchor, ballQueue, resetButton, restartButton, settingsButton, scoreText, border)
    {
        GameConfig.START_FILLER_ROW = 11;
        GameConfig.MAX_BALL_IN_ROW = 17;
        GameConfig.START_ROW_COUNT = CalculateRowCount();
    }
    private int CalculateRowCount()
    {
        return (int)((Screen.height - (Screen.height / 10)) / (Screen.width / GameConfig.MAX_BALL_IN_ROW));
    }
}
