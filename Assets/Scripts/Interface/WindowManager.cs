using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindowType
{
    HardMod,
    Settings,
    GameOver,
    Restart,
    GameWin
}

public class WindowManager : MonoBehaviour
{
    public static WindowManager Instance { get; private set; }

    public Transform locale;
    private GameObject actualWindow;
    private GameObject gunAnchor;

    private void Start()
    {
        if (Instance != null) return;
        else Instance = this;
        locale = GameObject.Find("MenuCanvas").transform;
        gunAnchor = GameObject.Find("GunAnchor");
    }

    public void OpenWindow(WindowType _type)
    {
        GunDisabler.Instance.AddAction("ShowingWindow");

        if (actualWindow != null)
            CloseWindow(actualWindow.GetComponent<Window>());

        GameObject window = Instantiate(Resources.Load<GameObject>("Prefabs/Windows/" + _type.ToString()));
        window.transform.SetParent(locale, false);
        window.transform.localPosition = Vector3.zero;

        actualWindow = window;
        gunAnchor.SetActive(false);
    }

    public void CloseWindow(Window _window)
    {
        GunDisabler.Instance.RemoveAction("ShowingWindow");
        gunAnchor.SetActive(true);
        Destroy(_window.gameObject);
    }
}
