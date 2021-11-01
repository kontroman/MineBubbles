using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchIcon : MonoBehaviour
{

    public GameObject iconOn;
    public GameObject iconOff;
    public void _switchIcon()
    {
        iconOn.SetActive(!iconOn.activeSelf);
        iconOff.SetActive(!iconOff.activeSelf);
    }

    
}
