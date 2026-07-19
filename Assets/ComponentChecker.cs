using UnityEngine;

public class ComponentChecker : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("===== COMPONENT CHECKER START =====");
        
        PopupManager pm = FindAnyObjectByType<PopupManager>();
        Debug.Log("PopupManager in scene: " + (pm != null ? "FOUND on " + pm.gameObject.name : "NOT FOUND"));
        
        GreetingWatcher gw = FindAnyObjectByType<GreetingWatcher>();
        Debug.Log("GreetingWatcher in scene: " + (gw != null ? "FOUND on " + gw.gameObject.name : "NOT FOUND"));
        
        PengurusAliranPopupAR pap = FindAnyObjectByType<PengurusAliranPopupAR>();
        Debug.Log("PengurusAliranPopupAR in scene: " + (pap != null ? "FOUND on " + pap.gameObject.name : "NOT FOUND"));
        
        PengurusPopupObjek[] popups = FindObjectsByType<PengurusPopupObjek>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log("PengurusPopupObjek objects found: " + popups.Length);
        foreach (var p in popups)
        {
            Debug.Log("  - " + p.gameObject.name);
        }
        
        Debug.Log("===== COMPONENT CHECKER END =====");
    }
}
