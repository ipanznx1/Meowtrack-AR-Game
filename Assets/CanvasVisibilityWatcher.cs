using UnityEngine;
using System.Diagnostics;

// Attach to the Canvas_Maklumat GameObject to detect when it becomes inactive and capture a stack trace for debugging.
public class CanvasVisibilityWatcher : MonoBehaviour
{
    private bool lastActiveState;

    void Start()
    {
        lastActiveState = gameObject.activeInHierarchy;
    }

    void Update()
    {
        bool current = gameObject.activeInHierarchy;
        if (current != lastActiveState)
        {
            UnityEngine.Debug.Log("[CanvasVisibilityWatcher] Canvas active state changed to: " + current);
            // Capture stack trace to help find who deactivated it (best-effort).
            var st = new StackTrace(true);
            UnityEngine.Debug.Log("[CanvasVisibilityWatcher] StackTrace at detection:\n" + st.ToString());
            lastActiveState = current;
        }
    }
}
