using System;
using System.Collections;
using UnityEngine;

public static class CoroutineExtension
{
    public static Coroutine WaitSecond(this MonoBehaviour component, float second, Action actionAfter)
    {
        return component.StartCoroutine(WaitTo(second, actionAfter));
    }

    private static IEnumerator WaitTo(float wait, Action actionAfter)
    {
        yield return new WaitForSeconds(wait);

        actionAfter.Invoke();
    }
}
