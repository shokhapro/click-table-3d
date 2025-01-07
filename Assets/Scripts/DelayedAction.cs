using System.Collections;
using UnityEngine;
using UnityEngine.Events;

internal static class Delays
{
    public static Coroutine DelayedAction(this MonoBehaviour script, float time, UnityAction action)
    {
        IEnumerator DelayedActionCoroutine()
        {
            if (time > 0) yield return new WaitForSeconds(time);
            else yield return new WaitForEndOfFrame();

            action.Invoke();
        }
        
        return script.StartCoroutine(DelayedActionCoroutine());
    }
}