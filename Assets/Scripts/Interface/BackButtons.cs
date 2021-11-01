using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtons : MonoBehaviour
{
    [SerializeField]
    private string BottomHex;

    [SerializeField]
    private string TopHex;

    public void SetNewBackground()
    {
        BackgroundChanger.Instance.SetNewColors(TopHex, BottomHex);
    }
}
