using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDisabler : MonoBehaviour
{
    public static GunDisabler Instance { get; private set; }

    private List<string> actions = new List<string>();

    private void Awake()
    {
        if (Instance != null) return;
        else Instance = this;

        Init();
    }

    public void AddAction(string action)
    {
        actions.Add(action);
    }

    public void ClearList()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.15f);
        actions.Clear();
    }

    public void RemoveAction(string action)
    {
        actions.Remove(actions.Find(x => x == action));
    }

    public void RemoveActionWithDelay(string action, float delay)
    {
        if (isActiveAndEnabled)
        {
            StartCoroutine(DestroyWithDelay(action, delay));
        }
    }

    IEnumerator DestroyWithDelay(string action, float delay)
    {
        yield return new WaitForSeconds(delay);
        var newAction = actions.Find(x => x == action);
        if (newAction != null)
        {
            actions.Remove(actions.Find(x => x == action));
        }
    }

    private void Init()
    {
        actions.Clear();
    }

    public bool CanShoot()
    {
        return actions.Count == 0;
    }
}
