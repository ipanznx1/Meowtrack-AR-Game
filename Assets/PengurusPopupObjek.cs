using UnityEngine;
using UnityEngine.UI;

public class PengurusPopupObjek : MonoBehaviour
{
    [Header("Paparan Popup")]
    [SerializeField] private GameObject popupSeterusnya;
    [SerializeField] private GameObject coverFlowUI;
    [SerializeField] private bool autoShowPadaStart = false;

    [Header("Tetapan Lain")]
    [SerializeField] private bool tutupPopupLainSemasaMula = true;

    private Button butangLanjut;

    private void Awake()
    {
        butangLanjut = GetComponentInChildren<Button>();
    }

    private void AutoLinkIfNeeded()
    {
        if (popupSeterusnya != null) return;

        PengurusPopupObjek[] semua = FindObjectsByType<PengurusPopupObjek>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (semua == null || semua.Length <= 1) return;

        // Prefer grouping by same parent if multiple popups share the same parent
        var sameParent = new System.Collections.Generic.List<PengurusPopupObjek>();
        foreach (var p in semua)
        {
            if (p.transform.parent == this.transform.parent) sameParent.Add(p);
        }

        PengurusPopupObjek[] urut;
        if (sameParent.Count > 1)
        {
            sameParent.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
            urut = sameParent.ToArray();
        }
        else
        {
            // fallback: sort by name
            System.Array.Sort(semua, (a, b) => string.Compare(a.name, b.name));
            urut = semua;
        }

        for (int i = 0; i < urut.Length; i++)
        {
            if (urut[i] == this)
            {
                if (i + 1 < urut.Length)
                {
                    popupSeterusnya = urut[i + 1].gameObject;
                }
                else
                {
                    popupSeterusnya = null; // last popup
                }
                break;
            }
        }
    }

    private void Start()
    {
        if (autoShowPadaStart)
        {
            TunjukPopupIni();
        }
        else
        {
            if (tutupPopupLainSemasaMula)
            {
                TutupPopupLain();
            }
        }
        // Auto-link sequence if the designer didn't assign next popup
        AutoLinkIfNeeded();
    }

    private void OnEnable()
    {
        Debug.Log($"[PengurusPopupObjek] OnEnable() for {gameObject.name}");
        if (butangLanjut != null)
        {
            Debug.Log($"[PengurusPopupObjek] Button found: {butangLanjut.name}, adding listener.");
            butangLanjut.onClick.RemoveAllListeners();
            butangLanjut.onClick.AddListener(PindahKePopupSeterusnya);
        }
        else
        {
            Debug.LogWarning($"[PengurusPopupObjek] NO BUTTON FOUND as child of {gameObject.name}!");
        }
    }

    public void TunjukPopupIni()
    {
        if (tutupPopupLainSemasaMula)
        {
            TutupPopupLain();
        }

        gameObject.SetActive(true);
    }

    public void PindahKePopupSeterusnya()
    {
        Debug.Log($"[PengurusPopupObjek] PindahKePopupSeterusnya() called from {gameObject.name}");
        if (popupSeterusnya != null)
        {
            Debug.Log($"[PengurusPopupObjek] Showing next popup: {popupSeterusnya.name}");
            gameObject.SetActive(false);
            popupSeterusnya.SetActive(true);
            return;
        }

        Debug.Log($"[PengurusPopupObjek] No next popup. Hiding {gameObject.name}");
        gameObject.SetActive(false);

        if (coverFlowUI != null)
        {
            Debug.Log("[PengurusPopupObjek] Activating CoverFlowUI.");
            coverFlowUI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[PengurusPopupObjek] coverFlowUI is NULL!");
        }

        var coverFlow = coverFlowUI != null ? coverFlowUI.GetComponent<CoverFlowUI>() : null;
        if (coverFlow != null)
        {
            Debug.Log("[PengurusPopupObjek] Calling CoverFlowUI.ResetDanMunculTutorial().");
            coverFlow.ResetDanMunculTutorial();
        }
    }

    private void TutupPopupLain()
    {
        PengurusPopupObjek[] semuaPopup = FindObjectsByType<PengurusPopupObjek>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var popup in semuaPopup)
        {
            if (popup != this)
            {
                popup.gameObject.SetActive(false);
            }
        }
    }
}
