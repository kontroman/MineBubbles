using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPointer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GunDisabler.Instance.AddAction("MouseOnUI");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(waiter());
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(0.2f);
        GunDisabler.Instance.RemoveAction("MouseOnUI");
    }

    private void OnDisable()
    {
        GunDisabler.Instance.RemoveActionWithDelay("MouseOnUI", 0.07f);
    }
    private void OnDestroy()
    {
        GunDisabler.Instance.RemoveActionWithDelay("MouseOnUI", 0.07f);
    }
}
