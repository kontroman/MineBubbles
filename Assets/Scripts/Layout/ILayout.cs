using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILayout
{
    void Setup();
    Vector2 ScreenScale();
    float CalculateRadius();
}
