using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodDestroyer : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
}
