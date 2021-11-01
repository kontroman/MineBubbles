using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayoutFactory
{
    public static ILayout Create
        (
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
        GameObject scoreText
        )
    {
        if (Mathf.Abs(Camera.main.aspect - 0.75f) < 0.0001f) //if tablet
        {
            return new IPadLayout(
                canvas,
                grid,
                ceiling,
                leftWall,
                rightWall,
                gunAnchor,
                ballQueue,
                resetButton,
                restartButton,
                settingsButton,
                scoreText
                );
        }
        else
            return new PortraitLayout(
                canvas,
                grid,
                ceiling,
                leftWall,
                rightWall,
                gunAnchor,
                ballQueue,
                resetButton,
                restartButton,
                settingsButton,
                scoreText
                );

    }
}
