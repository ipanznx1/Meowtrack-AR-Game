using UnityEngine;

// Attach this to an empty GameObject in the scene to automatically show info popups
// on a mobile device for testing. It calls PengesanSentuhAR.DebugShowFirstSensor().
public class ForcePopupTester : MonoBehaviour
{
    public float delaySeconds = 2f;

    void Start()
    {
        // Only auto-run on device or when explicitly enabled in editor
        if (Application.isEditor)
        {
            Debug.Log("[ForcePopupTester] Running in Editor — skipping automatic debug show. Use the context menu to trigger manually.");
            return;
        }

        Invoke(nameof(TriggerDebugShow), delaySeconds);
    }

    void TriggerDebugShow()
    {
        var pengesan = FindObjectOfType<PengesanSentuhAR>();
        if (pengesan == null)
        {
            Debug.LogWarning("[ForcePopupTester] No PengesanSentuhAR found in scene.");
            return;
        }

        bool ok = pengesan.DebugShowFirstSensor();
        Debug.Log("[ForcePopupTester] DebugShowFirstSensor returned: " + ok);
    }
}
