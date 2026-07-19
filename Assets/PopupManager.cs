using UnityEngine;

public class PopupManager : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("[PopupManager] Awake() called.");
        HideAllPopups();
    }

    private void HideAllPopups()
    {
        PengurusPopupObjek[] allPopups = GetComponentsInChildren<PengurusPopupObjek>(true);
        Debug.Log($"[PopupManager] Found {allPopups.Length} popups to hide.");
        foreach (var popup in allPopups)
        {
            Debug.Log($"[PopupManager] Hiding popup: {popup.gameObject.name}");
            popup.gameObject.SetActive(false);
        }
        Debug.Log("[PopupManager] All popups hidden at start.");
    }

    public void ShowPopup(PengurusPopupObjek popup)
    {
        if (popup != null)
        {
            popup.gameObject.SetActive(true);
        }
    }
}
